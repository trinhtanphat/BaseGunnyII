import {
  resolveDestination,
  validateBinaryFrame,
  validateUpgradeRequest,
} from './policy.mjs';

function responseForStatus(status) {
  const messages = {
    403: 'Forbidden route',
    404: 'Not found',
    405: 'Method not allowed',
    426: 'WebSocket upgrade required',
  };
  return new Response(messages[status] ?? 'Rejected', { status });
}

function asBytes(data) {
  if (data instanceof ArrayBuffer) return new Uint8Array(data);
  if (ArrayBuffer.isView(data)) {
    return new Uint8Array(data.buffer, data.byteOffset, data.byteLength);
  }
  return null;
}

function safeCloseSocket(socket) {
  try { socket.close(); } catch { }
}
async function pumpTcpToWebSocket(webSocket, socket) {
  const reader = socket.readable.getReader();
  try {
    while (true) {
      const { value, done } = await reader.read();
      if (done) break;
      if (value?.byteLength) webSocket.send(value);
    }
    webSocket.close(1000, 'TCP closed');
  } catch {
    webSocket.close(1011, 'TCP relay failed');
  } finally {
    try { reader.releaseLock(); } catch { }
    safeCloseSocket(socket);
  }
}

function bindWebSocketToTcp(webSocket, socket) {
  const writer = socket.writable.getWriter();
  webSocket.addEventListener('message', async (event) => {
    const closeCode = validateBinaryFrame(event.data);
    if (closeCode !== null) {
      webSocket.close(closeCode, 'Rejected frame');
      safeCloseSocket(socket);
      return;
    }
    await writer.write(asBytes(event.data));
  });

  webSocket.addEventListener('close', () => {
    try { writer.releaseLock(); } catch { }
    safeCloseSocket(socket);
  });
  webSocket.addEventListener('error', () => {
    safeCloseSocket(socket);
  });
}

export function createWorker(deps) {
  return {
    async fetch(request, _env, ctx) {
      const url = new URL(request.url);
      if (url.pathname === '/healthz') {
        return new Response(JSON.stringify({ status: 'ok' }), {
          headers: { 'content-type': 'application/json' },
        });
      }

      const rejectStatus = validateUpgradeRequest(request);
      if (rejectStatus !== null) return responseForStatus(rejectStatus);

      const destination = resolveDestination(url.searchParams.get('route'));
      const socket = deps.connectTcp(destination);
      const { client, server } = deps.createWebSocketPair();
      server.accept();
      bindWebSocketToTcp(server, socket);
      ctx.waitUntil(pumpTcpToWebSocket(server, socket));
      return deps.switchingProtocols(client);
    },
  };
}
