using GunnySocketProxy;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.GetSection("GunnyProxy");

var gameHost = configuration["GameHost"] ?? "103.9.156.181";
var gamePort = ParseInt(configuration["GamePort"], 9200);
var maxFrameBytes = ParseInt(configuration["MaxFrameBytes"], 262_144);
var idleSeconds = ParseInt(configuration["IdleTimeoutSeconds"], 30);
var lifetimeMinutes = ParseInt(configuration["MaxConnectionLifetimeMinutes"], 30);
var maxConcurrent = ParseInt(configuration["MaxConnectionsPerClient"], 2);
var maxAttempts = ParseInt(configuration["MaxConnectionAttemptsPerMinute"], 10);

var proxyOptions = new ProxyOptions
{
    Routes = new Dictionary<string, ProxyDestination>(StringComparer.Ordinal)
    {
        ["game"] = new ProxyDestination(gameHost, gamePort)
    },
    MaxFrameBytes = maxFrameBytes,
    IdleTimeout = TimeSpan.FromSeconds(idleSeconds),
    MaxConnectionLifetime = TimeSpan.FromMinutes(lifetimeMinutes),
    MaxConnectionsPerClient = maxConcurrent,
    MaxConnectionAttemptsPerMinute = maxAttempts
};

var policy = new DestinationPolicy(proxyOptions);
var limiter = new ConnectionLimiter(maxConcurrent, maxAttempts);
var bridge = new WebSocketTcpBridge(proxyOptions);
var app = builder.Build();
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(15)
});

app.MapGet("/healthz", () => Results.Json(new { status = "ok" }));

app.Map("/socket", async context =>
{
    var route = context.Request.Query["route"].ToString();
    if (!policy.TryResolve(route, out var destination))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return;
    }

    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    var clientKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    if (!limiter.TryAcquire(clientKey, out var lease))
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return;
    }
    using (lease)
    using (var socket = await context.WebSockets.AcceptWebSocketAsync())
    {
        try
        {
            await bridge.BridgeAsync(socket, destination, context.RequestAborted);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
        }
    }
});

app.Run();

static int ParseInt(string? value, int fallback)
{
    if (string.IsNullOrWhiteSpace(value)) return fallback;
    return int.TryParse(value, out var parsed)
        ? parsed
        : throw new InvalidOperationException($"Invalid integer configuration value: {value}");
}
