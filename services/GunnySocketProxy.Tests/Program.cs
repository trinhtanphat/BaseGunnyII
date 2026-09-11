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
Require(!policy.TryResolve("game?port=22", out _), "route input must not carry arbitrary destination data");
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