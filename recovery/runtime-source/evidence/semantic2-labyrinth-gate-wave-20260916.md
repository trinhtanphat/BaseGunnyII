# Semantic phase 2: Labyrinth gate-wave recovery

Date: 2026-09-16
Parent recovery commit: `3950f4449ec9d9bcd4e2fc5283c11a1cafece185`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Qualification and scope

The current blocker matrix contained 90 missing recovered Mession files and 149 compiler error rows. A detached qualification worktree promoted all 90 sources, then added only the exact recovered `PVEGame.CanEnterGate`, `CanShowBigBox`, and `CreateGate(bool)` contracts.

That experiment reduced compiler errors from `149` to `89`, eliminating all 20 `CreateGate`, 20 `CanEnterGate`, and 20 `CanShowBigBox` errors with no replacement errors. Error-bearing mission files fell to 58; exactly 32 Labyrinth missions became compiler-clean.

This wave promotes only those 32 compiler-clean missions. The eight remaining Labyrinth stages `40012`, `40016`, `40022`, `40026`, `40030`, `40032`, `40034`, and `40040` remain deferred; fresh qualification shows they are still blocked by recovered `CreateBoss(7)` semantics.

## TDD and parity

The guard was introduced first and RED for five missing contract groups: both gate fields, `CreateGate(bool)`, all 32 mission sources, and all 32 compile mappings.
After promotion the guard is GREEN for all five groups.
All 32 mission files match recovered Road source after file-scoped namespace normalization only.
`CreateGate(bool)` has exact recovered tokens and both gate field declarations are present in recovered and canonical source.Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.

## Fresh build verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with the net35 reference-assembly pack and C# 7.3.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` was explicitly disabled.
Warnings remain the existing legacy test-assembly / unused-field warnings.

## Fresh runtime-source inventory

- Runtime types: `1551`; runtime members: `10639`; recovered C# files: `2334`.
- `runtime-only` types: `203` (was `235`).
- `canonical-present` types: `713` (was `681`).
- `variant-conflict` types: `59` (unchanged).
- `same-signature/body-different` types: `576` (unchanged).
- `runtime-only` members: `2410` (was `2705`).
- `canonical-present` members: `5817` (was `5522`).
- `variant-conflict` members: `176` (unchanged).
- `same-signature/body-different` members: `2236` (unchanged).
The 32 promoted runtime-only mission types account exactly for the type delta. The member delta is 295 runtime-only members becoming canonical-present.

Promoted stages: all `40001-40011`, then `40013-40015`, `40017-40021`, `40023-40025`, `40027-40029`, `40031`, `40033`, and `40035-40039`; the guard contains the exact 32-file allowlist.

## Safety

`git -c core.whitespace=cr-at-eol diff --check` passes while preserving repository EOL policy.
The wave is isolated on the recovery branch. No `master` merge, force push, production deployment, service restart, or runtime binary replacement was performed.
