# Gunny Cloudflare Domains + R2 Design

Date: 2026-09-12
Status: Approved design, implementation pending

## Goals

Move public game traffic away from raw `103.9.156.182` URLs and remove the dead legacy resource host dependency.

Use two public domains:

- `gunny.qs3d.site` for launcher/web/game/API traffic.
- `resource.qs3d.site` for immutable game resources.

Preserve the current VPS as the authoritative Gunny application origin and preserve the existing 14 Cloudflare asset shards as migration fallback.

## Current verified state

- VPS origin: `103.9.156.182`.
- `gunny.qs3d.site` and `resource.qs3d.site` currently do not resolve.
- Current game `SITE` is `http://res.gn.zing.vn/`, causing Ruffle `InvalidDomain` failures.
- Local resource repository contains the exact failing equipment assets.
- Local resource repository contains the canonical `sound/1006.flv`; no synthetic asset is required.
- `image/`: about 115,227 files / 3.18 GiB.
- `sound/`: 226 files / about 205 MiB.
- Existing 14 `ddtank-assets-shard-*` Workers use deterministic `SHA-256(path) -> bucket-map -> shard` routing.
- Existing shard builds cover image extensions only, so they cannot be the complete long-term resource origin.
- Resource repo already defines R2 bucket `ddtank-resource` and a `full-resource` profile.
- Local Cloudflare Wrangler authentication is currently expired; R2 S3 credentials are not present in environment variables.
- `subactivelist` source fix is merged, but production still serves the old `Tank.Request.dll` and returns HTTP 500.

## Architecture

### 1. Game/API edge: `gunny.qs3d.site`

A Cloudflare Worker acts as the public edge for the existing VPS.

It forwards game/web/API traffic to `103.9.156.182` while preserving method, query string, request body, and relevant headers. The Worker sets the origin `Host` header expected by IIS and adds no game semantics of its own.

Primary public paths include:

- `/Gunny/*`
- `/Request/*`
- launcher/login/config endpoints already served by the VPS

The Worker must support normal HTTP requests and must not cache dynamic `/Request/*` responses. Static VPS content may use conservative cache rules only after correctness is proven.

### 2. Resource edge: `resource.qs3d.site`

Cloudflare R2 bucket `ddtank-resource` is the primary immutable resource store.

The public object namespace mirrors the repository roots exactly:

- `/image/*`
- `/sound/*`
- `/flash/*`
- `/xml/*`
- `/partical/*`
- `/video/*`
- `/weekly/*`

`resource.qs3d.site` is bound to R2 through Cloudflare custom-domain support or a minimal read-only Worker in front of R2 if direct custom-domain binding is unavailable.

The 14 existing image shard Workers stay deployed during migration. They are fallback only; they are not exposed as the final client `SITE` because non-image resources are not present there.

No runtime request should depend on `res.gn.zing.vn` after cutover.

## Client configuration

After both domains pass public probes, `config.xml` is changed to:

- `FLASHSITE=https://gunny.qs3d.site/Gunny/flash/`
- `BACKUP_FLASHSITE=https://gunny.qs3d.site/Gunny/flash/`
- `SITE=https://resource.qs3d.site/`
- `FIRSTPAGE=https://gunny.qs3d.site/Gunny`
- `REGISTER=https://gunny.qs3d.site/Gunny`
- `REQUEST_PATH=https://gunny.qs3d.site/Request/`
- `LOGIN_PATH=https://gunny.qs3d.site`

Any remaining production config entries that still reference the raw VPS IP are reviewed individually. Only URLs that are confirmed to be game-owned and successfully proxied are migrated in this change.

## Migration sequence

1. Re-authenticate Cloudflare without committing tokens or credentials.
2. Validate R2 configuration and create/confirm `ddtank-resource`.
3. Plan the `full-resource` upload and record file/byte totals.
4. Upload with non-destructive `copy`; never use destructive sync during migration.
5. Verify remote objects/checksums.
6. Bind `resource.qs3d.site` and probe representative equipment assets plus `sound/1006.flv`.
7. Deploy `gunny.qs3d.site` proxy Worker and verify `/Gunny/config.xml`, `/Request/ServerList.aspx`, and representative login/request behavior.
8. Deploy the already-merged `subactivelist` handler/DLL to the VPS with a timestamped backup and automatic rollback on failed HTTP probe.
9. Patch VPS `config.xml` to the two new HTTPS domains.
10. Run a fresh Ruffle full-loading smoke and then a real-login smoke.

No client cutover occurs before steps 1-8 are green.

## Failure handling and rollback

Every production mutation is preceded by a backup or uses an additive Cloudflare deployment.

- R2 upload is additive and checksum-verified.
- Existing 14 shard Workers remain untouched until post-cutover verification is complete.
- VPS `Tank.Request.dll`, `subactivelist.ashx`, and `config.xml` receive timestamped backups before replacement.
- If `subactivelist` does not return HTTP 200 with a successful `<Result>`, restore the previous DLL/handler immediately.
- If either public domain fails probes, keep the client on the current working URL set.
- If Ruffle regresses after config cutover, restore the previous `config.xml` while retaining the new domains for diagnosis.

The raw VPS origin stays operational during migration so rollback does not depend on Cloudflare.

## Security and operations

- Never commit Cloudflare API tokens, R2 access keys, VPS passwords, login keys, or database secrets.
- Prefer Wrangler/OAuth login or scoped Cloudflare tokens supplied through the environment.
- Prefer SSH key authentication for VPS deployment once installed; do not persist plaintext passwords in scripts.
- `gunny.qs3d.site` must proxy only the intended Gunny origin and must not become an open proxy.
- Dynamic API responses are not edge-cached.
- Resource objects are read-only to public clients and may receive long immutable cache lifetimes after checksum verification.
- Cloudflare and VPS logs must not record game login keys beyond what existing application behavior requires.

## Verification gates

The migration is complete only when all of these are freshly verified:

1. `resource.qs3d.site` resolves publicly over HTTPS.
2. Representative equipment images return HTTP 200 with the expected content type.
3. `https://resource.qs3d.site/sound/1006.flv` returns HTTP 200 and matches the local source hash.
4. `gunny.qs3d.site` resolves publicly over HTTPS.
5. `/Gunny/config.xml` and `/Request/ServerList.aspx` work through `gunny.qs3d.site`.
6. `/Request/subactivelist.ashx` returns HTTP 200 with a success result.
7. Ruffle log contains no `subactivelist` HTTP 500 and no requests to `res.gn.zing.vn`.
8. Full `Loading.swf` reaches the expected post-loader state without the previous activity-interface popup.
9. A real login session reaches gameplay or the expected authenticated lobby without new blocking resource errors.

## Non-goals

- Rewriting Gunny server/gameplay logic.
- Deleting the 14 existing asset shard Workers during initial migration.
- Moving authoritative game state away from the VPS.
- Fabricating missing resources; only canonical repository assets are published.
- Enabling destructive R2 synchronization during migration.

## Implementation ownership

`BaseGunnyII` owns client/server URL configuration and VPS request-handler compatibility. `Resource` owns canonical static assets, R2 upload/verification, and any resource-domain Worker/configuration. A small Cloudflare gateway component owns `gunny.qs3d.site` proxy behavior and must stay independent of game business logic.
