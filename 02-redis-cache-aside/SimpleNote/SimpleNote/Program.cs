using Microsoft.EntityFrameworkCore;
using SimpleNote; 
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

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

app.MapGet("/notes/{id}", async (string id, AppDbContext db) =>
{
    Log.Information("Get note #{id}");
    var note = await db.Notes.FirstOrDefaultAsync(x => x.Id.ToString() == id);
    return note;
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