# Guild Quest Authority Fix Spec

## Problem
Quest 339 ("Hiệu lệnh guild") can be rendered as completed/claimable by the Flash client even when the authoritative server quest progress says it is incomplete.

## Required behavior
- A player not currently in a valid guild must never complete quest condition type 18.
- QUEST_UPDATE must serialize exactly the fields consumed by the deployed 4.1 client parser, without an extra per-quest field that shifts subsequent records.
- Quest claiming must remain server-authoritative and revalidate current conditions before awarding rewards.
- Existing guild join/leave refresh behavior must remain intact.

## Scope
Change only guild quest synchronization/packet contract and regression coverage. Do not migrate FightPower to BIGINT in this change.