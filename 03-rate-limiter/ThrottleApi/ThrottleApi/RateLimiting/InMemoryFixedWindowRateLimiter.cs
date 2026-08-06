using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace ThrottleApi.RateLimiting;

/// <summary>
/// M1: a fixed-window counter held in process memory. Deliberately flawed in two ways you will
/// demonstrate later — the boundary burst (M2) and the fact that each replica counts on its own
/// (M3). Registered as a singleton, so the state field is shared across concurrent requests.
/// </summary>
public sealed class InMemoryFixedWindowRateLimiter : IRateLimiter
{
    private readonly RateLimitSettings _settings;
    private readonly TimeProvider _time;

    // Window start and count must move together, so they live in one value that is replaced as a
    // unit. Nothing evicts idle keys — they leak, which Redis solves for free with a TTL in M3.
    private readonly ConcurrentDictionary<string, (DateTimeOffset WindowStart, int Count)> _windows = new();

    public InMemoryFixedWindowRateLimiter(IOptions<RateLimitSettings> settings, TimeProvider time)
    {
        _settings = settings.Value;
        _time = time;
    }

    public Task<RateLimitDecision> TryAcquireAsync(string key, CancellationToken cancellationToken = default)
    {
        var now = _time.GetUtcNow();
        var window = _settings.Window;

        // Read, decide and write as one atomic operation. The update factory is a pure function of
        // the value it is handed, which is what makes it safe for ConcurrentDictionary to run it
        // again when another thread wins the race: it just recomputes from the newer value.
        var entry = _windows.AddOrUpdate(
            key,
            _ => (now, 1),
            (_, existing) => now - existing.WindowStart >= window
                ? (now, 1)                                       // previous window lapsed: start fresh
                : (existing.WindowStart, existing.Count + 1));

        // Decide from the value that was stored, never from one read beforehand — this count is the
        // one this request actually claimed.
        if (entry.Count <= _settings.PermitLimit)
        {
            return Task.FromResult(RateLimitDecision.Allow(_settings.PermitLimit - entry.Count));
        }

        // Round up. A Retry-After of 0 tells the client to come straight back, which is exactly the
        // hammering the limiter exists to stop.
        var retryAfter = entry.WindowStart + window - now;
        return Task.FromResult(
            RateLimitDecision.Deny(TimeSpan.FromSeconds(Math.Ceiling(retryAfter.TotalSeconds))));
    }
}
