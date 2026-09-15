# PvP Match Bot Fill Design

## Goal
Let a real player find a playable 1v1 match when concurrency is low without creating persistent fake accounts.

## Existing reference
`DDTank41` already contains `RobotGamePlayer`, `RobotManager`, waiting-room robot population, and an `IsAutoBot` flag. `BaseGunnyII` lacks this implementation and uses an older FightServer room-create protocol, so the newer files cannot be copied unchanged.

## Design
Port only the minimum in-memory bot player support required for Match 1v1. The compiled runtime performs matchmaking inside Fighting.Server, so a human room is searched against real rooms first; if still unmatched after the room has been eligible for 5 seconds, with the room manager polling once per second, Fighting.Server creates one synthetic bot room locally and starts it through the normal GameMgr/PVPGame flow. If a human opponent arrives first, no bot is allocated.

Bots have negative runtime-only IDs, `IsAutoBot=true`, a null packet sink, a valid weapon/equipment loadout, and stats derived from the human's grade/fight power band. They are removed from room/world state after the match and are never inserted into membership/user tables.

## Reward policy
Matches containing an auto bot must not award ranked/guild/offer progression intended for human-vs-human matchmaking. Normal visual game-over and base GP flow may remain so the match completes normally.

## Success criteria
- Human opponent within 5 seconds => human-vs-human, no bot.
- No human opponent after 5 seconds => exactly one bot fills 1v1.
- Bot is created inside the compiled Fighting.Server matchmaking path, so no new cross-server packet field is introduced; `IBotGamePlayer` is the explicit in-process bot marker.
- Bot can take turns/shoot through server-side combat flow.
- Bot is removed after match; no database account is created.
- Bot matches do not grant ranked/guild/offer farming rewards.