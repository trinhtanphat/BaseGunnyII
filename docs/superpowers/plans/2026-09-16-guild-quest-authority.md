# Guild Quest Authority Fix Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Prevent quest 339 from becoming falsely completed/claimable because of guild-state or QUEST_UPDATE packet desynchronization.

**Architecture:** Keep `OwnConsortiaCondition` as the authoritative guild predicate. Lock the legacy Flash packet contract with a regression smoke test, remove any serializer field not consumed by the client parser, and retain claim-time condition revalidation.

**Tech Stack:** C#/.NET Framework, PowerShell smoke tests, legacy Flash/ActionScript packet contract, GitHub Actions.

**Spec:** `docs/superpowers/specs/2026-09-16-guild-quest-authority.md`

## Global Constraints
- Work from `origin/master` in the isolated worktree.
- Do not modify FightPower storage/types in this change.
- Do not bypass tests or merge gates.

---

### Task 1: Lock QUEST_UPDATE contract

**Files:**
- Create: `tests/guild-quest-update-contract-smoke.ps1`
- Modify: `Game.Server/Packets/Server/AbstractPacketLib.cs`

**Interfaces:**
- Consumes: deployed 4.1 client order `QuestID, IsAchieved, Condition1..4, CompleteDate, RepeatLeft, Quality, IsExist`.
- Produces: a serializer with no unconsumed trailing per-quest integer.

- [ ] Write a smoke test that fails while `pkg.WriteInt(3)` follows `WriteBoolean(info.Data.IsExist)`.
- [ ] Run it and confirm RED for the extra QuestLevel field.
- [ ] Remove only the unconsumed serializer field.
- [ ] Run the new smoke plus `tests/guild-quest-membership-smoke.ps1` and confirm GREEN.

### Task 2: Guard claim-time authority

**Files:**
- Modify: `tests/guild-quest-update-contract-smoke.ps1`
- Verify: `Game.Server/Packets/Client/QuestFinishHandler.cs`, `Game.Server/Quests/BaseQuest.cs`

**Interfaces:**
- Consumes: `QuestInventory.Finish` → `BaseQuest.Finish` → `CanCompleted`.
- Produces: regression proof that rewards are not issued solely from stale client completion state.

- [ ] Extend the smoke test to assert QuestFinishHandler delegates to QuestInventory.Finish.
- [ ] Assert BaseQuest.Finish calls CanCompleted before condition Finish/reward flow.
- [ ] Run the smoke and confirm GREEN without broad production changes.

### Task 3: Build, publish, deploy, verify

**Files:**
- Build: `GameServer.sln`
- Deploy: `C:\Gunny\GunnyFileExe\SERVER\Road\Game.Server.dll` and matching dependencies only if changed.

- [ ] Run relevant smoke tests and `git diff --check`.
- [ ] Build Release with the installed MSBuild toolchain.
- [ ] Commit and push the isolated branch.
- [ ] Open PR, wait for required checks, then merge to `master` without bypass/force.
- [ ] Back up the live DLL, deploy the exact merged build, restart Road service, and verify process/log health.
- [ ] Verify quest 339 server state remains incomplete for a no-guild fixture and becomes eligible only after valid guild membership.