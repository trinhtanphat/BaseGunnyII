$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bot = Get-Content (Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs') -Raw
$bomb = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\SimpleBomb.cs') -Raw

if (-not $bomb.Contains('double distance = target.Distance(p);')) {
    throw 'runtime SimpleBomb.MakeDamage no longer uses Living.Distance'
}
if (-not $bot.Contains('return target.Distance(new Point(impactX, impactY));')) {
    throw 'bot splash planner must use the same Living.Distance geometry as SimpleBomb.MakeDamage'
}
if ($bot.Contains('target.BoundDistance(new Point(impactX, impactY))')) {
    throw 'bot splash planner still uses BoundDistance and can diverge from runtime damage geometry'
}
Write-Output 'PVP_BOT_SPLASH_DISTANCE_PARITY_SMOKE=PASS'
