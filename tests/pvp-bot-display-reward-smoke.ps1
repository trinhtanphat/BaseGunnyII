$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Game.Logic\PVPGame.cs'
$text = Get-Content $file -Raw
if ($text -notmatch '(?s)if\s*\(\s*isBot\s*\).*?p\.GainGP\s*=\s*0\s*;.*?p\.GainOffer\s*=\s*0\s*;.*?p\.CanTakeOut\s*=\s*0\s*;') {
    throw 'Bot participant anti-farm guard for GP/offer/card reward is missing.'
}
if ($text -notmatch '(?s)else\s*\{.*?p\.GainGP\s*=\s*p\.PlayerDetail\.AddGP\(gp\).*?p\.GainOffer\s*=\s*p\.PlayerDetail\.AddOffer\(.*?p\.CanTakeOut\s*=') {
    throw 'Human participant reward/card path is missing when a bot is present.'
}
foreach ($field in @('TotalKill','TotalHurt','TotalShootCount','TotalCure')) {
    if ($text -notmatch ('pkg\.WriteInt\(p\.' + $field + '\)')) {
        throw ('GAME_OVER packet no longer exposes display stat: ' + $field)
    }
}
Write-Output 'PVP_BOT_DISPLAY_REWARD_SMOKE=PASS'
