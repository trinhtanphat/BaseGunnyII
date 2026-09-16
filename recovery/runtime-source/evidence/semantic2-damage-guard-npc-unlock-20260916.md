# Phase 2 damage and guard effect NPC unlock

- Semantic commit: `995809e` restores additive effect slots `DamageEffect=41` and `GuardEffect=42` without renumbering existing canonical IDs.
- `DamageEffect` runtime source is from Road; `GuardEffect` runtime source is from Fight; Fight/Road enums agree on slots 41/42.
- Exact-head RED snapshot failed on missing `DamageEffect` slot 41; current GREEN smoke passed.
- `Game.Logic` Release/net35 build completed with exit 0.
- Qualification started from 9 deferred NPC candidates.
- Four `Third*KingFirst` files remain blocked only by `SimpleBoss.CreateBoss`.
- Dependency-closed NPCs: `ThirteenHardBrynBoss`, `ThirteenNormalBrynBoss`, `ThirteenSimpleBrotherNpc`, `ThirteenSimpleBrynBoss`, `ThirteenTerrorBrynBoss`.
- Trimmed 5-file GameServerScript build: exit 0.
- Final guards: 5/5 compile mapping, sensitive hits 0, diff-check PASS.
- Fresh inventory: runtime-only 341, variant-conflict 59, canonical-present 574, runtime-only members 3670.
- Previous verified inventory was runtime-only 348; seven transitioned types are the two effects plus five NPC scripts.
- Fresh GameServerScript build: 0 compiler errors with `PostBuildEvent` disabled.
