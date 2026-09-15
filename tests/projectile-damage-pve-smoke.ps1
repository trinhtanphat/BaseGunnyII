$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$simpleBombPath = Join-Path $root 'Game.Logic\Phy\Object\SimpleBomb.cs'
$livingPath = Join-Path $root 'Game.Logic\Phy\Object\Living.cs'
$simpleBomb = Get-Content $simpleBombPath -Raw
$living = Get-Content $livingPath -Raw
function Require([bool]$ok, [string]$message) {
    if (-not $ok) { throw $message }
}
Require ($simpleBomb.Contains('m_owner.OnTakedDamage(m_owner, ref damage, ref critical);')) 'projectile damage event must keep damage and critical as independent refs'
Require (-not $simpleBomb.Contains('m_owner.OnTakedDamage(m_owner, ref damage, ref damage);')) 'projectile damage event aliases damage into critical'
Require ($simpleBomb.Contains('int resolvedDamage = (int)damage;')) 'projectile damage must resolve fractional positive damage explicitly'
Require ($simpleBomb.Contains('return resolvedDamage > 0 ? resolvedDamage : 1;')) 'an in-radius positive hit must not truncate to zero'
Require ($simpleBomb.Contains('p.TakeDamage(m_owner, ref damage, ref critical')) 'target HP must be updated per projectile hit'
Require ($simpleBomb.Contains('if (p is SimpleBoss)')) 'PvE boss hit branch missing'
Require ($simpleBomb.Contains('((PVEGame)m_game).OnShooted();')) 'PvE mission hit callback missing'
Require ($living.Contains('pkg.WriteInt(bomb.Actions.Count);')) 'FIRE packet must carry each projectile action list'
Require ($living.Contains('foreach (BombAction ac in bomb.Actions)')) 'FIRE packet must serialize per-projectile impact actions'
Write-Host 'PROJECTILE_DAMAGE_PVE_SMOKE=PASS'
