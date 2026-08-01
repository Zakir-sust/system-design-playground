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
            // A cache failure is not a miss. A miss means "Redis answered, and it does not have this";
            // this means "Redis did not answer at all". Logging it as a miss would quietly corrupt the
            // hit-ratio metric that PLAN.md asks you to add later. Either way, read through to Postgres.
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

        // Invalidate *after* the write commits. Evicting first would let a concurrent
        // reader re-populate the cache with the pre-update row before the new one lands,
        // leaving a stale entry with a fresh TTL.
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
            // Deliberately swallowed: the database write has already committed, so failing the request
            // now would tell the caller their change did not happen when it did. The cost is a stale
            // entry until the TTL expires — which is precisely what the TTL is there for. This is the
            // one log line in the app that genuinely means "someone is being served wrong data".
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
