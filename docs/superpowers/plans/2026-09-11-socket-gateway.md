# Gunny Socket Gateway Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a restricted .NET 8 WebSocket-to-TCP bridge for iOS Ruffle Web clients, allowing only configured Gunny game destinations.

**Architecture:** Kestrel accepts WebSocket connections on `/socket`, validates a configured route key, opens only the corresponding allowlisted TCP destination, and pumps bounded binary frames bidirectionally. Policy and bridge logic remain separately testable from the host.

**Tech Stack:** C# 12, ASP.NET Core .NET 8, `WebSocket`, `TcpClient`, existing SDK tooling without third-party runtime packages.

**Spec:** `docs/superpowers/specs/2026-09-11-mobile-launcher-design.md`

## Global Constraints

- Never accept arbitrary destination host/port from the client.
- Initial route maps to the configured game host on TCP `9200`.
- Enforce bounded message size, idle timeout, connection lifetime, and per-IP concurrency/rate limits.
- Proxy forwards raw bytes without parsing game payloads.
- Logs must not contain cookies, passwords, auth keys, CAPTCHA values, or payload bodies.

---

### Task 1: Destination Policy

**Files:**
- Create: `services/GunnySocketProxy/GunnySocketProxy.csproj`
- Create: `services/GunnySocketProxy/ProxyOptions.cs`
- Create: `services/GunnySocketProxy/DestinationPolicy.cs`
- Create: `services/GunnySocketProxy.Tests/GunnySocketProxy.Tests.csproj`
- Create: `services/GunnySocketProxy.Tests/Program.cs`

**Interfaces:**
- `ProxyOptions` defines `Routes`, `MaxFrameBytes`, `IdleTimeout`, `MaxConnectionLifetime`, and `MaxConnectionsPerClient`.
- `DestinationPolicy.TryResolve(string route, out ProxyDestination destination)` returns only configured endpoints.
- [ ] **Step 1: Write failing policy smoke**

```csharp
var options = ProxyOptions.CreateDefault("103.9.156.182", 9200);
var policy = new DestinationPolicy(options);
Require(policy.TryResolve("game", out var approved) && approved.Port == 9200, "approved route missing");
Require(!policy.TryResolve("103.9.156.182:22", out _), "arbitrary destination must be rejected");
```

- [ ] **Step 2: Run test and confirm RED**

Run: `dotnet run --project services/GunnySocketProxy.Tests/GunnySocketProxy.Tests.csproj -c Release`
Expected: compile failure because proxy policy types do not exist.

- [ ] **Step 3: Implement immutable destination mapping**

```csharp
public sealed record ProxyDestination(string Host, int Port);
public bool TryResolve(string route, out ProxyDestination destination) =>
    _routes.TryGetValue(route, out destination!);
```

Validate ports in `1..65535`, positive limits, and non-empty route names during options construction.

- [ ] **Step 4: Re-run policy smoke and commit**

Expected: `GUNNY_SOCKET_POLICY_SMOKE=PASS`.

```bash
git add services/GunnySocketProxy services/GunnySocketProxy.Tests
git commit -m "feat(proxy): add restricted destination policy"
```

### Task 2: Bounded WebSocket/TCP Bridge

**Files:**
- Create: `services/GunnySocketProxy/WebSocketTcpBridge.cs`
- Modify: `services/GunnySocketProxy.Tests/Program.cs`

**Interfaces:**
- `Task BridgeAsync(WebSocket socket, ProxyDestination destination, CancellationToken token)`.

- [ ] **Step 1: Add loopback TCP fixture and failing bridge smoke**

Start a `TcpListener` on loopback with an ephemeral port, map route `fixture` to it, send a binary WebSocket test frame through a fake/in-memory socket adapter, and assert the fixture receives the exact bytes and echoes them back unchanged.

- [ ] **Step 2: Confirm RED**

Expected: compile failure because `WebSocketTcpBridge` is absent.

- [ ] **Step 3: Implement two bounded pump loops**

Use `TcpClient.ConnectAsync(host, port, token)`, pooled buffers no larger than `MaxFrameBytes`, reject text frames, reject oversized fragmented messages, propagate cancellation, and close both sides on EOF/error. Do not log buffer contents.

- [ ] **Step 4: Run bridge smoke**

Expected: `GUNNY_SOCKET_BRIDGE_SMOKE=PASS` with byte-for-byte echo.

- [ ] **Step 5: Commit**

```bash
git add services/GunnySocketProxy services/GunnySocketProxy.Tests
git commit -m "feat(proxy): bridge bounded websocket traffic to tcp"
```

### Task 3: Host, Limits, and Health Endpoint

**Files:**
- Create: `services/GunnySocketProxy/Program.cs`
- Create: `services/GunnySocketProxy/ConnectionLimiter.cs`
- Create: `services/GunnySocketProxy/appsettings.json`
- Modify: `services/GunnySocketProxy.Tests/Program.cs`

**Interfaces:**
- `GET /healthz` returns `{ "status": "ok" }` only.
- `GET /socket?route=game` upgrades only when route and client limits pass.

- [ ] **Step 1: Add failing limiter assertions**

Create a limiter with max concurrent connections `2`; acquire twice for the same synthetic client key, assert third acquire is rejected, release one, assert another acquire succeeds.

- [ ] **Step 2: Implement limiter and host**

Use a `ConcurrentDictionary<string, ClientWindow>`, monotonic timestamps, and `try/finally` release. Add WebSockets middleware, reject non-WebSocket `/socket` requests with `400`, unknown routes with `403`, and limit breaches with `429`.

- [ ] **Step 3: Configure safe defaults**

`appsettings.json` contains route `game`, host `103.9.156.182`, port `9200`, `MaxFrameBytes=262144`, idle timeout `30s`, connection lifetime `30m`, concurrent connections per client `2`, and connection attempts per minute `10`. No secret values are stored.

- [ ] **Step 4: Verify host and tests**

Run:
`dotnet run --project services/GunnySocketProxy.Tests/GunnySocketProxy.Tests.csproj -c Release`
`dotnet build services/GunnySocketProxy/GunnySocketProxy.csproj -c Release`
Expected: policy, bridge, and limiter smoke markers PASS; build has zero errors.

- [ ] **Step 5: Commit**

```bash
git add services/GunnySocketProxy services/GunnySocketProxy.Tests
git commit -m "feat(proxy): add hardened websocket gateway host"
```

### Task 4: Gateway Regression Gate

- [ ] Run both proxy smoke and Release build.
- [ ] Run `git diff --check` and require no output.
- [ ] Confirm `rg -n "password|captcha|auth.?key|cookie" services/GunnySocketProxy` matches no logging statements or configuration secrets.
