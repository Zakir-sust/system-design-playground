namespace ThrottleApi.RateLimiting;

/// <summary>
/// The answer to "may this caller proceed?", plus what the response needs in order to tell the
/// client how to behave: how much budget is left, and how long to wait after a rejection.
/// </summary>
/// <param name="Allowed">Whether the request may continue down the pipeline.</param>
/// <param name="Remaining">Requests left in the current window (0 when rejected).</param>
/// <param name="RetryAfter">How long until the caller may try again (<see cref="TimeSpan.Zero"/> when allowed).</param>
public readonly record struct RateLimitDecision(bool Allowed, long Remaining, TimeSpan RetryAfter)
{
    public static RateLimitDecision Allow(long remaining) => new(true, remaining, TimeSpan.Zero);

    public static RateLimitDecision Deny(TimeSpan retryAfter) => new(false, 0, retryAfter);
}
