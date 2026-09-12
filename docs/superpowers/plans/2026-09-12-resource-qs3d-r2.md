# resource.qs3d.site R2 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Publish the canonical Gunny resource tree through `https://resource.qs3d.site/` using R2 without deleting or fabricating assets.

**Architecture:** Reuse `E:\Gunny\Resource` and its existing `r2-deploy.mjs` full-resource profile. Bulk upload uses rclone/S3 credentials; R2 receives a Cloudflare custom domain; the 14 image shard Workers stay deployed as fallback during migration.

**Tech Stack:** Node.js 20+, Cloudflare Wrangler 4.x, Cloudflare R2, rclone, PowerShell.

**Spec:** `docs/superpowers/specs/2026-09-12-gunny-cloudflare-domains-r2-design.md`

## Global Constraints

- Bucket remains `ddtank-resource` in account `291f5d12e63427644f59ac4a1d8f9664`.
- Public domain is exactly `resource.qs3d.site`.
- Publish only canonical roots: `flash`, `image`, `partical`, `sound`, `video`, `weekly`, `xml`.
- Use non-destructive `copy`; never run R2 `sync` during this migration.
- Never commit Cloudflare tokens, R2 access keys, or generated temporary rclone config.
- Existing 14 `ddtank-assets-shard-*` Workers remain untouched until post-cutover verification.

---

### Task 1: Add a deterministic public-resource probe

**Files:**
- Create: `E:\Gunny\Resource\scripts\probe-resource-domain.mjs`
- Create: `E:\Gunny\Resource\test\probe-resource-domain.test.mjs`
- Modify: `E:\Gunny\Resource\package.json`

**Interfaces:**
- Produces: `buildProbeUrls(baseUrl)` and CLI exit code 0 only when every representative asset is HTTP 200.
- [ ] **Step 1: Write the failing Node test**

```js
import test from 'node:test';
import assert from 'node:assert/strict';
import { buildProbeUrls } from '../scripts/probe-resource-domain.mjs';

test('resource domain probes canonical image and sound paths', () => {
  assert.deepEqual(buildProbeUrls('https://resource.qs3d.site/'), [
    'https://resource.qs3d.site/image/equip/f/head/default/2/show.png',
    'https://resource.qs3d.site/image/equip/f/glass/default/2/show.png',
    'https://resource.qs3d.site/image/equip/f/cloth/cloth76/1/show.png',
    'https://resource.qs3d.site/image/equip/f/suits/default/1/show.png',
    'https://resource.qs3d.site/image/equip/f/eff/default/1/show.png',
    'https://resource.qs3d.site/sound/1006.flv',
  ]);
});
```

- [ ] **Step 2: Run RED**

Run: `cd /d E:\Gunny\Resource && node --test test/probe-resource-domain.test.mjs`

Expected: FAIL because `probe-resource-domain.mjs` does not exist.

- [ ] **Step 3: Implement the minimal probe module**

The CLI must normalize one trailing slash, HEAD all six URLs, require status 200, and print `RESOURCE_DOMAIN_GATE=PASS` only after all six pass. Export `buildProbeUrls` so tests do not perform network I/O.
- [ ] **Step 4: Add npm scripts and verify GREEN**

Add to `package.json`:

```json
"test": "node --test test/*.test.mjs",
"probe:resource-domain": "node scripts/probe-resource-domain.mjs"
```

Run: `npm test`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add package.json scripts/probe-resource-domain.mjs test/probe-resource-domain.test.mjs
git commit -m "test(resource): add public domain probes"
```

### Task 2: Validate and upload the full canonical resource profile

**Files:**
- Existing: `E:\Gunny\Resource\r2-deployment.json`
- Existing: `E:\Gunny\Resource\scripts\r2-deploy.mjs`
- Generated/ignored: `E:\Gunny\Resource\.r2\r2-report.json`

- [ ] **Step 1: Verify tooling and credentials without printing secrets**

```powershell
if (-not (Get-Command rclone -ErrorAction SilentlyContinue)) { throw 'rclone missing' }
if (-not $env:R2_ACCESS_KEY_ID) { throw 'R2_ACCESS_KEY_ID missing' }
if (-not $env:R2_SECRET_ACCESS_KEY) { throw 'R2_SECRET_ACCESS_KEY missing' }
if (-not $env:CLOUDFLARE_API_TOKEN) { throw 'CLOUDFLARE_API_TOKEN missing' }
```

- [ ] **Step 2: Validate configuration and produce the full-resource plan**

Run:

```powershell
cd E:\Gunny\Resource
$env:R2_PROFILE='full-resource'
npm run r2:validate
npm run r2:plan
```

Expected: plan includes all seven roots and no missing-root error.
- [ ] **Step 3: Confirm/create the bucket and upload non-destructively**

```powershell
npm run r2:bucket:list
npm run r2:deploy
```

If `ddtank-resource` is absent, run `npm run r2:bucket:create` once, then rerun `npm run r2:deploy`.

Expected: rclone copy completes for every configured root and `r2:verify` reports status `ok`; no delete/sync command is executed.

### Task 3: Bind `resource.qs3d.site` and verify public objects

**Files:**
- Modify after GREEN: `E:\Gunny\Resource\r2-deployment.json`

- [ ] **Step 1: Resolve the Cloudflare zone ID without hard-coding it**

```powershell
$headers = @{ Authorization = "Bearer $env:CLOUDFLARE_API_TOKEN" }
$zone = Invoke-RestMethod -Headers $headers -Uri 'https://api.cloudflare.com/client/v4/zones?name=qs3d.site'
if (-not $zone.success -or $zone.result.Count -ne 1) { throw 'qs3d.site zone lookup failed' }
$zoneId = $zone.result[0].id
```

- [ ] **Step 2: Add the R2 custom domain**

```powershell
cd E:\Gunny\Resource
npx wrangler r2 bucket domain add ddtank-resource --domain resource.qs3d.site --zone-id $zoneId --min-tls 1.2
npx wrangler r2 bucket domain get ddtank-resource --domain resource.qs3d.site
```

Expected: domain status becomes active/connected.

- [ ] **Step 3: Run the public RED-to-GREEN gate**

```powershell
$env:R2_PUBLIC_BASE_URL='https://resource.qs3d.site'
$env:R2_PROFILE='full-resource'
npm run r2:probe
npm run probe:resource-domain
```

Expected: representative R2 probe PASS and `RESOURCE_DOMAIN_GATE=PASS`.

- [ ] **Step 4: Persist the verified public base URL only after the gate is green**

Change `r2-deployment.json` `publicBaseUrl` from `null` to `https://resource.qs3d.site`, rerun `npm run r2:validate`, then commit:

```bash
git add r2-deployment.json
git commit -m "ops(resource): bind qs3d resource domain"
```

### Task 4: Final resource acceptance

- [ ] Run `npm test`, `npm run r2:verify`, and `npm run probe:resource-domain` fresh.
- [ ] Confirm `https://resource.qs3d.site/sound/1006.flv` has the same SHA-256 as `E:\Gunny\Resource\sound\1006.flv` by downloading to a temporary file and comparing `Get-FileHash` outputs.
- [ ] Confirm none of the 14 `ddtank-assets-shard-*` deployments were removed.
