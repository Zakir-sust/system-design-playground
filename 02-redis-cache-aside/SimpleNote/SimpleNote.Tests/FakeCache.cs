using SimpleNote.Caching;

namespace SimpleNote.Tests;

public class FakeCache : ICache
{
    public Dictionary<string, object> Cache { get; } = new Dictionary<string, object>();
    public Task<T?> GetAsync<T>(string key)
    {
        if (Cache.ContainsKey(key)) return Task.FromResult((T?)Cache[key]);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        Cache[key] = value;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key)
    {
        return Task.FromResult(Cache.Remove(key));
    }
}