using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace SimpleNote.Tests;

public class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().WithImage("postgres:16").Build();
    private readonly RedisContainer _redis = new RedisBuilder().WithImage("redis:7").Build();

    public string RedisConnectionString => _redis.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", _db.GetConnectionString());    
        builder.UseSetting("ConnectionStrings:Redis", _redis.GetConnectionString());
        builder.UseSetting("Seq:ServiceUrl", "");
    }
    
    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        await _redis.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _redis.DisposeAsync();
        await base.DisposeAsync();
    }
}