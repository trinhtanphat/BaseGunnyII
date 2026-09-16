# Semantic phase 2: GON6101 ball-wave recovery

Date: 2026-09-16
Parent recovery commit: `ada1a1c0e8e1e8e2035745d598339931d6887198`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Dependency closure

The initial mission blocker set was `BaseGame.Shuffer<T>`, `PVEGame.CreateBall`, and `BaseGame.ClearBall`, all used by `GON6101`.
Exact runtime review showed `CreateBall` requires the recovered `Ball` type and its supporting state/behavior: `BaseGame.m_tempBall`, both `AddBall` overloads, `Living.PickBall`, and `PhysicalObj.ActionMapping/getActionMap`.
The wave therefore promotes the complete dependency-closed ball path rather than a compile-only mission patch.

## TDD and semantic parity

The guard was introduced first and was RED with 11 missing contracts before source promotion.
After promotion the guard is GREEN for Shuffer, ball state/AddBall/ClearBall, CreateBall, Ball source, PickBall, ActionMapping, project mappings, and GON6101.
Exact-token parity is true for `Shuffer<T>`, `ClearBall`, both `AddBall` overloads, `CreateBall`, and `PickBall`.
Fight/Road runtime variants agree for the promoted primitive methods.
`Ball.cs` and `GON6101.cs` match recovered Road source after file-scoped namespace normalization only.
`PhysicalObj.getActionMap` preserves all 13 recovered mappings plus default, lowered from a switch expression to a C# 7.3 switch statement.
Sensitive scan: `0` hits. Decompiler/source-artifact scan: `0` hits.

## Fresh build verification

Toolchain: `C:\Gunny\BuildTools\dotnet8\dotnet.exe` SDK 8.0.425 / MSBuild 17.11.48 with `Microsoft.NETFramework.ReferenceAssemblies.net35` targeting root and `LangVersion=7.3`.
`Game.Logic` Release rebuild: exit `0`, compiler error lines `0`.
`GameServerScript` Release rebuild: exit `0`, compiler error lines `0`; `PostBuildEvent=` was explicitly disabled.
Remaining warnings are pre-existing legacy test-assembly / unused-field warnings and are not introduced by this wave.
No production deploy or service restart was performed.

## Fresh runtime-source inventory

- Runtime types: `1551`; runtime members: `10639`; recovered C# files: `2334`.
- `runtime-only` types: `235` (was `237`).
- `canonical-present` types: `681` (was `679`).
- `variant-conflict` types: `59` (unchanged).
- `same-signature/body-different` types: `576` (unchanged).
- `runtime-only` members: `2706` (was `2724`).
- `canonical-present` members: `5521` (was `5504`).
- `variant-conflict` members: `176` (unchanged).
- `same-signature/body-different` members: `2236` (was `2235`).

The two promoted runtime-only types are `Game.Logic.Phy.Object.Ball` and `GameServerScript.AI.Messions.GON6101`.
Seventeen runtime-only members became canonical-present. `PhysicalObj.getActionMap(string)` becomes `same-signature/body-different` because of the required C# 7.3 syntax lowering; its mapping behavior is parity-checked explicitly.

## Safety

`git -c core.whitespace=cr-at-eol diff --check` passes while preserving existing CRLF/mixed-EOL policy.
This recovery branch remains isolated: no merge to `master`, no force push, no production deployment/restart.
