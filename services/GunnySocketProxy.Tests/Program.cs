using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
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
var idleListener = new TcpListener(IPAddress.Loopback, 0);
idleListener.Start();
var idlePort = ((IPEndPoint)idleListener.LocalEndpoint).Port;
var idleServerTask = Task.Run(async () =>
{
    using var tcp = await idleListener.AcceptTcpClientAsync();
    var oneByte = new byte[1];
    try { await tcp.GetStream().ReadAsync(oneByte); } catch (IOException) { }
});
var idleOptions = new ProxyOptions
{
    Routes = new Dictionary<string, ProxyDestination> { ["fixture"] = new("127.0.0.1", idlePort) },
    MaxFrameBytes = 1024,
    IdleTimeout = TimeSpan.FromMilliseconds(150),
    MaxConnectionLifetime = TimeSpan.FromSeconds(2),
    MaxConnectionsPerClient = 2,
    MaxConnectionAttemptsPerMinute = 10
};
var idleBridge = new WebSocketTcpBridge(idleOptions);
using var silentSocket = new SilentWebSocket();
var idleWatch = Stopwatch.StartNew();
try { await idleBridge.BridgeAsync(silentSocket, new ProxyDestination("127.0.0.1", idlePort), CancellationToken.None); }
catch (OperationCanceledException) { }
idleWatch.Stop();
idleListener.Stop();
await idleServerTask;
Require(idleWatch.Elapsed < TimeSpan.FromSeconds(1), "idle timeout was not enforced");
Console.WriteLine("GUNNY_SOCKET_IDLE_TIMEOUT_SMOKE=PASS");

Require(!policy.TryResolve("game?port=22", out _), "route input must not carry arbitrary destination data");
Require(options.MaxConnectionAttemptsPerMinute == 10, "default connection attempt limit mismatch");
RequireThrows<ArgumentOutOfRangeException>(() => new DestinationPolicy(ProxyOptions.CreateDefault("103.9.156.182", 0)), "invalid port must be rejected");
RequireThrows<ArgumentOutOfRangeException>(() => new DestinationPolicy(WithFrameLimit(options, 0)), "zero frame limit must be rejected");
Console.WriteLine("GUNNY_SOCKET_POLICY_SMOKE=PASS");
var listener = new TcpListener(IPAddress.Loopback, 0);
listener.Start();
var fixturePort = ((IPEndPoint)listener.LocalEndpoint).Port;
var expectedPayload = new byte[] { 0x47, 0x55, 0x4E, 0x4E, 0x59 };
byte[]? tcpReceived = null;
var fixtureTask = Task.Run(async () =>
{
    using var tcp = await listener.AcceptTcpClientAsync();
    await using var stream = tcp.GetStream();
    var buffer = new byte[64];
    var count = await stream.ReadAsync(buffer);
    tcpReceived = buffer[..count];
    await stream.WriteAsync(tcpReceived);
    await stream.FlushAsync();
    tcp.Client.Shutdown(SocketShutdown.Send);
});

var bridgeOptions = ProxyOptions.CreateDefault("127.0.0.1", fixturePort);
var bridge = new WebSocketTcpBridge(bridgeOptions);
using var scriptedSocket = new ScriptedWebSocket(expectedPayload);
using var bridgeCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await bridge.BridgeAsync(scriptedSocket, new ProxyDestination("127.0.0.1", fixturePort), bridgeCts.Token);
await fixtureTask;
listener.Stop();
Require(tcpReceived is not null && tcpReceived.SequenceEqual(expectedPayload), "TCP fixture payload mismatch");
Require(scriptedSocket.SentBytes.SequenceEqual(expectedPayload), "WebSocket echo payload mismatch");
Console.WriteLine("GUNNY_SOCKET_BRIDGE_SMOKE=PASS");

var fragmentListener = new TcpListener(IPAddress.Loopback, 0);
fragmentListener.Start();
var fragmentPort = ((IPEndPoint)fragmentListener.LocalEndpoint).Port;
byte[]? fragmentedTcpReceived = null;
var fragmentServerTask = Task.Run(async () =>
{
    using var tcp = await fragmentListener.AcceptTcpClientAsync();
    await using var stream = tcp.GetStream();
    var buffer = new byte[64];
    var count = await stream.ReadAsync(buffer);
    fragmentedTcpReceived = buffer[..count];
    tcp.Client.Shutdown(SocketShutdown.Send);
});
var fragmentBridge = new WebSocketTcpBridge(ProxyOptions.CreateDefault("127.0.0.1", fragmentPort));
using var fragmentedSocket = new FragmentedWebSocket(new byte[] { 0x47, 0x55, 0x4E }, new byte[] { 0x4E, 0x59 });
using var fragmentCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await fragmentBridge.BridgeAsync(fragmentedSocket, new ProxyDestination("127.0.0.1", fragmentPort), fragmentCts.Token);
await fragmentServerTask;
fragmentListener.Stop();
Require(fragmentedTcpReceived is not null && fragmentedTcpReceived.SequenceEqual(expectedPayload), "fragmented WebSocket message was not reassembled");
Console.WriteLine("GUNNY_SOCKET_FRAGMENT_SMOKE=PASS");

