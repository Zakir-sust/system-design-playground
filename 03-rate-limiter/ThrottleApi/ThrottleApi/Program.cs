using Serilog;
using StackExchange.Redis;
using ThrottleApi.Endpoints;
using ThrottleApi.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!);

    // Don't refuse to start just because Redis isn't up yet — M1 and M2 don't need it at all.
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddHealthChecks()
    .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>(), name: "redis");

builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection(RateLimitSettings.SectionName));


builder.Services.AddSingleton(TimeProvider.System);


builder.Services.AddSingleton<IRateLimiter, InMemoryFixedWindowRateLimiter>();

// Only wire up Seq when it is actually configured; passing null to WriteTo.Seq throws while the
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

// Position matters: this runs before routing, so a rejected request costs nothing but the limiter
// check. Rate limiting after authentication is a valid choice too — you get a real user id as the
// key, but you pay for the auth work on requests you are about to throw away.
app.UseMiddleware<RateLimitMiddleware>();

app.MapGet("/", () => "Hello World!");
app.MapDemoEndpoints();
app.MapHealthChecks("/health");

app.Run();

// Top-level statements compile to an internal Program class, which WebApplicationFactory<Program>
// in the test project cannot see. Re-declaring it as public partial makes it visible.
public partial class Program { }
