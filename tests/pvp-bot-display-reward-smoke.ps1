$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Game.Logic\PVPGame.cs'
$text = Get-Content $file -Raw
if ($text -notmatch '(?s)if\s*\(\s*hasBot\s*\).*?gp\s*=\s*0\s*;.*?p\.GainGP\s*=\s*0\s*;.*?p\.GainOffer\s*=\s*0\s*;') {
    throw 'NPC match anti-farm guard for persisted GP/offer is missing.'
}
if ($text -notmatch 'p\.CanTakeOut\s*=\s*hasBot\s*\?\s*0\s*:') {
    throw 'NPC match card reward guard is missing.'
}
if ($text -notmatch 'int\s+displayGp\s*=\s*gp\s*;') {
    throw 'Calculated GP is not preserved for result display before anti-farm zeroing.'
}
if ($text -notmatch 'pkg\.WriteInt\(\s*hasBot\s*\?\s*displayGp\s*:\s*p\.GainGP\s*\)') {
    throw 'GAME_OVER packet does not expose display-only GP for NPC matches.'
}
Write-Output 'PVP_BOT_DISPLAY_REWARD_SMOKE=PASS'