var oversizeListener = new TcpListener(IPAddress.Loopback, 0);
oversizeListener.Start();
var oversizePort = ((IPEndPoint)oversizeListener.LocalEndpoint).Port;
var oversizeServerTask = Task.Run(async () =>
{
    using var tcp = await oversizeListener.AcceptTcpClientAsync();
    await Task.Delay(100);
});
var oversizeOptions = WithFrameLimit(ProxyOptions.CreateDefault("127.0.0.1", oversizePort), 4);
var oversizeBridge = new WebSocketTcpBridge(oversizeOptions);
using var oversizeSocket = new FragmentedWebSocket(new byte[] { 1, 2, 3 }, new byte[] { 4, 5 });
var oversizeRejected = false;
try
{
    await oversizeBridge.BridgeAsync(oversizeSocket, new ProxyDestination("127.0.0.1", oversizePort), CancellationToken.None);
}
catch (InvalidDataException)
{
    oversizeRejected = true;
}
oversizeListener.Stop();
await oversizeServerTask;
Require(oversizeRejected, "fragmented message above MaxFrameBytes must be rejected");
Console.WriteLine("GUNNY_SOCKET_FRAGMENT_LIMIT_SMOKE=PASS");

var limiter = new ConnectionLimiter(maxConcurrent: 2, maxAttemptsPerMinute: 10);
Require(limiter.TryAcquire("client-a", out var lease1), "first lease rejected");
Require(limiter.TryAcquire("client-a", out var lease2), "second lease rejected");
Require(!limiter.TryAcquire("client-a", out _), "third concurrent lease must be rejected");
lease1.Dispose();
Require(limiter.TryAcquire("client-a", out var lease3), "lease should succeed after release");
lease2.Dispose();
lease3.Dispose();
Console.WriteLine("GUNNY_SOCKET_LIMITER_SMOKE=PASS");

var rateLimiter = new ConnectionLimiter(maxConcurrent: 5, maxAttemptsPerMinute: 2);
Require(rateLimiter.TryAcquire("client-rate", out var rate1), "rate attempt one rejected");
rate1.Dispose();
Require(rateLimiter.TryAcquire("client-rate", out var rate2), "rate attempt two rejected");
rate2.Dispose();
Require(!rateLimiter.TryAcquire("client-rate", out _), "third attempt inside rate window must be rejected");
Console.WriteLine("GUNNY_SOCKET_RATE_LIMIT_SMOKE=PASS");
static ProxyOptions WithFrameLimit(ProxyOptions source, int value) => new()
{
    Routes = source.Routes,
    MaxFrameBytes = value,
    IdleTimeout = source.IdleTimeout,
    MaxConnectionLifetime = source.MaxConnectionLifetime,
    MaxConnectionsPerClient = source.MaxConnectionsPerClient
};

sealed class ScriptedWebSocket : WebSocket
{
    private readonly byte[] _payload;
    private bool _received;
    private WebSocketState _state = WebSocketState.Open;
    private readonly List<byte> _sent = new();

    public ScriptedWebSocket(byte[] payload) => _payload = payload;
    public IReadOnlyList<byte> SentBytes => _sent;
    public override WebSocketCloseStatus? CloseStatus => null;
    public override string? CloseStatusDescription => null;
    public override WebSocketState State => _state;
    public override string? SubProtocol => null;
    public override void Abort() => _state = WebSocketState.Aborted;
    public override void Dispose() => _state = WebSocketState.Closed;

    public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    {
        _state = WebSocketState.Closed;
        return Task.CompletedTask;
    }
    public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    {
        _state = WebSocketState.CloseSent;
        return Task.CompletedTask;
    }

    public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        if (!_received)
        {
            _received = true;
            _payload.CopyTo(buffer.Array!, buffer.Offset);
            return Task.FromResult(new WebSocketReceiveResult(_payload.Length, WebSocketMessageType.Binary, true));
        }

        _state = WebSocketState.CloseReceived;
        return Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true));
    }

    public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
    {
        if (messageType != WebSocketMessageType.Binary) throw new InvalidOperationException("Bridge must send binary frames.");
        _sent.AddRange(buffer.AsSpan().ToArray());
        return Task.CompletedTask;
    }
}
sealed class FragmentedWebSocket : WebSocket
{
    private readonly byte[][] _fragments;
    private int _index;
    private WebSocketState _state = WebSocketState.Open;
    public FragmentedWebSocket(params byte[][] fragments) => _fragments = fragments;
    public override WebSocketCloseStatus? CloseStatus => null;
    public override string? CloseStatusDescription => null;
    public override WebSocketState State => _state;
    public override string? SubProtocol => null;
    public override void Abort() => _state = WebSocketState.Aborted;
    public override void Dispose() => _state = WebSocketState.Closed;
    public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    { _state = WebSocketState.Closed; return Task.CompletedTask; }
    public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    { _state = WebSocketState.CloseSent; return Task.CompletedTask; }
    public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        if (_index < _fragments.Length)
        {
            var fragment = _fragments[_index++];
            fragment.CopyTo(buffer.Array!, buffer.Offset);
            return Task.FromResult(new WebSocketReceiveResult(fragment.Length, WebSocketMessageType.Binary, _index == _fragments.Length));
        }
        _state = WebSocketState.CloseReceived;
        return Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true));
    }
    public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

sealed class SilentWebSocket : WebSocket
{
    private WebSocketState _state = WebSocketState.Open;
    public override WebSocketCloseStatus? CloseStatus => null;
    public override string? CloseStatusDescription => null;
    public override WebSocketState State => _state;
    public override string? SubProtocol => null;
    public override void Abort() => _state = WebSocketState.Aborted;
    public override void Dispose() => _state = WebSocketState.Closed;
    public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    { _state = WebSocketState.Closed; return Task.CompletedTask; }
    public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
    { _state = WebSocketState.CloseSent; return Task.CompletedTask; }
    public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        throw new InvalidOperationException("unreachable");
    }
    public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
