import test from 'node:test';
import assert from 'node:assert/strict';
import { createWorker } from '../src/worker-core.mjs';

class FakeWebSocket {
  constructor() {
    this.handlers = new Map();
    this.sent = [];
    this.closeArgs = null;
    this.accepted = false;
  }
  accept() { this.accepted = true; }
  addEventListener(name, handler) {
    const handlers = this.handlers.get(name) ?? [];
    handlers.push(handler);
    this.handlers.set(name, handlers);
  }
  async emit(name, event = {}) {
    await Promise.all((this.handlers.get(name) ?? []).map((handler) => handler(event)));
  }
  send(data) { this.sent.push(data); }
  close(code, reason) { this.closeArgs = [code, reason]; }
}

function makeHarness(readChunks = []) {
  const client = new FakeWebSocket();
  const server = new FakeWebSocket();
  const tcpWrites = [];
  const connections = [];
  const pending = [];
  const tcpSocket = {
    readable: new ReadableStream({
      start(controller) {
        for (const chunk of readChunks) controller.enqueue(chunk);
        controller.close();
      },
    }),
    writable: new WritableStream({
      write(chunk) { tcpWrites.push(new Uint8Array(chunk)); },
    }),
    close() {},
  };
  const ctx = { waitUntil(promise) { pending.push(promise); } };
  const worker = createWorker({
    connectTcp(destination) {
      connections.push(destination);
      return tcpSocket;
    },
    createWebSocketPair() { return { client, server }; },
    switchingProtocols(webSocket) { return { status: 101, webSocket }; },
  });
  return { worker, client, server, tcpWrites, connections, pending, ctx };
}

test('healthz does not open a TCP connection', async () => {
  const h = makeHarness();
  const response = await h.worker.fetch(new Request('https://edge.test/healthz'), {}, h.ctx);
  assert.equal(response.status, 200);
  assert.equal(h.connections.length, 0);
});
test('game websocket connects only to the fixed target and forwards binary bytes', async () => {
  const h = makeHarness();
  const request = new Request('https://edge.test/socket?route=game', {
    headers: { Upgrade: 'websocket' },
  });
  const response = await h.worker.fetch(request, {}, h.ctx);
  assert.equal(response.status, 101);
  assert.equal(response.webSocket, h.client);
  assert.equal(h.server.accepted, true);
  assert.deepEqual(h.connections, [{ hostname: '103.9.156.181', port: 9200 }]);

  await h.server.emit('message', { data: new Uint8Array([7, 8, 9]) });
  assert.deepEqual([...h.tcpWrites[0]], [7, 8, 9]);
});

test('text and oversized websocket messages are closed without TCP writes', async () => {
  const h = makeHarness();
  await h.worker.fetch(new Request('https://edge.test/socket?route=game', {
    headers: { Upgrade: 'websocket' },
  }), {}, h.ctx);

  await h.server.emit('message', { data: 'not-binary' });
  assert.equal(h.server.closeArgs[0], 1003);
  assert.equal(h.tcpWrites.length, 0);
});
test('oversized websocket messages close with 1009 before TCP write', async () => {
  const h = makeHarness();
  await h.worker.fetch(new Request('https://edge.test/socket?route=game', {
    headers: { Upgrade: 'websocket' },
  }), {}, h.ctx);

  await h.server.emit('message', { data: new Uint8Array(262145) });
  assert.equal(h.server.closeArgs[0], 1009);
  assert.equal(h.tcpWrites.length, 0);
});

test('TCP bytes are forwarded back to the websocket', async () => {
  const h = makeHarness([new Uint8Array([4, 5, 6])]);
  await h.worker.fetch(new Request('https://edge.test/socket?route=game', {
    headers: { Upgrade: 'websocket' },
  }), {}, h.ctx);

  await Promise.all(h.pending);
  assert.equal(h.server.sent.length, 1);
  assert.deepEqual([...h.server.sent[0]], [4, 5, 6]);
});
