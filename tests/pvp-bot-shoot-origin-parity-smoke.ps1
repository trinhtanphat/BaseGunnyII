$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bot = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
$text = Get-Content $bot -Raw
if ($text -match 'player\.Shoot\(aimX,\s*aimY,\s*force,\s*angle\)') {
    throw 'bot fires from target/aim coordinates instead of the live shoot origin'
}
foreach ($token in @(
    'Point shootPoint = player.GetShootPoint();',
    'player.Shoot(shootPoint.X, shootPoint.Y, force, angle);'
)) {
    if (-not $text.Contains($token)) { throw "bot shoot-origin parity missing: $token" }
}
Write-Host 'PVP_BOT_SHOOT_ORIGIN_PARITY_SMOKE=PASS'
