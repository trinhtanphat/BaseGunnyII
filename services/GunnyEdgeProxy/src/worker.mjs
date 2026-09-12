import { connect } from 'cloudflare:sockets';
import { createWorker } from './worker-core.mjs';

export default createWorker({
  connectTcp(destination) {
    return connect(destination);
  },
  createWebSocketPair() {
    const pair = new WebSocketPair();
    return { client: pair[0], server: pair[1] };
  },
  switchingProtocols(webSocket) {
    return new Response(null, { status: 101, webSocket });
  },
});
