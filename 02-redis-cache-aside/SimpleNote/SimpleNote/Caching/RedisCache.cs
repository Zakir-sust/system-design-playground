using System.Text.Json;
using StackExchange.Redis;

namespace SimpleNote.Caching;

/// <summary>
/// The only class in the app that knows Redis exists.
/// </summary>
public sealed class RedisCache : ICache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IConnectionMultiplexer _redis;

    public RedisCache(IConnectionMultiplexer redis) => _redis = redis;

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _redis.GetDatabase().StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!, JsonOptions) : default;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl) =>
        _redis.GetDatabase().StringSetAsync(key, JsonSerializer.Serialize(value, JsonOptions), ttl);

    public Task<bool> RemoveAsync(string key) =>
        _redis.GetDatabase().KeyDeleteAsync(key);
}
