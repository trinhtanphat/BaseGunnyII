using System.Net.Sockets;
using System.Net.WebSockets;

namespace GunnySocketProxy;

public sealed class WebSocketTcpBridge
{
    private readonly ProxyOptions _options;

    public WebSocketTcpBridge(ProxyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _ = new DestinationPolicy(options);
        _options = options;
    }

    public async Task BridgeAsync(
        WebSocket socket,
        ProxyDestination destination,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(socket);
        using var tcp = new TcpClient();
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        lifetime.CancelAfter(_options.MaxConnectionLifetime);
        using var idle = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
        RefreshIdle(idle);
        await tcp.ConnectAsync(destination.Host, destination.Port, idle.Token);
        await using var stream = tcp.GetStream();

        var upstream = PumpWebSocketToTcpAsync(socket, stream, tcp, idle, idle.Token);
        var downstream = PumpTcpToWebSocketAsync(stream, socket, idle, idle.Token);
        await Task.WhenAll(upstream, downstream);
    }
    private async Task PumpWebSocketToTcpAsync(
        WebSocket socket,
        NetworkStream stream,
        TcpClient tcp,
        CancellationTokenSource idle,
        CancellationToken token)
    {
        var buffer = new byte[_options.MaxFrameBytes];
        while (!token.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(buffer, token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                try { tcp.Client.Shutdown(SocketShutdown.Send); } catch (SocketException) { }
                return;
            }
            if (result.MessageType != WebSocketMessageType.Binary)
                throw new InvalidDataException("Only binary WebSocket frames are accepted.");
            if (!result.EndOfMessage)
                throw new InvalidDataException("Fragmented WebSocket messages are not accepted yet.");
            RefreshIdle(idle);
            await stream.WriteAsync(buffer.AsMemory(0, result.Count), token);
            await stream.FlushAsync(token);
        }
    }
    private async Task PumpTcpToWebSocketAsync(
        NetworkStream stream,
        WebSocket socket,
        CancellationTokenSource idle,
        CancellationToken token)
    {
        var buffer = new byte[_options.MaxFrameBytes];
        while (!token.IsCancellationRequested)
        {
            var count = await stream.ReadAsync(buffer, token);
            if (count == 0) break;
            RefreshIdle(idle);
            await socket.SendAsync(
                buffer.AsMemory(0, count),
                WebSocketMessageType.Binary,
                endOfMessage: true,
                cancellationToken: token);
        }

        if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "TCP closed", token);
    }

    private void RefreshIdle(CancellationTokenSource idle) => idle.CancelAfter(_options.IdleTimeout);
}
