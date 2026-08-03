namespace SimpleNote.Caching;

public sealed class CacheSettings
{
    public const string SectionName = "Cache";
    public string keyPrefix { get; set; } = "note:v1";
    public int TtlSeconds { get; set; } = 60;
}
