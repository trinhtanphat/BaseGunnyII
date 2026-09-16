# Semantic phase 2: remaining Mession blocker matrix after CreateNpc(6)

Date: 2026-09-16
Recovery branch: `recovery/runtime-semantic-phase2-20260915`
Base commit: `387265c446b1fc27dca17e965fc122950c22fb6c`

## Fresh qualification

A detached worktree at the recovery head promoted all 90 still-missing recovered Mession source files into `GameServerScript.csproj`, using namespace-only normalization where required for C# 7.3. `GameServerScript` was then built with the isolated .NET 8.0.425 MSBuild/Roslyn toolchain, net35 reference assemblies, and `PostBuildEvent=` disabled.

Result: 90 error-bearing mission files and 149 compiler error rows. `CreateNpc(6)` contributes zero remaining errors; the previous count was 154 rows, so its five known blockers were removed exactly with no replacement blocker introduced.

## Current blocker groups

- `CreateBoss(7)`: 40 errors / 35 files.
- `TakeSnow`: 24 / 24.
- `CanEnterGate`: 20 / 20.
- `CanShowBigBox`: 20 / 20.
- `CreateGate`: 20 / 20.
- `TakeConsortiaBossAward`: 10 / 10.
- `UpdatePveResult`: 6 / 6.
- `TotalDameLiving`: 5 / 5.- `IsKillWorldBoss`: 2 / 2.
- `AddTemplate(5)`: 1 / 1.
- `ChangeSpecialBall`: 1 / 1.

## Dependency assessment

`CreateBoss(7)` is not an isolated overload. Exact runtime behavior consumes `PVEGame.WorldbossBood` and `AllWorldDameBoss`, sourced through `IGamePlayer` and implementations in `GamePlayer`/`ProxyPlayer`, with state populated by world-boss room and consortia-boss flows. `BotProxyPlayer` has no recovered counterpart proving equivalent values, so this cluster remains fail-closed.

`AddTemplate(5)` is a cross-layer inventory and notification contract: recovered code extends `IGamePlayer`, Fighting `ProxyPlayer`/`ServerClient`, and Road `GamePlayer`. Restoring only a wrapper would discard `eItemNotice` semantics, so `CSM1083` and `TakeSnow` remain deferred.

`TakeConsortiaBossAward`, `TotalDameLiving`, `UpdatePveResult`, `IsKillWorldBoss`, and `ChangeSpecialBall` form the same wider result/persistence state cluster and are not promoted independently.

The Labyrinth state group (`CanEnterGate`, `CanShowBigBox`, `CreateGate`) is the next isolated candidate. It will be qualified separately against all 90 missions before any source promotion.

## Exclusive current blockers

Nine missions are currently blocked only by `CreateBoss(7)`: `DCH4202`, `DCN4102`, `DCT4302`, `DLH5204`, `DLN5104`, `DLT5304`, `ETH3202`, `ETN3102`, `ETT3302`.

`CSM1083` is blocked only by `AddTemplate(5)`. Twenty-four CampBattle/QX/YearMonster mission files carry `TakeSnow`; numerous odd Labyrinth stages currently have `CreateGate` as their sole visible compile blocker.

## Safety

This document records qualification evidence only. No `master` merge, force push, production deployment, service restart, or runtime binary replacement was performed.