# Semantic phase 2: IsKillWorldBoss state recovery

Date: 2026-09-16
Parent recovery commit: `0b703ce571606277fdf580d4e32954a8ee980300`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Dependency closure

Recovered Fight and Road `PVEGame` sources both define exactly `public bool IsKillWorldBoss;` with no initializer.
The only additional recovered occurrences are assignments to `false` in `AC30002` and `AC30004`; no recovered reader or extra lifecycle/persistence contract exists for this field.
Canonical `PVEGame` lacked the field, so this wave restores only the exact state member and intentionally does not promote either mission yet because they still have unrelated blockers.

## TDD and parity

The guard checks that the field exists exactly once and that no guessed initializer was introduced.
Fresh guard result: GREEN for both contracts.
Road recovered field: present. Fight recovered field: present. Canonical candidate field: present exactly once.

## Qualification

A detached worktree at exact parent `0b703ce571606277fdf580d4e32954a8ee980300` promoted the remaining 58 recovered missions for compiler qualification only.
Before the field, `GameServerScript` reported `83` compiler error rows.
After adding the exact field, it reported `81` compiler error rows and `0` remaining `IsKillWorldBoss` errors.
No mission source is promoted by this wave.
## Fresh build verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with the .NET Framework 3.5 reference pack and `LangVersion=7.3`.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` was explicitly disabled.

## Fresh runtime-source inventory

- Runtime types: `1551`; runtime members: `10639`; recovered C# files: `2334`.
- `runtime-only` types: `203`; `canonical-present` types: `713`; `variant-conflict` types: `59`.
- `runtime-only` members: `2410`; `canonical-present` members: `5817`.
- Classification counts remain unchanged because this wave restores one field inside the already-canonical `PVEGame` type.

## Safety

Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.
`git -c core.whitespace=cr-at-eol diff --check` passes.
No production deploy/service restart was performed. `master` remains untouched and no force-push is permitted.