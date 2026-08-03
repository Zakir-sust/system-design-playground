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
    private readonly string _keyPrefix;

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
        _keyPrefix = settings.Value.keyPrefix;
    }

    // The single definition of the key. Read and invalidation cannot drift apart.
    private string CacheKey(Guid id) => $"{_keyPrefix}:{id}";

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        try
        {
            var cached = await _cache.GetAsync<Note>(CacheKey(id));
            if (cached is not null)
            {
                _logger.LogInformation("Cache HIT: note {Id}", id);
                return cached;
            }

            _logger.LogInformation("Cache MISS: note {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache unavailable reading note {Id}; falling back to the database", id);
        }

        var note = await _inner.GetByIdAsync(id);
        if (note is not null)
        {
            try
            {
                await _cache.SetAsync(CacheKey(id), note, _ttl);
            }
            catch (Exception ex)
            {
                // Failing to populate the cache costs performance, never correctness. Serve the note.
                _logger.LogError(ex, "Cache unavailable writing note {Id}", id);
            }
        }

        return note;
    }

    public async Task<bool> UpdateAsync(Guid id, string content)
    {
        var updated = await _inner.UpdateAsync(id, content);
        if (!updated) return false;


        await InvalidateAsync(id, "updated");
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await _inner.DeleteAsync(id);
        if (!deleted) return false;

        await InvalidateAsync(id, "deleted");
        return true;
    }

    private async Task InvalidateAsync(Guid id, string operation)
    {
        try
        {
            var evicted = await _cache.RemoveAsync(CacheKey(id));
            _logger.LogInformation("Note {Id} {Operation}, cache evicted: {Evicted}", id, operation, evicted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Note {Id} {Operation} but cache eviction FAILED; entry is stale until TTL expires",
                id, operation);
        }
    }

    // Nothing to invalidate: a brand new id cannot already be cached.
    public Task<Note> AddAsync(string content) => _inner.AddAsync(content);

    // Deliberately uncached — see the note in Program.cs.
    public Task<IReadOnlyList<Note>> GetAllAsync() => _inner.GetAllAsync();
}
