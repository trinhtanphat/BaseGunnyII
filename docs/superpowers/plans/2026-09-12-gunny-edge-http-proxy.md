# gunny.qs3d.site Edge Proxy Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extend the existing Gunny Edge Proxy so `https://gunny.qs3d.site` safely proxies Gunny web/API traffic to the VPS while retaining the existing WebSocket-to-TCP gateway.

**Architecture:** Reuse `services/GunnyEdgeProxy`. HTTP proxying is allowlisted to `/Gunny`, `/Gunny/*`, `/Request`, and `/Request/*`; upstream is fixed to `http://103.9.156.182`; redirects that point back to the raw origin are rewritten to `https://gunny.qs3d.site`; unknown paths remain closed.

**Tech Stack:** Cloudflare Workers, Node.js `node:test`, Wrangler 4.131.1.

**Spec:** `docs/superpowers/specs/2026-09-12-gunny-cloudflare-domains-r2-design.md`

## Global Constraints

- Preserve `GET /socket?route=game` behavior and fixed TCP target `103.9.156.182:9200`.
- Never accept an arbitrary HTTP origin, hostname, or port from the caller.
- Do not cache dynamic `/Request/*` responses.
- Do not log credentials, request bodies, login keys, or game payload bytes.
- Public custom domain is exactly `gunny.qs3d.site`.
- Raw VPS redirects must not leak `103.9.156.182` back to the client.

---

### Task 1: Add failing HTTP proxy policy tests

**Files:**
- Modify: `services/GunnyEdgeProxy/test/policy.test.mjs`
- Modify: `services/GunnyEdgeProxy/test/worker.test.mjs`
**Interfaces:**
- Consumes: existing WebSocket policy functions.
- Produces: `isAllowedHttpPath(pathname)` and HTTP proxy behavior through injected `fetchHttp`.

- [ ] **Step 1: Add RED policy assertions**

```js
assert.equal(isAllowedHttpPath('/Gunny'), true);
assert.equal(isAllowedHttpPath('/Gunny/config.xml'), true);
assert.equal(isAllowedHttpPath('/Request'), true);
assert.equal(isAllowedHttpPath('/Request/ServerList.aspx'), true);
assert.equal(isAllowedHttpPath('/socket'), false);
assert.equal(isAllowedHttpPath('/anything-else'), false);
```

- [ ] **Step 2: Add RED worker tests**

Cover all of these behaviors with a fake `fetchHttp` dependency:

```text
GET /Gunny/config.xml -> upstream http://103.9.156.182/Gunny/config.xml
POST /Request/example.ashx?a=1 -> method/query/body preserved
302 Location: http://103.9.156.182/Gunny/Default.aspx -> rewritten to https://gunny.qs3d.site/Gunny/Default.aspx
GET /unknown -> 404 without an upstream fetch
GET /socket?route=game Upgrade:websocket -> existing TCP path unchanged
```

- [ ] **Step 3: Run RED**

Run: `cd /d E:\Gunny\BaseGunnyII\services\GunnyEdgeProxy && npm test`
Expected: new HTTP proxy tests FAIL because the policy/helper does not exist yet.
### Task 2: Implement the fixed-origin HTTP proxy

**Files:**
- Modify: `services/GunnyEdgeProxy/src/policy.mjs`
- Modify: `services/GunnyEdgeProxy/src/worker-core.mjs`
- Modify: `services/GunnyEdgeProxy/src/worker.mjs`

- [ ] **Step 1: Implement the allowlist in `policy.mjs`**

```js
export function isAllowedHttpPath(pathname) {
  return pathname === '/Gunny'
    || pathname.startsWith('/Gunny/')
    || pathname === '/Request'
    || pathname.startsWith('/Request/');
}
```

- [ ] **Step 2: Implement fixed-origin request construction**

Use only the request path/query supplied by the client. Construct upstream URLs against the constant origin `http://103.9.156.182`; never read origin host/port from query parameters or headers. Preserve method, body, and ordinary headers; strip Cloudflare hop-by-hop headers.

- [ ] **Step 3: Rewrite origin redirects**

When an upstream `Location` begins with `http://103.9.156.182/`, rewrite only that prefix to `https://gunny.qs3d.site/`. Leave unrelated external redirects unchanged.

- [ ] **Step 4: Inject the real fetch dependency**

`worker.mjs` must pass `fetchHttp: fetch` into `createWorker`; unit tests continue to supply a fake fetch function.

- [ ] **Step 5: Run GREEN**

Run: `npm test`
Expected: all old WebSocket tests plus new HTTP tests PASS.

- [ ] **Step 6: Commit**

```bash
git add services/GunnyEdgeProxy/src services/GunnyEdgeProxy/test
git commit -m "feat(edge): proxy Gunny HTTP traffic"
```
### Task 3: Bind the custom domain and verify the bundle

**Files:**
- Modify: `services/GunnyEdgeProxy/wrangler.jsonc`
- Modify: `services/GunnyEdgeProxy/README.md`

- [ ] **Step 1: Add the custom-domain route**

Add:

```json
"routes": [
  { "pattern": "gunny.qs3d.site", "custom_domain": true }
]
```

Keep `workers_dev: true` during migration so the workers.dev endpoint remains a fallback probe target.

- [ ] **Step 2: Update README deployment contract**

Document `https://gunny.qs3d.site`, the HTTP allowlist, redirect rewriting, and the fact that `/Request/*` is not cached by the Worker.

- [ ] **Step 3: Run bundle gate**

```powershell
cd E:\Gunny\BaseGunnyII\services\GunnyEdgeProxy
npm install --no-audit --no-fund --package-lock=false
npm test
npm run dry-run
```

Expected: tests PASS and `.wrangler-dryrun/worker.js` is produced.

- [ ] **Step 4: Commit**

```bash
git add services/GunnyEdgeProxy/wrangler.jsonc services/GunnyEdgeProxy/README.md
git commit -m "ops(edge): bind gunny qs3d domain"
```

### Task 4: Deploy and verify `gunny.qs3d.site`

- [ ] **Step 1: Authenticate Cloudflare without persisting a plaintext token in the repo**

Run `npx wrangler login` interactively or provide a scoped `CLOUDFLARE_API_TOKEN` in the process environment, then run `npm run whoami`.

- [ ] **Step 2: Deploy**

Run: `npm run deploy`
Expected: Worker deploy succeeds and the custom domain certificate is provisioned.
- [ ] **Step 3: Public HTTP probes**

Run:

```powershell
$base='https://gunny.qs3d.site'
(Invoke-WebRequest -UseBasicParsing "$base/healthz").StatusCode
(Invoke-WebRequest -UseBasicParsing "$base/Gunny/config.xml").StatusCode
(Invoke-WebRequest -UseBasicParsing "$base/Request/ServerList.aspx").StatusCode
```

Expected: all three return HTTP 200.

- [ ] **Step 4: Security probes**

Verify `https://gunny.qs3d.site/not-allowed` returns 404 and does not reach the VPS. Verify a POST to a known `/Request/*` smoke endpoint preserves method/body and returns the same semantic result as direct-origin access.

- [ ] **Step 5: Redirect leak gate**

Exercise the login redirect path and inspect `Location`; any redirect back to the Gunny origin must begin `https://gunny.qs3d.site/`, not `http://103.9.156.182/`.

- [ ] **Step 6: Final fresh verification**

Run `npm test`, `npm run dry-run`, and the public probes again. Do not modify VPS `config.xml` until every gate in this plan is green.
