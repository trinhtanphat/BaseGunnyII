# Gunny runtime source recovery status — 2026-09-14

## Verified recovery
- Runtime first-party entries decompiled: **19/19**.
- Distinct deployed binary variants: **17**.
- Buildable recovery projects: **19/19 PASS**, `--no-restore`, **0 warnings / 0 errors**.
- Deployed runtime integrity: **19/19 SHA256 match** the recovery baseline.
- Running services were not restarted: `Center.Service`, `Fighting.Service`, `Road.Service` remain running.

## Repo-wide Roslyn confirmation
Best-source repositories were indexed by fully-qualified C# type/method signatures, not filenames:
- `DDTank41`: 2,429 C# files, 2,345 types, 11,538 methods.
- `DDTank4.1`: 510 C# files, 506 types, 1,659 methods.
- `BaseGunnyII`: 1,230 C# files, 1,220 types, 5,663 methods.
- `DDTank-3.0`: 1,316 C# files, 1,142 types, 5,735 methods.

Confirmed runtime-only type source files copied: **657** service-scoped files, **654 unique files after runtime-SHA dedupe**.
Method-only signature deltas requiring manual semantic merge/review: **1,475** across 12 assembly rows.
## Largest confirmed runtime-only areas
- `Road/GameServerScripts`: 525 confirmed types, 4,893 confirmed runtime-only method signatures.
- `Road/Game.Server`: 73 confirmed types, 870 confirmed runtime-only method signatures.
- `Fight/Game.Logic`: 24 confirmed types, 314 confirmed runtime-only method signatures.
- `Road/Game.Logic`: 24 confirmed types, 314 confirmed runtime-only method signatures.
- `center|Fight|Road/Bussiness`: 2 confirmed types per row; Fight/Road share the same runtime SHA.

## Recovery artifacts
- Raw forensic decompile: `C:\Gunny\_decompiled_20260914\RuntimeByService`
- Buildable copy: `C:\Gunny\_work\RuntimeByService-buildable-20260914`
- Repo-wide confirmation: `C:\Gunny\_work\runtime-confirmed-diff-roslyn-20260914.{json,md}`
- Recovery bundle: `C:\Gunny\_work\RecoveredRuntimeOnly-20260914`
- Recovery ZIP: `C:\Gunny\_work\RecoveredRuntimeOnly-20260914.zip`
- Build verification: `C:\Gunny\_work\runtime-buildable-final-results-20260914.json`
- Deployed hash verification: `C:\Gunny\_work\deployed-hash-verification-20260914.json`

## Important limitation
A confirmed type is absent by exact fully-qualified name from the entire best-source repository. A confirmed method signature is absent by exact Roslyn-normalized signature. Method-only deltas can still represent changed signatures, generated code, or semantic/compiler-era differences, so they are intentionally not auto-ported. No original source repository, deployed binary, configuration, or running service was modified by this recovery pass.
