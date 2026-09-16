# Phase 2 player speed multiplier NPC unlock

- Semantic commit: `8f43b9d` restores the Fight/Road-consensus speed-multiplier movement chain.
- Chain: `SendGamePlayerProperty` -> `Living.SpeedMultX` -> `PlayerSpeedMultAction` -> `Player.StartSpeedMult`.
- TDD RED failed at missing `SendGamePlayerProperty`; GREEN smoke passed after the bounded implementation.
- `Game.Logic` Release/net35 build completed with exit 0.
- Qualification started from 14 deferred Road NPC candidates.
- Fresh compiler map after the semantic cluster: 13 files still failed, 0 canonical files failed.
- The newly dependency-closed NPC is `GameServerScript.AI.NPC.YearMonster`.
- Trimmed build with only `YearMonster`: exit 0.
- Final guards: 1/1 compile mapping, sensitive hits 0, artifact hits 0, diff-check PASS.
- Fresh inventory: runtime-only 353, variant-conflict 59, canonical-present 562, runtime-only members 3830.
- Previous verified inventory was runtime-only 355; the two transitioned types are `Game.Logic.Actions.PlayerSpeedMultAction` and `GameServerScript.AI.NPC.YearMonster`.
- Fresh `GameServerScript` build: 0 compiler errors with `PostBuildEvent` disabled.
