using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleNote;
using SimpleNote.Caching;
using SimpleNote.Notes;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!);
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>(), name: "redis");

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(CacheSettings.SectionName));
builder.Services.AddSingleton<ICache, RedisCache>();

builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<INoteRepository>(sp => new CachedNoteRepository(
    sp.GetRequiredService<NoteRepository>(),
    sp.GetRequiredService<ICache>(),
    sp.GetRequiredService<IOptions<CacheSettings>>(),
    sp.GetRequiredService<ILogger<CachedNoteRepository>>()));

// Only wire up Seq when it is actually configured. Passing null to WriteTo.Seq throws while the
// host is being built, which in a test run shows up as every test failing for an unrelated reason.
var seqServerUrl = builder.Configuration["Seq:ServerUrl"];

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
    if (!string.IsNullOrWhiteSpace(seqServerUrl))
    {
        config.WriteTo.Seq(seqServerUrl);
    }
});

var app = builder.Build();

// Warn here rather than inside the UseSerilog callback: that callback runs *while* the logger is
// being constructed, so Log.Logger is still the silent default and anything written there is
// discarded. By this point Build() has assigned the real logger.
if (string.IsNullOrWhiteSpace(seqServerUrl))
{
    Log.Warning("Seq:ServerUrl is not configured; logging to console only");
}

app.MapGet("/", () => "Hello World!");
app.MapNoteEndpoints();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Fail loudly. An app that cannot reach its database has not started, and continuing here
    // surfaces later as "relation Notes does not exist", which sends you debugging EF Core
    // instead of the connection string that is actually wrong.
    if (!db.Database.CanConnect())
    {
        Log.Fatal("Could not connect to the database");
        throw new InvalidOperationException(
            "Could not connect to the database. Check ConnectionStrings:DefaultConnection.");
    }

    Log.Information("Connected to database, applying migrations");
    db.Database.Migrate();
}

app.Run();

// Top-level statements compile to an internal Program class, which WebApplicationFactory<Program>
// in the test project cannot see. Re-declaring it as public partial makes it visible.
public partial class Program { }
