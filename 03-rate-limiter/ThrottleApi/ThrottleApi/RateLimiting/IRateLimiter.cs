namespace ThrottleApi.RateLimiting;

/// <summary>
/// One algorithm, expressed without HTTP or Redis types. The middleware talks to this, so swapping
/// in-memory for Redis (M3) or fixed window for token bucket (M4) is a one-line DI change and the
/// implementations can be unit tested on their own.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Spends one unit of the caller's budget and reports whether it was available. Implementations
    /// must be safe to call concurrently — that race is the whole point of the exercise.
    /// </summary>
    /// <param name="key">Identifies the caller (IP, API key, user id).</param>
    Task<RateLimitDecision> TryAcquireAsync(string key, CancellationToken cancellationToken = default);
}
