namespace ThrottleApi.RateLimiting;

public sealed class RateLimitSettings
{
    public const string SectionName = "RateLimit";

    /// <summary>Kill switch, so the limiter can be turned off without a rebuild.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>How many requests one client may make per window.</summary>
    public int PermitLimit { get; set; } = 10;

    public int WindowSeconds { get; set; } = 60;

    /// <summary>Namespaces the Redis keys. Bump the version when the key layout changes.</summary>
    public string KeyPrefix { get; set; } = "ratelimit:v1";

    public TimeSpan Window => TimeSpan.FromSeconds(WindowSeconds);
}
