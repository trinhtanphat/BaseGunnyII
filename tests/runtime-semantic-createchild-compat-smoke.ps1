$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$living = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
$npc = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\SimpleNpc.cs') -Raw
$pve = Get-Content (Join-Path $root 'Game.Logic\PVEGame.cs') -Raw
$boss = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\SimpleBoss.cs') -Raw
function Assert-Match([string]$text,[string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match $living 'public\s+LivingConfig\s+Config' 'Living.Config missing'
Assert-Match $npc 'SimpleNpc\(int id, BaseGame game, NpcInfo npcInfo, int type, int direction\)' 'directional SimpleNpc ctor missing'
Assert-Match $npc 'npcInfo\.Immunity, direction\)' 'directional SimpleNpc ctor does not pass direction to Living'
Assert-Match $pve 'public\s+LivingConfig\s+BaseLivingConfig\(\)' 'BaseLivingConfig missing'
Assert-Match $pve 'CreateNpc\(int npcId, int x, int y, int type, int direction\)' 'directional CreateNpc missing'
Assert-Match $boss 'CreateChild\(int id, int x, int y, int disToSecond, int maxCount, int direction\)' 'directional CreateChild missing'
Assert-Match $npc 'SimpleNpc\(int id, BaseGame game, NpcInfo npcInfo, int type\)' 'legacy SimpleNpc ctor missing'
Assert-Match $pve 'CreateNpc\(int npcId, int x, int y, int type\)' 'legacy CreateNpc missing'
Assert-Match $boss 'CreateChild\(int id, int x, int y, int disToSecond, int maxCount\)' 'legacy CreateChild missing'
Assert-Match $pve 'config\.isBotom\s*=\s*1' 'BaseLivingConfig bottom default missing'
Assert-Match $pve 'config\.IsTurn\s*=\s*true' 'BaseLivingConfig turn default missing'
Assert-Match $pve 'config\.ReduceBloodStart\s*=\s*1' 'BaseLivingConfig blood default missing'
Write-Host 'RUNTIME_SEMANTIC_CREATECHILD_SMOKE=PASS'
