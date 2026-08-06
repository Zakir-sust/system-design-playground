using Microsoft.Extensions.Options;

namespace ThrottleApi.RateLimiting;

/// <summary>
/// The HTTP half of rate limiting: identify the caller, ask <see cref="IRateLimiter"/> whether they
/// may proceed, and either continue the pipeline or short-circuit with 429. No counting logic lives
/// here — that belongs to the limiter, which is what makes the algorithms swappable.
/// </summary>
public sealed class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _limiter;
    private readonly RateLimitSettings _settings;
    private readonly ILogger<RateLimitMiddleware> _logger;

    public RateLimitMiddleware(
        RequestDelegate next,
        IRateLimiter limiter,
        IOptions<RateLimitSettings> settings,
        ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _limiter = limiter;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // The health probe is the orchestrator's business, not a caller's. Limiting it means a noisy
        // client can starve the probe and get the replica marked unhealthy — a self-inflicted outage.
        if (!_settings.Enabled || context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var key = ResolveClientKey(context);
        var decision = await _limiter.TryAcquireAsync(key, context.RequestAborted);

        // Indexer, not Headers.Add: Add throws when the key is already present, and a middleware
        // that runs on every request is exactly where a duplicate eventually bites you.
        context.Response.Headers["X-RateLimit-Limit"] = _settings.PermitLimit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = decision.Remaining.ToString();

        if (decision.Allowed)
        {
            await _next(context);
            return;
        }

        // Short-circuit: _next is never called, so the endpoint does no work at all. Everything here
        // must happen before the first byte of the body — once the response has started, headers and
        // status code are frozen.
        _logger.LogWarning("Rate limit exceeded for {ClientKey} on {Path}", key, context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.Headers.RetryAfter =
            ((int)Math.Ceiling(decision.RetryAfter.TotalSeconds)).ToString();
        context.Response.ContentType = "text/plain";

        await context.Response.WriteAsync("Too many requests. Try again later.");
    }

    /// <summary>
    /// A stable identifier for the caller. Whatever you pick ends up in a Redis key, so keep it
    /// bounded — an attacker who controls the key can otherwise fill Redis with junk.
    /// </summary>
    private static string ResolveClientKey(HttpContext context) =>
        // RemoteIpAddress is null for connections with no remote endpoint (in-memory test servers,
        // Unix sockets). Falling back to a shared bucket is the safe default: unknown callers share
        // one budget rather than each getting an unlimited one.
        //
        // Behind the gateway every request appears to come from the proxy, so in M3 you will need
        // ForwardedHeaders — and only trust that header from proxies you control, since a client can
        // forge it and mint itself a fresh budget per request.
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
