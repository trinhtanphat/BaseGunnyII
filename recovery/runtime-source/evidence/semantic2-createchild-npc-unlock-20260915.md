# Phase 2 - directional child spawn unlock

Date: 2026-09-15
Branch: `recovery/runtime-semantic-phase2-20260915`

## Semantic cluster

The recovered Fight and Road runtime variants agree on the additive directional NPC spawn APIs.
The cluster restores `Living.Config`, `SimpleNpc(..., direction)`, `PVEGame.BaseLivingConfig/CreateNpc(..., direction)`, and `SimpleBoss.CreateChild(..., direction)`.
Legacy overloads remain present and unchanged in call shape.

TDD smoke: `tests/runtime-semantic-createchild-compat-smoke.ps1`.
The smoke was observed RED before the cluster and GREEN after implementation.
## Compiler-guided qualification

Starting deferred NPC files: 107.
After restoring the directional child-spawn cluster, 61 files still had compiler errors and 46 became dependency-closed.
The 61 blocked files were removed from the qualification wave; the remaining 46 built successfully.

Fresh inventory after the 46-file wave:
- runtime-only types: 402
- variant-conflict types: 59
- canonical-present types: 513
- runtime-only members: 4,451

Final guards: 46 files / 46 compile items, zero sensitive hits, zero binary/config artifacts, `git diff --check` clean, GameServerScript build exit 0 with zero errors.
