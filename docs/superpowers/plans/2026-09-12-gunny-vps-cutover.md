# Gunny VPS Domain Cutover Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Deploy the merged `subactivelist` fix, switch launcher/config HTTP URLs to the new HTTPS domains, and prove the full Ruffle login/load path without losing rollback.

**Architecture:** Keep the VPS authoritative. Deploy request-handler binaries and config through SSH with timestamped backups, while launcher HTTP URLs move to `gunny.qs3d.site`; keep the raw TCP game socket target `103.9.156.182:9200` explicitly decoupled from the HTTPS game base.

**Tech Stack:** .NET/C#, PowerShell, OpenSSH, IIS/ASP.NET, Ruffle desktop.

**Spec:** `docs/superpowers/specs/2026-09-12-gunny-cloudflare-domains-r2-design.md`

## Global Constraints

- Do not cut over config before both Cloudflare domains pass their respective plans.
- Keep game TCP at `103.9.156.182:9200`; Cloudflare HTTP custom domains do not replace the legacy raw socket.
- Never log or commit passwords, session keys, login keys, or DB secrets.
- Back up `Tank.Request.dll`, `subactivelist.ashx`, and `Gunny/config.xml` before mutation.
- Roll back immediately if `subactivelist` is not HTTP 200 success after deploy.
- Use exact-PID targeting for Ruffle smoke processes; never broad-kill Ruffle.

---

### Task 1: Decouple launcher HTTPS base from the raw game socket

**Files:**
- Modify: `tools/GunnyLauncher/GunnyLauncher.Core/GunnyProtocolContract.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.Core/RuffleLaunchCommand.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.App/LauncherSettings.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.ContractExport/Program.cs`
- Modify: `tools/GunnyLauncher/GunnyLauncher.Tests/Program.cs`
- Modify: `tools/GunnyLauncher/README.md`
**Interfaces:**
- Produces: `GunnyProtocolContract.DefaultGameBaseUrl`, `GameSocketHost`, and `GameSocketPort`.

- [ ] **Step 1: Write the failing launcher assertions**

Change test fixtures to use `https://gunny.qs3d.site/Gunny/` and assert:

```csharp
Require(GunnyProtocolContract.DefaultGameBaseUrl == "https://gunny.qs3d.site/Gunny/", "default game base mismatch");
Require(GunnyProtocolContract.GameSocketHost == "103.9.156.182", "game socket host mismatch");
Require(GunnyProtocolContract.GameSocketPort == 9200, "game socket port mismatch");
Require(argLine.Contains("--base|https://gunny.qs3d.site/Gunny/flash/", StringComparison.OrdinalIgnoreCase), "HTTPS Ruffle base missing");
Require(argLine.Contains("--socket-allow|103.9.156.182:9200", StringComparison.Ordinal), "raw game socket allowlist missing");
```

- [ ] **Step 2: Run RED**

Run the existing launcher test project.
Expected: FAIL because the new constants do not exist and `RuffleLaunchCommand` still derives socket host from `gameBase.Host`.

- [ ] **Step 3: Implement minimal constants and socket decoupling**

Add to `GunnyProtocolContract`:

```csharp
public const string DefaultGameBaseUrl = "https://gunny.qs3d.site/Gunny/";
public const string GameSocketHost = "103.9.156.182";
public const int GameSocketPort = 9200;
```

Use those socket constants in `RuffleLaunchCommand`; use `DefaultGameBaseUrl` in launcher defaults and contract export.

- [ ] **Step 4: Run GREEN and commit**

Run launcher tests and contract export; verify generated contract has HTTPS game base but raw TCP socket host. Then commit `feat(launcher): use qs3d game domain`.
### Task 2: Add a tested config-domain patcher

**Files:**
- Create: `scripts/ops/Patch-GunnyPublicDomains.ps1`
- Create: `scripts/ops/Test-Patch-GunnyPublicDomains.ps1`

**Interfaces:**
- `Patch-GunnyPublicDomains.ps1 -Path` accepts the full path to a target `config.xml`, mutates only verified Gunny-owned URL nodes, and exits nonzero if expected nodes are missing.

- [ ] **Step 1: Write the failing patcher test**

The test copies `E:\Gunny\_deploy\web-766a81f\gunny\config.xml` to a temp path, invokes the patcher, then asserts exact values:

```text
FLASHSITE=https://gunny.qs3d.site/Gunny/flash/
BACKUP_FLASHSITE=https://gunny.qs3d.site/Gunny/flash/
SITE=https://resource.qs3d.site/
FIRSTPAGE=https://gunny.qs3d.site/Gunny
REGISTER=https://gunny.qs3d.site/Gunny
REQUEST_PATH=https://gunny.qs3d.site/Request/
LOGIN_PATH=https://gunny.qs3d.site
POLICY_FILES/file=https://gunny.qs3d.site/Gunny/crossdomain.xml
```

