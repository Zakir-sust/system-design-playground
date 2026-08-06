namespace SimpleNote.Tests;

public class NoteCacheIntegrationTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;

    public NoteCacheIntegrationTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Foo() { var client = _fixture.CreateClient(); /* ... */ }
}