using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleNote;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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
    Console.WriteLine($"notes count: {db.Notes.Count()}");
    return db.Notes.ToList();
});
app.MapPost("/notes", (AppDbContext db, string note) =>
{
    Console.WriteLine($"New note: {note}");
    var noteModel = new Note
    {
        Id = Guid.NewGuid(),
        Content = note,
    };
    db.Notes.Add(noteModel);
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.CanConnect())
    {
        Console.WriteLine("Connected to Database");
    }
    else
    {
        Console.WriteLine("Could not connect to Database");
    }
}
app.Run();