# Phase 2 continuous blood reduction NPC unlock

- Semantic commit: `b56d962` restores `ContinueReduceBlood` with explicit effect slot 40.
- Existing canonical effect IDs 1-36 were not renumbered.
- TDD RED failed on missing slot 40; GREEN smoke and Game.Logic Release/net35 build passed.
- Qualification started from 13 deferred Road NPC candidates.
- Fresh compiler map after the semantic cluster: 9 files still failed, 0 canonical files failed.
- Dependency-closed NPCs: `QXBoss70001`, `SeventhHardSecondBoss`, `SeventhNormalSecondBoss`, `SeventhSimpleSecondBoss`.
- Trimmed 4-file GameServerScript build: exit 0.
- Final guards: 4/4 compile mapping, sensitive hits 0, diff-check PASS.
- Fresh inventory: runtime-only 348, variant-conflict 59, canonical-present 567, runtime-only members 3761.
- Previous verified inventory was runtime-only 353; five transitioned types are the effect plus four NPC scripts.
- Fresh GameServerScript build: 0 compiler errors with PostBuildEvent disabled.