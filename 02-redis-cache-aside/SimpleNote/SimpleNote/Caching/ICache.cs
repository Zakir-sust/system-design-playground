namespace SimpleNote.Caching;

/// <summary>
/// Cache operations, expressed without any Redis types so callers can be unit tested
/// against a fake. Serialization is the implementation's problem, not the caller's.
/// </summary>
public interface ICache
{
    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan ttl);

    /// <returns><c>true</c> if the key existed and was removed.</returns>
    Task<bool> RemoveAsync(string key);
}
