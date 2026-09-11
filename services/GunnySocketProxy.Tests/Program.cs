using GunnySocketProxy;

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static void RequireThrows<T>(Action action, string message) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new Exception(message);
}

var options = ProxyOptions.CreateDefault("103.9.156.182", 9200);
var policy = new DestinationPolicy(options);
Require(policy.TryResolve("game", out var approved), "approved route missing");
Require(approved.Host == "103.9.156.182", "approved host mismatch");
Require(approved.Port == 9200, "approved port mismatch");
Require(!policy.TryResolve("103.9.156.182:22", out _), "arbitrary destination must be rejected");
Require(!policy.TryResolve("game?port=22", out _), "route input must not carry arbitrary destination data");
RequireThrows<ArgumentOutOfRangeException>(() => new DestinationPolicy(ProxyOptions.CreateDefault("103.9.156.182", 0)), "invalid port must be rejected");
RequireThrows<ArgumentOutOfRangeException>(() => new DestinationPolicy(WithFrameLimit(options, 0)), "zero frame limit must be rejected");
Console.WriteLine("GUNNY_SOCKET_POLICY_SMOKE=PASS");

static ProxyOptions WithFrameLimit(ProxyOptions source, int value) => new()
{
    Routes = source.Routes,
    MaxFrameBytes = value,
    IdleTimeout = source.IdleTimeout,
    MaxConnectionLifetime = source.MaxConnectionLifetime,
    MaxConnectionsPerClient = source.MaxConnectionsPerClient
};
