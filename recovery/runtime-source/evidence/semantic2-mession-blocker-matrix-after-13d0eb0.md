# Semantic phase 2: remaining mission blocker matrix after 13d0eb0

Date: 2026-09-16
Recovery commit: `13d0eb0a00d3d12b91cd5ea19332475cc1adfd32`
Branch: `recovery/runtime-semantic-phase2-20260915`

## Fresh qualification

A detached worktree at `0b703ce571606277fdf580d4e32954a8ee980300` promoted the remaining 58 recovered missions for compiler qualification only.
The `IsKillWorldBoss` wave reduced the compiler matrix from `83` to `81` rows and removed both `IsKillWorldBoss` errors.
After that wave the remaining matrix is:

- `CreateBoss(7)`: 40 errors across 35 files.
- `TakeSnow`: 24 errors across 24 files.
- `TakeConsortiaBossAward`: 10 errors across 10 files.
- `UpdatePveResult`: 6 errors across 6 files.
- `AddTemplate(5)`: 1 error in 1 file.

Total: `81` error rows across `58` mission files.

## Exclusive mission groups

`CreateBoss(7)` alone blocks DCH4202, DCN4102, DCT4302, DLH5204, DLN5104, DLT5304, ETH3202, ETN3102, ETT3302, Labyrinth40012/16/22/26/30/32/34/40.
`TakeSnow` alone blocks CampBattle60001-60017 except 60018, CampBattle60019/60020, QX12016, and YearMonster.
`AddTemplate(5)` alone blocks CSM1083. `UpdatePveResult` alone blocks YearMonster1347.
## Why the remaining clusters are deferred

`CreateBoss(7)` depends on recovered `WorldbossBood` / `AllWorldDameBoss` state propagated through `IGamePlayer`, `GamePlayer`, `ProxyPlayer`, room/consortia setup, and `PVEGame`.
`UpdatePveResult` and `AddTemplate(5)` likewise expand `IGamePlayer` and require implementations in all implementers.
Canonical also contains `Fighting.Server.GameObjects.BotProxyPlayer`, but the recovered runtime-source snapshot contains no matching `BotProxyPlayer` implementation for these newer interface contracts.

A fresh filesystem search across the 182 Gunny worktrees found only canonical-style `BotProxyPlayer.cs` copies and no copy containing `WorldbossBood`, `AllWorldDameBoss`, `UpdatePveResult`, or the five-argument `AddTemplate` notice contract.
GitHub code search and a public GitHub web search also found no exact `BotProxyPlayer` carrier for these contracts.
Because inventing no-op/default implementations would be semantic guesswork, these cross-layer clusters remain fail-closed.

## Safety / next direction

No production deploy or service restart was performed. `master` is untouched and no force-push is permitted.
The next recovery work should select another runtime-only member/type whose recovered Road/Fight implementations are dependency-closed in canonical source, rather than forcing one of the blocked interface-expansion clusters.