- [ ] **Step 2: Run RED**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File scripts\ops\Test-Patch-GunnyPublicDomains.ps1`
Expected: FAIL because the patcher does not exist.

- [ ] **Step 3: Implement the XML patcher**

Use `[xml]` parsing, require exactly one target for each node, update only the listed attributes, preserve UTF-8 XML output, and never touch `COUNT_PATH`, community URLs, or other third-party feature URLs in this task.

- [ ] **Step 4: Run GREEN and commit**

Rerun the patcher test; compare original vs temp output to ensure only intended URL nodes changed. Commit `ops(config): add qs3d domain patcher`.
### Task 3: Establish password-free SSH deployment and deploy `subactivelist`

**Files:**
- Existing local private key: `E:\Gunny\_ops\gunny_vps_rsa`
- Existing local public key: `E:\Gunny\_ops\gunny_vps_rsa.pub`
- Existing bootstrap: `E:\Gunny\_ops\Install-Gunny-VPS-Key.cmd`
- Source DLL: `Tank.Request\bin\Tank.Request.dll`
- Source handler: `Tank.Request\subactivelist.ashx`

- [ ] **Step 1: Verify SSH key authentication**

Run:

```powershell
ssh -i E:\Gunny\_ops\gunny_vps_rsa -o BatchMode=yes -o StrictHostKeyChecking=yes Administrator@103.9.156.182 "echo SSH_KEY_GATE=PASS"
```

If this fails because the public key is not installed, run the existing one-time key bootstrap interactively; do not embed the VPS password in scripts or command lines. Rerun until `SSH_KEY_GATE=PASS` is printed.

- [ ] **Step 2: Fresh-build and smoke the handler locally**

Run the established `Tank.Request` build and `Smoke-SubActiveList-Compiled.ps1`. Require build exit 0 and `SUBACTIVITY_COMPILED_GATE=PASS`.

- [ ] **Step 3: Backup and deploy over key-authenticated SSH**

Set `$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'` and `$backup = "C:\GunnyWeb\backups\subactivity-$stamp"`; create that directory, copy the current `request\bin\Tank.Request.dll` and handler there, upload the fresh DLL/ASHX, verify remote DLL SHA-256 equals local SHA-256, then wait for IIS app-domain reload.

- [ ] **Step 4: Production gate**

Probe both direct origin and edge:

```text
http://103.9.156.182/Request/subactivelist.ashx
https://gunny.qs3d.site/Request/subactivelist.ashx
```

Expected: HTTP 200 and XML containing `value="true"`. On any mismatch, restore the backup immediately and stop cutover.
### Task 4: Cut over VPS `config.xml` with rollback

- [ ] **Step 1: Pre-cutover public dependency gate**

Require all of the following before touching VPS config:

```text
https://resource.qs3d.site/image/equip/f/head/default/2/show.png -> 200
https://resource.qs3d.site/sound/1006.flv -> 200
https://gunny.qs3d.site/Gunny/config.xml -> 200
https://gunny.qs3d.site/Request/ServerList.aspx -> 200
https://gunny.qs3d.site/Request/subactivelist.ashx -> 200 success
```

- [ ] **Step 2: Build the candidate config locally**

Copy the current production/staged config to a temporary file, run `Patch-GunnyPublicDomains.ps1`, and inspect the resulting eight URL values before upload.

- [ ] **Step 3: Backup and replace production config**

Over key-authenticated SSH, set `$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'` and `$backup = "C:\GunnyWeb\backups\domain-cutover-$stamp"`; create that directory, copy the live `Gunny\config.xml`, upload the candidate config, then probe `https://gunny.qs3d.site/Gunny/config.xml` and verify every expected domain value is present.

- [ ] **Step 4: Rollback rule**

If the public config probe fails, any expected value is missing, or the endpoint is not HTTP 200, immediately restore the previous config from the timestamped backup and stop.

### Task 5: Full Ruffle and launcher acceptance

- [ ] **Step 1: Synthetic full-loader smoke**

Use the exact packaged Ruffle binary with `--graphics gl` and `--no-avm2-optimizer`. Run `Loading.swf` through `https://gunny.qs3d.site/Gunny/flash/` with existing user `win` and an intentionally invalid smoke key. Never use or print a real session key.

- [ ] **Step 2: Inspect the fresh Ruffle log**

Require all of these to be absent:

```text
subactivelist.ashx ... 500
smallnote.txt ... 404
config.xml ... 404
res.gn.zing.vn
InvalidDomain("http://res.gn.zing.vn
```

An eventual invalid-login/disconnect message is expected for the synthetic key and is not a failure.

- [ ] **Step 3: Build/package the launcher from exact merged source**

Run launcher tests, contract export, publish, and package verification. Require HTTPS game base in the package while `--socket-allow` remains exactly `103.9.156.182:9200`.

- [ ] **Step 4: Real-login smoke**

Launch the exact final package and let the user authenticate normally. Inspect sanitized logs only; never echo password or login key. Require the authenticated lobby/game path to load without the former activity-interface popup or blocking resource-domain errors.

- [ ] **Step 5: Final verification and commit**

Run all launcher tests, config patch tests, edge public probes, resource public probes, and production `subactivelist` probe fresh. Commit only source/docs/scripts; do not commit runtime logs, credentials, or generated packages.
