namespace GunnySocketProxy;

public sealed record ProxyDestination(string Host, int Port);

public sealed class ProxyOptions
{
    public required IReadOnlyDictionary<string, ProxyDestination> Routes { get; init; }
    public int MaxFrameBytes { get; init; } = 262_144;
    public TimeSpan IdleTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan MaxConnectionLifetime { get; init; } = TimeSpan.FromMinutes(30);
    public int MaxConnectionsPerClient { get; init; } = 2;
    public int MaxConnectionAttemptsPerMinute { get; init; } = 10;

    public static ProxyOptions CreateDefault(string host, int port) => new()
    {
        Routes = new Dictionary<string, ProxyDestination>(StringComparer.Ordinal)
        {
            ["game"] = new ProxyDestination(host, port)
        }
    };
}
