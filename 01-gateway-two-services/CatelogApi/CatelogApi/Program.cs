var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient("orders", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["OrdersBaseUrl"]);
});
builder.Services.AddHealthChecks();
// builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

app.MapGet("/ping", () => "Catelog API");

app.MapGet("/with-orders", async (IHttpClientFactory factory) =>
{
    Console.WriteLine("With orders endpoint");
    var client = factory.CreateClient("orders");
    var res =  await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/ping"));
    return await res.Content.ReadAsStringAsync();
});

app.Run();
