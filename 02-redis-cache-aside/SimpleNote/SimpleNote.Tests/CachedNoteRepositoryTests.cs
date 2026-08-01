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
        await cache.SetAsync($"note:{id}", new Note{Id=id, Content="Ki ase Jibone"}, TimeSpan.FromSeconds(60));
        
        var cachedNoteRepo = new CachedNoteRepository(repo, cache, Options.Create(new CacheSettings()), NullLogger<CachedNoteRepository>.Instance);
        var result = await cachedNoteRepo.GetByIdAsync(id);
        Assert.Equal(0, repo.GetByIdCalls);
    }
}