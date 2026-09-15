# Semantic phase 2 - SendGameFocus NPC unlock

- Semantic API commit: `b823bc7` (`PVEGame.SendGameFocus`).
- Qualification source set before API: 29 deferred NPC files.
- Compiler errors after API: 25 files.
- Dependency-closed NPC files promoted: 4.
- GameServerScript build: PASS, 0 compiler errors.
- Runtime-only types: 370 -> 366.
- Variant-conflict types: 59 (unchanged).
- Canonical-present types: 549.
- Runtime-only members: 3998.
- `git diff --check`: PASS.
- No production deploy or service restart performed by this workflow.
