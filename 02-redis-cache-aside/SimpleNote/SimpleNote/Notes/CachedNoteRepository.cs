using Microsoft.Extensions.Options;
using SimpleNote.Caching;

namespace SimpleNote.Notes;

/// <summary>
/// Cache-aside decorator over <see cref="INoteRepository"/>. Callers resolve this and
/// cannot tell a cache is involved; every read and write of a note goes through here,
/// which is what makes "you must invalidate" enforceable rather than remembered.
/// </summary>
public sealed class CachedNoteRepository : INoteRepository
{
    private readonly INoteRepository _inner;
    private readonly ICache _cache;
    private readonly ILogger<CachedNoteRepository> _logger;
    private readonly TimeSpan _ttl;

    public CachedNoteRepository(
        INoteRepository inner,
        ICache cache,
        IOptions<CacheSettings> settings,
        ILogger<CachedNoteRepository> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
        _ttl = TimeSpan.FromSeconds(settings.Value.TtlSeconds);
    }

    // The single definition of the key. Read and invalidation cannot drift apart.
    private static string CacheKey(Guid id) => $"note:{id}";

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<Note>(CacheKey(id));
        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT: note {Id}", id);
            return cached;
        }

        _logger.LogInformation("Cache MISS: note {Id}", id);
        var note = await _inner.GetByIdAsync(id);
        if (note is not null)
        {
            await _cache.SetAsync(CacheKey(id), note, _ttl);
        }

        return note;
    }

    public async Task<bool> UpdateAsync(Guid id, string content)
    {
        var updated = await _inner.UpdateAsync(id, content);
        if (!updated) return false;

        // Invalidate *after* the write commits. Evicting first would let a concurrent
        // reader re-populate the cache with the pre-update row before the new one lands,
        // leaving a stale entry with a fresh TTL.
        var evicted = await _cache.RemoveAsync(CacheKey(id));
        _logger.LogInformation("Updated note {Id}, cache evicted: {Evicted}", id, evicted);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await _inner.DeleteAsync(id);
        if (!deleted) return false;

        var evicted = await _cache.RemoveAsync(CacheKey(id));
        _logger.LogInformation("Deleted note {Id}, cache evicted: {Evicted}", id, evicted);
        return true;
    }

    // Nothing to invalidate: a brand new id cannot already be cached.
    public Task<Note> AddAsync(string content) => _inner.AddAsync(content);

    // Deliberately uncached — see the note in Program.cs.
    public Task<IReadOnlyList<Note>> GetAllAsync() => _inner.GetAllAsync();
}
