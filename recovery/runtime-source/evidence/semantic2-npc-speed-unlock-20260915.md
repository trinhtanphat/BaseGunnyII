# Phase 2 - NPC speed semantic unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`

## Semantic delta

Recovered `SqlDataProvider.Data.NpcInfo` has an additive `int speed` property in Center, Fight, and Road variants.
Recovered `Bussiness.ProduceBussiness.GetAllNPCInfo()` hydrates it from the `speed` column returned by `SP_NPC_Info_All`.
The canonical source previously had neither the property nor the hydration assignment.

TDD smoke: `tests/runtime-semantic-npc-speed-smoke.ps1`.
The test was observed RED on the missing property, then GREEN after the two-file semantic patch.

## Build qualification

`SqlDataProvider`, `Bussiness`, `Game.Logic`, and `GameServerScript` build with exit code 0 using the .NET Framework reference-pack root.
`GameServerScript` qualification disables its post-build copy event to avoid deployment side effects.

## Unlock impact

Qualification started from 136 deferred NPC source files.
After the speed metadata cluster, compiler errors remained in 107 files and 29 files became dependency-closed.
Those 29 files were retained; the 107 still-blocked files were removed from the qualification wave.
The retained 29-file wave builds successfully as part of `GameServerScript`.

Fresh semantic inventory after retaining the wave:
- runtime-only types: 477 -> 448
- variant-conflict types: 59 -> 59
- canonical-present types: 467
- runtime-only members: 5002

Final guards require 29 NPC files, 29 project compile entries, zero sensitive hits, zero binary/config artifacts, and `git diff --check` success.
