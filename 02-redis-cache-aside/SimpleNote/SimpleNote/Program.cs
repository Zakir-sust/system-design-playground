using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SimpleNote; 
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Host.UseSerilog((context, config) => config
    .WriteTo.Console()
    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]!)
);


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.MapGet("/", () => "Hello World!");

app.MapGet("/notes", (AppDbContext db) =>
{
    Log.Information("Notes count: {Count}", db.Notes.Count());
    return db.Notes.ToList();
});

// One place defines the cache key, so read and invalidation can never drift apart.
 string NoteKey(Guid id) => $"note:{id}";

app.MapGet("/notes/{id:guid}", async (Guid id, AppDbContext db, IConnectionMultiplexer redis) =>
{
    var redisDb = redis.GetDatabase();
    var cachedNote = await redisDb.StringGetAsync(NoteKey(id));

    if (cachedNote.HasValue)
    {
        Log.Information("Cache HIT: note {Id}", id);
        return Results.Ok(JsonSerializer.Deserialize<Note>(cachedNote!));
    }

    Log.Information("Cache MISS: note {Id}", id);
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();

    await redisDb.StringSetAsync(NoteKey(id), JsonSerializer.Serialize(note));
    return Results.Ok(note);
});

app.MapPut("/notes/{id:guid}", async (Guid id, string content, AppDbContext db, IConnectionMultiplexer redis) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();

    note.Content = content;
    await db.SaveChangesAsync();

    // Invalidate *after* the write commits. Deleting first would let a concurrent
    // reader re-populate the cache with the old row before the new one lands.
    var evicted = await redis.GetDatabase().KeyDeleteAsync(NoteKey(id));
    Log.Information("Updated note {Id}, cache evicted: {Evicted}", id, evicted);

    return Results.NoContent();
});

app.MapDelete("/notes/{id:guid}", async (Guid id, AppDbContext db, IConnectionMultiplexer redis) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note is null) return Results.NotFound();

    db.Notes.Remove(note);
    await db.SaveChangesAsync();

    var evicted = await redis.GetDatabase().KeyDeleteAsync(NoteKey(id));
    Log.Information("Deleted note {Id}, cache evicted: {Evicted}", id, evicted);

    return Results.NoContent();
});

app.MapPost("/notes", async (AppDbContext db, string note) =>
{
    Log.Information("New note: {Note}", note);
    var noteModel = new Note
    {
        Id = Guid.NewGuid(),
        Content = note,
    };
    db.Notes.Add(noteModel);
    await db.SaveChangesAsync();
});

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