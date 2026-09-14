# PvP Bot Fill Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fill an otherwise-empty 1v1 PvP match with one runtime-only bot and let that bot complete turns without creating or rewarding a fake account.

**Architecture:** Matchmaking actually runs inside the compiled `Fighting.Server` room manager, so the bot is created there after a per-room 5,000 ms eligibility deadline while matchmaking polls every 1 second rather than serialized through the Game.Server↔Fighting protocol. The bot implements `IGamePlayer` plus a small `IBotGamePlayer` hook consumed by `PVPGame`; synthetic rooms never call upstream room lifecycle methods for the bot.

**Tech Stack:** C#/.NET Framework legacy server, `Fighting.Server`, `Game.Logic`, PowerShell smoke tests.

**Spec:** `docs/superpowers/specs/2026-09-14-pvp-bot-fill-design.md`

## Global Constraints
- Human-vs-human always wins the race if a real opponent is available before fill.
- Fill only one-player non-guild matchmaking rooms.
- Bot IDs are negative, in-memory only, and never written to user/member tables.
- Bot matches must not grant ranked/guild/offer farming progression.
- The bot must reach normal `PVPGame` physics/shoot flow and be removed with the synthetic room.

---

### Task 1: Lock bot-fill source contracts
**Files:** Test `tests/pvp-bot-fill-smoke.ps1`; inspect `Fighting.Server/Rooms/ProxyRoomMgr.cs`, `ProxyRoom.cs`, `Game.Logic/PVPGame.cs`.
- [ ] Add assertions for 1-second polling and a per-room 5,000 ms fill deadline, one-player guard, non-guild guard, synthetic-room lifecycle guard, bot turn hook, auto-loading, and reward suppression.
- [ ] Run the test against the unmodified baseline and confirm at least one required contract is RED.
- [ ] Run it against the candidate and require PASS.
### Task 2: Implement the runtime-only bot player
**Files:** Create `Game.Logic/IBotGamePlayer.cs`, `Fighting.Server/GameObjects/BotProxyPlayer.cs`; modify project files only where explicit compile lists require it.
- [ ] Implement `IBotGamePlayer.IsBot` and `TakeTurn(PVPGame, Player)`.
- [ ] Implement `BotProxyPlayer` with copied public combat stats/weapon, negative ID, empty persistence/reward methods, and server-side target selection + `GetShootForceAndAngle` + `Shoot`.
- [ ] Compile the runtime-buildable `Game.Logic` and `Fighting.Server` projects; fix interface mismatches without widening scope.

### Task 3: Fill unmatched rooms safely
**Files:** Modify `Fighting.Server/Rooms/ProxyRoom.cs`, `Fighting.Server/Rooms/ProxyRoomMgr.cs`.
- [ ] Add synthetic-room marker and wait-cycle state; synthetic rooms must not send start/stop/remove upstream.
- [ ] Run normal opponent search first. Only if no match exists, room has one player, and game type is not Guild, create one bot once `tick >= BotFillEligibleTick` (5,000 ms after room creation).
- [ ] Start the match through existing `GameMgr.StartBattleGame`; reset wait state when a human match is found.

### Task 4: Drive bot turns and suppress farming
**Files:** Modify `Game.Logic/PVPGame.cs`.
- [ ] Mark bot players loaded immediately during loading.
- [ ] After `SendGameNextTurn`, invoke `IBotGamePlayer.TakeTurn` only for the current bot.
- [ ] Detect any bot in the match and suppress persistent match/guild/offer/ranked progression while preserving the game-over packet and room stop flow.

### Task 5: Build, deploy, and live-verify
**Files:** Runtime-buildable Fight tree plus VPS live `SERVER/Fight` binaries.
- [ ] Build fresh `Game.Logic` and `Fighting.Server`; require zero compile errors and `git diff --check` on canonical source.
- [ ] Backup every live DLL/PDB before replacement; verify SHA-256 after copy.
- [ ] Restart only Fighting/Road components required by the changed binaries and verify service ports/logs.
- [ ] Start real matchmaking with one human: real opponent before threshold => no bot; otherwise exactly one bot appears, takes a shot, match ends, and no bot account/reward progression is persisted.
- [ ] Commit/push canonical branch, open PR, verify exact remote head, and merge only after all available checks plus runtime evidence are green.
