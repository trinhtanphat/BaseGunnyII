$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$simpleBomb = Join-Path $root 'Game.Logic\Phy\Object\SimpleBomb.cs'
if (-not (Test-Path $simpleBomb)) { throw "missing SimpleBomb source: $simpleBomb" }
$text = Get-Content $simpleBomb -Raw
$bad = 'm_owner\.OnTakedDamage\(m_owner,\s*ref\s+damage,\s*ref\s+damage\)'
$good = 'm_owner\.OnTakedDamage\(m_owner,\s*ref\s+damage,\s*ref\s+critical\)'
if ($text -match $bad) { throw 'projectile damage aliases damage into the critical ref argument' }
if ($text -notmatch $good) { throw 'projectile damage must pass the critical accumulator separately' }
$kill = 'ActionType\.KILL_PLAYER,\s*p\.Id,\s*damage\s*\+\s*critical'
if ($text -notmatch $kill) { throw 'projectile result packet must publish damage + critical' }
Write-Host 'PROJECTILE_DAMAGE_REF_SMOKE=PASS'
