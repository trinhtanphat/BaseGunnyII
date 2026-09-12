namespace GunnySocketProxy;

public sealed class DestinationPolicy
{
    private readonly IReadOnlyDictionary<string, ProxyDestination> _routes;

    public DestinationPolicy(ProxyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        Validate(options);
        _routes = new Dictionary<string, ProxyDestination>(options.Routes, StringComparer.Ordinal);
    }

    public bool TryResolve(string route, out ProxyDestination destination)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            destination = default!;
            return false;
        }

        return _routes.TryGetValue(route, out destination!);
    }

    private static void Validate(ProxyOptions options)
    {
        if (options.Routes.Count == 0) throw new ArgumentException("At least one route is required.", nameof(options));
        if (options.MaxFrameBytes <= 0) throw new ArgumentOutOfRangeException(nameof(options.MaxFrameBytes));
        if (options.IdleTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(options.IdleTimeout));
        if (options.MaxConnectionLifetime <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(options.MaxConnectionLifetime));
        if (options.MaxConnectionsPerClient <= 0) throw new ArgumentOutOfRangeException(nameof(options.MaxConnectionsPerClient));
        if (options.MaxConnectionAttemptsPerMinute <= 0) throw new ArgumentOutOfRangeException(nameof(options.MaxConnectionAttemptsPerMinute));
        foreach (var pair in options.Routes)
        {
            if (string.IsNullOrWhiteSpace(pair.Key)) throw new ArgumentException("Route name is required.", nameof(options));
            if (string.IsNullOrWhiteSpace(pair.Value.Host)) throw new ArgumentException("Destination host is required.", nameof(options));
            if (pair.Value.Port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(options));
        }
    }
}
