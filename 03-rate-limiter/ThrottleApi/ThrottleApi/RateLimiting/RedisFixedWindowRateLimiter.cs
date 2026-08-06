using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ThrottleApi.RateLimiting;

/// <summary>
/// M3: the same fixed window, but the counter lives in Redis so every replica shares it. Swap this
/// in for <see cref="InMemoryFixedWindowRateLimiter"/> in Program.cs once you have reproduced the
/// per-replica bug.
/// </summary>
public sealed class RedisFixedWindowRateLimiter : IRateLimiter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly RateLimitSettings _settings;
    private readonly TimeProvider _time;

    public RedisFixedWindowRateLimiter(
        IConnectionMultiplexer redis,
        IOptions<RateLimitSettings> settings,
        TimeProvider time)
    {
        _redis = redis;
        _settings = settings.Value;
        _time = time;
    }

    public Task<RateLimitDecision> TryAcquireAsync(string key, CancellationToken cancellationToken = default) =>
        // TODO M3 — build a key that includes the window (e.g. "{KeyPrefix}:{key}:{windowStart}") so
        // old windows expire themselves, then StringIncrementAsync + KeyExpireAsync.
        //
        // Then ask the question the interviewer will ask: INCR and EXPIRE are two round-trips. What
        // happens if the process dies between them, or if two requests both see count == 1? The fix
        // is a Lua script (ScriptEvaluateAsync) that increments, sets the TTL, and returns the count
        // in one atomic step. Getting there is the improvement listed in PLAN.md.
        throw new NotImplementedException("M3: Redis-backed fixed window — see PLAN.md.");
}
