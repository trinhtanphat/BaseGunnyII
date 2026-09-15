$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$pvp = Join-Path $root 'Game.Logic\PVPGame.cs'
if (-not (Test-Path $pvp)) { throw "missing PVPGame.cs: $pvp" }
$text = Get-Content $pvp -Raw

if ($text -notmatch 'eTankCmdType\.GAME_OVER') {
  throw 'PVP game-over packet is no longer emitted'
}
if ($text -notmatch 'if\s*\(\s*!p\.IsLiving\s*\|\|\s*p\.Blood\s*<=\s*0\s*\)') {
  throw 'PVP winner scan must ignore dead or zero-HP players'
}
if ($text -notmatch 'PvpWinnerResolver\.Resolve\(\s*redAlive\s*,\s*blueAlive\s*\)') {
  throw 'PVP GameOver no longer resolves the surviving team through PvpWinnerResolver'
}
if ($text -notmatch 'WriteBoolean\(\s*p\.Team\s*==\s*winTeam\s*\)') {
  throw 'GAME_OVER packet winner flag is not wired to winTeam'
}
if ($text -notmatch 'PlayerDetail\.OnGameOver\(\s*this\s*,\s*p\.Team\s*==\s*winTeam\s*,\s*p\.GainGP\s*\)') {
  throw 'PlayerDetail.OnGameOver winner callback is not wired to winTeam'
}
if ($text -notmatch '(?s)if\s*\(\s*hasBot\s*\).*?p\.GainGP\s*=\s*0\s*;.*?p\.GainOffer\s*=\s*0\s*;') {
  throw 'NPC matches must still suppress farmable GP/offer rewards'
}
if ($text -notmatch 'p\.CanTakeOut\s*=\s*hasBot\s*\?\s*0\s*:') {
  throw 'NPC matches must not expose card take-out rewards'
}
Write-Host 'PVP_GAMEOVER_WIRING_SMOKE=PASS'
