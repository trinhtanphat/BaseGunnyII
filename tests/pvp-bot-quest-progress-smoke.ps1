$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$pvp = Join-Path $root 'Game.Logic\PVPGame.cs'
$text = Get-Content $pvp -Raw
if ($text -match 'if\s*\(\s*!hasBot\s*\)\s*\{\s*foreach\s*\(Player\s+p\s+in\s+players\)\s*\{\s*p\.PlayerDetail\.OnGameOver') {
  throw 'Bot match still suppresses OnGameOver for the real player, so battle quests cannot progress'
}
if ($text -notmatch 'foreach\s*\(Player\s+p\s+in\s+players\)\s*\{[^}]*PlayerDetail\.OnGameOver') {
  throw 'PVP game-over does not notify player details'
}
Write-Host 'PVP_BOT_QUEST_PROGRESS_SMOKE=PASS'
