namespace SimpleNote.Caching;

public sealed class CacheSettings
{
    public const string SectionName = "Cache";

    /// <summary>
    /// How long a cached note survives without being invalidated. This is the backstop:
    /// if an invalidation is ever missed, staleness is bounded by this value rather than forever.
    /// </summary>
    public int TtlSeconds { get; set; } = 60;
}
