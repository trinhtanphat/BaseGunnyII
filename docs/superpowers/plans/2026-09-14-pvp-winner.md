# PvP Winner Resolution Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Prevent zero-HP/all-dead PvP matches from being awarded to `CurrentPlayer.Team`.

**Architecture:** Extract winner selection into a pure resolver that accepts team/living/blood state. `PVPGame.GameOver()` projects fight players into that resolver and treats `-1` as draw/no winner.

**Tech Stack:** C# .NET Framework 4.0, Game.Logic, MSBuild v4.

**Spec:** `docs/superpowers/specs/2026-09-14-pvp-winner-design.md`

## Global Constraints
- No packet-layout change.
- No PvE result change.
- No winner-only guild logic when `winTeam == -1`.

### Task 1: Resolver RED→GREEN
**Files:** Create `Game.Logic/PvpWinnerResolver.cs`, `ops/test-pvp-winner.ps1`; modify `Game.Logic/Game.Logic.csproj` and `Game.Logic/PVPGame.cs`.
- [ ] Write a standalone C# smoke that requires red-alive→1, blue-alive→2, both-dead→-1, HP0-stale-living→-1.
- [ ] Run smoke before implementation and confirm RED because `PvpWinnerResolver` does not exist.
- [ ] Implement `PvpWinnerState` and `PvpWinnerResolver.Resolve(...)`; integrate into `GameOver()`.
- [ ] Guard `CalculateGuildMatchResult`/win reward paths for `winTeam == -1`.
- [ ] Run focused smoke, `GameServer.sln` build, `FightingServer.sln` build, and `git diff --check`.
- [ ] Commit `fix(pvp): resolve winner only from surviving teams`.

### Task 2: Runtime result check
- [ ] Deploy server binaries from the verified source build with backups.
- [ ] Run one normal win and one forced zero-HP/draw scenario; verify GAME_OVER flags match the resolver and no dead player is reported as winner.