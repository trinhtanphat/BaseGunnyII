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
foreach ($field in @('TotalKill','TotalHurt','TotalShootCount','TotalCure')) {
    if ($text -notmatch ('pkg\.WriteInt\(p\.' + $field + '\)')) {
        throw ('GAME_OVER packet no longer exposes display stat: ' + $field)
    }
}
Write-Output 'PVP_BOT_DISPLAY_REWARD_SMOKE=PASS'