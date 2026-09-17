export const MAX_FRAME_BYTES = 262144;

const DESTINATIONS = Object.freeze({
  game: Object.freeze({ hostname: '103.9.156.181', port: 9200 }),
});

export function resolveDestination(route) {
  return DESTINATIONS[route] ?? null;
}

export function validateUpgradeRequest(request) {
  const url = new URL(request.url);
  if (url.pathname !== '/socket') return 404;
  if (request.method !== 'GET') return 405;
  if (!resolveDestination(url.searchParams.get('route'))) return 403;
  if ((request.headers.get('Upgrade') ?? '').toLowerCase() !== 'websocket') return 426;
  return null;
}

export function validateBinaryFrame(data) {
  if (typeof data === 'string') return 1003;
  const size = data instanceof ArrayBuffer ? data.byteLength : data?.byteLength;
  if (!Number.isInteger(size)) return 1003;
  if (size > MAX_FRAME_BYTES) return 1009;
  return null;
}
