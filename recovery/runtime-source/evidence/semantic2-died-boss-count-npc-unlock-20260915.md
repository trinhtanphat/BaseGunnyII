# Semantic phase 2: dead-boss count NPC unlock

- Semantic API commit: `b003d00` (`BaseGame.GetDiedBossCount`).
- Recovered Fight/Road bodies agree: enumerate `FindAllBoss()` and count bosses where `IsLiving == false`.
- Qualification input: 18 remaining recovered NPC scripts.
- After API restoration: 14 files still had compiler blockers; 4 became dependency-closed.
- Promoted NPCs: `ThirteenHardAntBoss`, `ThirteenNormalAntBoss`, `ThirteenSimpleAntBoss`, `ThirteenTerrorAntBoss`.
- Fresh inventory: runtime-only types 359 -> 355; variant-conflict 59; canonical-present 560.
- Fresh runtime-only members: 3852.
- Fresh GameServerScript Release/net35 qualification build: 0 compiler errors.
- Sensitive scan: 0 hits.
- No production deploy/restart was performed by this recovery lane.
