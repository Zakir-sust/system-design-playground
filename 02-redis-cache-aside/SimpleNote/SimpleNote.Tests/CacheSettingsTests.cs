using SimpleNote.Caching;

namespace SimpleNote.Tests;

public class CacheSettingsTests
{

    [Fact]
    public void TtlSeconds_WhenNotConfigured_DefaultTo60()
    {
        var settings = new CacheSettings();
        Assert.Equal(60, settings.TtlSeconds);
    }
}