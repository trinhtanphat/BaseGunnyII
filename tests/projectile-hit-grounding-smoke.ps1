$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$playerPath = Join-Path $root 'Game.Logic\Phy\Object\Player.cs'
$bombPath = Join-Path $root 'Game.Logic\Phy\Object\SimpleBomb.cs'
$player = Get-Content -Raw -Path $playerPath
$bomb = Get-Content -Raw -Path $bombPath
function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw "PROJECTILE_HIT_GROUNDING_FAIL: $message" }
}
Assert-True ($bomb -match 'p\.StartMoving\(\(int\)\(\(m_lifeTime \+ 1\) \* 1000\), 12\)') 'projectile hit must exercise delayed player grounding'
$signature = 'public override void StartMoving(int delay, int speed)'
$start = $player.IndexOf($signature)
Assert-True ($start -ge 0) 'delayed Player.StartMoving override missing'
$next = $player.IndexOf('public void StartGhostMoving()', $start)
Assert-True ($next -gt $start) 'cannot isolate delayed StartMoving body'
$body = $player.Substring($start, $next - $start)
Assert-True ($body -match 'base\.StartMoving\(delay, speed\);') 'delayed grounding must delegate to Living fall scheduling'
$beforeBase = $body.Substring(0, $body.IndexOf('base.StartMoving(delay, speed);'))
Assert-True ($beforeBase -notmatch 'FindYLineNotEmptyPoint') 'must not resolve/snap to terrain before delayed fall'
Assert-True ($beforeBase -notmatch 'm_[xy]\s*=') 'must not mutate player coordinates before delayed fall'
Write-Host 'PROJECTILE_HIT_GROUNDING_SMOKE=PASS'