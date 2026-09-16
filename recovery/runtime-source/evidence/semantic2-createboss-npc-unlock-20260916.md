# Phase 2 simple-boss spawn NPC unlock

- Semantic commit: `f5c9a63` restores the Fight/Road-consensus simple-boss spawn chain.
- Restored chain: additive living types, `Living.ActionStr`, ADD_LIVING action payload, action-aware `SimpleBoss`, boss bookkeeping, and default-config `PVEGame.CreateBoss(..., action)`.
- Existing legacy constructors/overloads remain unchanged.
- TDD RED failed on the missing recovered living-type tail; GREEN smoke passed.
- `Game.Logic` Release/net35 build completed with exit 0.
- Qualification covered the final 4 deferred Road NPC candidates.
- Dependency-closed NPCs: `ThirdHardKingFirst`, `ThirdNormalKingFirst`, `ThirdSimpleKingFirst`, `ThirdTerrorKingFirst`.
- Final guards: 4/4 compile mapping, sensitive hits 0, diff-check PASS.
- Fresh inventory: runtime-only 337, variant-conflict 59, canonical-present 579, runtime-only members 3606.
- Previous verified inventory was runtime-only 341.
- Fresh `GameServerScript` build: 0 compiler errors with `PostBuildEvent` disabled.
- Road NPC runtime-only backlog is now zero.
