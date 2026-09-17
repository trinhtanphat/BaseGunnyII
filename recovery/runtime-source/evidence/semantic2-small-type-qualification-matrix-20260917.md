# Semantic recovery — small-type qualification matrix (2026-09-17)

## Base
- Branch: `recovery/runtime-semantic-phase2-20260915`.
- Base SHA: `d53652a0780c1baa6cbdb80854c6404e3838ccdb`.
- Fresh inventory after everyday-active closure: runtime-only types 195; canonical-present types 721; variant-conflict types 59; runtime-only members 2385; canonical-present members 5842.

## Packet-handler qualification
- `UseReworkNameHandler`: deferred; recovered call requires a different `PlayerBussiness.RenameNick` signature.
- `QuickBuyGoldBoxHandler`: deferred; missing `GamePlayer.MoneyDirect` and 12-arg `CreateItemBox` overload.
- `SearchGoodsHandler`: deferred; missing `GameProperties.SearchGoodsFreeCount`.
- `HonorUpHandler`: deferred; missing totem/player state and managers (`totemId`, `MaxBuyHonor`, `Toemview`, `TotemHonorMgr`, money/honor APIs).
- `CampBattleHandler`: deferred; missing `BaseCampBattleRoom` and `RoomMgr.CampBattleRoom`.
- `FightFootballTimeTakeoutHandler`: deferred; missing football card state/manager APIs.
- `CardInfoHandler` was not counted as a valid candidate because a compile mapping already exists canonical; the batch helper intentionally excluded that result from qualification conclusions.

## Buffer qualification
- `ActivityDungeonBubbleBuffer` and `ActivityDungeonNetBuffer`: deferred.
- They require missing `GamePlayer.FightBuffs`, `UpdateFightBuff`, fight-server serialization, and share that infrastructure with consortia/world-boss buffers.

## Quest-condition qualification
- `AdoptPetCondition`, `CropPrimaryCondition`, `NewGearCondition`, `SeedFoodPetCondition`, `UserToemGemstoneCondition`, and `UnknowQuestCondition` each compile-fail only on their corresponding missing `GamePlayer` event.
- Runtime trigger audit found explicit trigger sites for AdoptPet, CropPrimary, NewGear, SeedFoodPet, and UserToemGemstone; `UnknowQuestConditionEvent` has no recovered trigger outside `GamePlayer` and remains inert/deferred.
- The trigger carriers are not uniformly canonical: `PlayerFarm`, `FigSpiritUpGradeHandler`, and `OpenOneTotemHandler` are absent; canonical `PetHandler`, `EnterFarmHandler`, and `UserChangeItemPlaceHandler` diverge materially from recovered runtime flow.
- Therefore no quest condition was promoted in this checkpoint; restoring event declarations without their action trigger semantics would be incomplete.

## Policy
- No compile-only or inert promotion.
- No master merge, production deploy, service restart, force push, or guessed cross-layer behavior.