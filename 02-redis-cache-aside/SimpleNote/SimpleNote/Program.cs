using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleNote;
using SimpleNote.Caching;
using SimpleNote.Notes;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(CacheSettings.SectionName));
builder.Services.AddSingleton<ICache, RedisCache>();

// Decorator wiring: the concrete NoteRepository is registered on its own, then wrapped.
// Only CachedNoteRepository is reachable as INoteRepository, so no caller can accidentally
// bypass the cache — or the invalidation that comes with it.
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<INoteRepository>(sp => new CachedNoteRepository(
    sp.GetRequiredService<NoteRepository>(),
    sp.GetRequiredService<ICache>(),
    sp.GetRequiredService<IOptions<CacheSettings>>(),
    sp.GetRequiredService<ILogger<CachedNoteRepository>>()));

builder.Host.UseSerilog((context, config) => config
    .WriteTo.Console()
    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]!)
);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapNoteEndpoints();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.CanConnect())
    {
        Log.Warning("Connected to Database");
        db.Database.Migrate();
    }
    else
    {
        Log.Information("Could not connect to Database");
    }
}

app.Run();

// Top-level statements compile to an internal Program class, which WebApplicationFactory<Program>
// in the test project cannot see. Re-declaring it as public partial makes it visible.
public partial class Program { }
