using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SimpleNote.Caching;
using SimpleNote.Notes;

namespace SimpleNote.Tests;

public class CachedNoteRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenNoteIsCached_ReturnsWithoutQueryingTheDatabase()
    {
        var repo = new FakeNoteRepository();
        var cache = new FakeCache();
        var id = Guid.NewGuid();
        var settings = new CacheSettings();
        await cache.SetAsync($"{settings.keyPrefix}:{id}", new Note{Id=id, Content="Ki ase Jibone"}, TimeSpan.FromSeconds(60));
        
        var cachedNoteRepo = new CachedNoteRepository(repo, cache, Options.Create(new CacheSettings()), NullLogger<CachedNoteRepository>.Instance);
        var result = await cachedNoteRepo.GetByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal("Ki ase Jibone", result.Content);
        Assert.Equal(0, repo.GetByIdCalls);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoteIsNotCached_QueriesDatabaseOnceAndPopulatesCache()
    {
        var repo = new FakeNoteRepository();
        var cache = new FakeCache();
        var id = Guid.NewGuid();
        var settings = new CacheSettings();
        var msg = "some message";
        repo.Notes.Add(id, new Note{Id=id, Content = msg});
        var cachedNoteRepo = new CachedNoteRepository(repo, cache, Options.Create(settings), NullLogger<CachedNoteRepository>.Instance);
        var result = await cachedNoteRepo.GetByIdAsync(id);
        
        Assert.NotNull(result);
        Assert.Equal(msg, result.Content);
        Assert.Equal(1, repo.GetByIdCalls);
        Assert.Equal(cache.Cache?.ContainsKey($"{settings.keyPrefix}:{id}"), true);
    }

    [Fact]
    public async Task UpdateAsync_WhenCached_EvictsCache()
    {
        var repo = new FakeNoteRepository();
        var cache = new FakeCache();
        var id = Guid.NewGuid();
        var msg = "some message";
        var settings = new CacheSettings();

        // The row must exist in the repository too: UpdateAsync returns false for a missing id
        // and never reaches the invalidation, so without this the test proves nothing.
        repo.Notes.Add(id, new Note{Id=id, Content = msg});
        cache.Cache.Add($"{settings.keyPrefix}:{id}", new Note{Id=id, Content = msg});

        var cachedNoteRepo = new CachedNoteRepository(repo, cache, Options.Create(settings), NullLogger<CachedNoteRepository>.Instance);
        Assert.True(cache.Cache.ContainsKey($"{settings.keyPrefix}:{id}"));
        var result = await cachedNoteRepo.UpdateAsync(id, "hjgjh");
        Assert.True(result);
        Assert.False(cache.Cache.ContainsKey($"{settings.keyPrefix}:{id}"));
    } 
}