# PvP Winner Resolution Fix Design

## Problem
`PVPGame.GameOver()` picks the first living player's team, but if no player is living it falls back to `CurrentPlayer.Team`. A match where everyone is dead can therefore award a win to the current-turn team.

## Design
Determine the winner only from players who are both `IsLiving` and `Blood > 0`. If exactly one team has such a player, that team wins. If neither team has a survivor, `winTeam` remains `-1` and the match is a draw: every player's `isWin` flag is false and winner-only guild logic is skipped.

## Compatibility
Do not change damage calculation, death transitions, reward packet layout, room type semantics, or PvE result logic.

## Success criteria
- Red dead / blue alive => blue wins.
- Blue dead / red alive => red wins.
- Both teams dead or HP=0 => no winner.
- No null dereference in guild result calculation when `winTeam == -1`.
- Existing normal PvP game-over flow remains unchanged.