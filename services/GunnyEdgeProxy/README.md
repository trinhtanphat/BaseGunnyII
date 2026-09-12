# Gunny Edge Proxy

Cloudflare Worker that exposes the legacy Gunny TCP game socket to Ruffle as a secure WebSocket endpoint.

## Fixed security boundary

- Public endpoint: `GET /socket?route=game` with a WebSocket upgrade.
- The only upstream destination is `103.9.156.182:9200`.
- Callers cannot provide an arbitrary host or port.
- Text WebSocket messages are rejected with close code `1003`.
- Binary messages larger than 256 KiB are rejected with close code `1009`.
- `GET /healthz` performs no upstream TCP connection.
- Payload bytes and credentials are never logged by this Worker.

The compatibility date is intentionally `2026-09-12`, which includes Cloudflare's current WebSocket close-frame behavior.

## Verify locally

```sh
npm ci
npm test
npm run dry-run
```

`npm run dry-run` must emit `.wrangler-dryrun/worker.js`. Local `wrangler dev` is optional; on the current Windows development machine Workerd startup has been intermittently very slow, so CI uses the deterministic unit/bundle gates on Ubuntu.

## Deploy

Authenticate Wrangler with the intended Cloudflare account, then run:

```sh
npm ci
npm run whoami
npm run deploy
```

After deployment, verify `/healthz`, then configure iOS build setting `GUNNY_SOCKET_PROXY_URL` as `wss://<deployed-worker-host>/socket?route=game`. Do not point it at a plaintext `ws://` URL.

### GitHub deployment

`.github/workflows/deploy-edge-gateway.yml` is manual-only. Configure repository secrets `CLOUDFLARE_API_TOKEN` and `CLOUDFLARE_ACCOUNT_ID`, then dispatch **Deploy Gunny Edge Gateway**. The workflow reruns unit tests and the Wrangler dry-run before it can deploy.

The CI workflow for pull requests does not require Cloudflare credentials.
