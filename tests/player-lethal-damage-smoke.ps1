$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Game.Logic\Phy\Object\Player.cs'
$text = Get-Content $file -Raw
if ($text -match 'damageAmount\s*=\s*m_blood\s*-\s*1') {
    throw 'Player.TakeDamage still clamps lethal self/team damage to 1 HP.'
}
if ($text -notmatch 'result\s*=\s*base\.TakeDamage\(source,\s*ref damageAmount,\s*ref criticalAmount,\s*msg\)') {
    throw 'Player.TakeDamage no longer delegates final lethal handling to Living.TakeDamage.'
}
$living = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
if ($living -notmatch 'if\s*\(m_blood\s*<=\s*0\)\s*\{\s*Die\(\)') {
    throw 'Living.TakeDamage is missing Blood <= 0 => Die().'
}
Write-Output 'PLAYER_LETHAL_DAMAGE_SMOKE=PASS'