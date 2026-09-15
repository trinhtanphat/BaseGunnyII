# Post-rebase qualification — 2026-09-15

Qualified source head before this evidence-only commit: `518cca10f89c4c392326c58ec1ce75abcaa9ca50`.
Base: `origin/master@b7a4c690a71f27b9a191c61100251cde699f5c31`.
Branch was `ahead 13 / behind 0` at qualification time.

## Semantic inventory

- `RuntimeSourceInventory.Tests`: PASS.
- Inventory CLI exit: 0.
- Recovered inventory: 1,551 types, 10,639 members, 2,334 C# files.
- Runtime-only/additive types remaining: 558.
- Variant-conflict types: 59.
- Current report: `canonical-diff.json` / `canonical-diff.md`.

## Canonical build gate

- `CenterServer.sln`: exit 0, 0 errors, 8 warnings.
- `FightingServer.sln`: exit 0, 0 errors, 26 warnings.
- `GameServer.sln`: exit 0, 0 errors, 128 warnings; manifest/assembly signing disabled only on the qualification command line.

## Recovered project gate

- 19/19 service-scoped recovered projects: PASS, 0 failures, 0 errors.
- That full build ran on the immediately preceding rebased head `9baf61f`; `recovery/runtime-source` was byte-identical across the final rebase to `518cca1`, so the built recovered project inputs did not change.

## Live runtime drift boundary

`manifest.json` remains a historical forensic snapshot; it is not silently refreshed to chase later deployments.
At the final live check, 14/19 deployed binary hashes still matched the snapshot and 5 differed: `center/Center.Server`, `Fight/Fighting.Server`, `Fight/Game.Logic`, `Fight/GameServerScripts`, and `Road/GameServerScripts`.
Exact expected/actual SHA-256 values and file mtimes are recorded in `runtime-live-vs-snapshot-20260915T1024.json`.
Center, Fighting, and Road services were observed with newer start times during qualification, confirming concurrent/external runtime activity.
This recovery workflow did not copy binaries into the live server folders and did not restart those services.

## Integration policy

Do not merge protected `master` from this evidence alone. Push this branch and use PR review/protected checks; keep variant conflicts and method/body deltas deferred unless separately reviewed and qualified.
