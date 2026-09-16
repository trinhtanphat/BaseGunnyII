$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$living=Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
$boss=Get-Content (Join-Path $root 'Game.Logic\Phy\Object\SimpleBoss.cs') -Raw
$pve=Get-Content (Join-Path $root 'Game.Logic\PVEGame.cs') -Raw
$base=Get-Content (Join-Path $root 'Game.Logic\BaseGame.cs') -Raw
$types=Get-Content (Join-Path $root 'Game.Logic\eLivingType.cs') -Raw
function Need([string]$t,[string]$p,[string]$m){ if($t -notmatch $p){ throw $m } }
Need $types 'ClearEnemy\s*,\s*\r?\n\s*BossSpecialDie' 'recovered living type tail missing'
Need $living 'private\s+string\s+m_action\s*;' 'Living action state missing'
Need $living 'public\s+string\s+ActionStr' 'Living.ActionStr missing'
Need $living 'm_action\s*=\s*""\s*;' 'Living action default missing'
Need $base 'WriteString\s*\(\s*living\.ActionStr\s*\)' 'ADD_LIVING action payload missing'
Need $boss 'List<SimpleBoss>\s+m_boss' 'SimpleBoss boss list missing'
Need $boss 'CurrentLivingBossNum' 'SimpleBoss living boss count missing'
Need $boss 'SimpleBoss\s*\(\s*int\s+id.*string\s+actions\s*\)' 'action-aware SimpleBoss ctor missing'
Need $boss 'case\s+1\s*:\s*\r?\n\s*(?:base\.)?Type\s*=\s*eLivingType\.ClearEnemy' 'ClearEnemy type mapping missing'
Need $boss 'ActionStr\s*=\s*actions' 'SimpleBoss action assignment missing'
Need $boss 'CreateBoss\s*\(\s*int\s+id\s*,\s*int\s+x\s*,\s*int\s+y\s*,\s*int\s+direction\s*,\s*int\s+disToSecond\s*,\s*int\s+maxCount\s*,\s*string\s+action\s*\)' 'SimpleBoss CreateBoss facade missing'
Need $boss 'maxCount\s*-\s*CurrentLivingNpcNum\s*>=\s*2' 'recovered spawn-count branch missing'
Need $pve 'CreateBoss\s*\(\s*int\s+npcId\s*,\s*int\s+x\s*,\s*int\s+y\s*,\s*int\s+direction\s*,\s*int\s+type\s*,\s*string\s+action\s*\)' 'PVE action-aware CreateBoss missing'
Need $pve 'new\s+SimpleBoss\s*\([^;]*type\s*,\s*action\s*\)' 'PVE action-aware constructor call missing'
Write-Output 'RUNTIME_SEMANTIC_SIMPLEBOSS_CREATEBOSS_SMOKE=PASS'
