namespace ThrottleApi.Endpoints;

/// <summary>
/// Something cheap to hammer with a load generator. The instance name is in the response on
/// purpose: with two replicas behind the gateway it shows which one served you, which is how M3's
/// "the limit is really 2x" bug becomes visible instead of theoretical.
/// </summary>
public static class DemoEndpoints
{
    public static IEndpointRouteBuilder MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/ping", () => Results.Ok(new
        {
            instance = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? Environment.MachineName,
            timestamp = DateTimeOffset.UtcNow
        }));

        return app;
    }
}
