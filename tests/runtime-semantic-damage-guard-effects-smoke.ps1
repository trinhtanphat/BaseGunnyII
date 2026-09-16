$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$enum=Get-Content (Join-Path $root 'Game.Logic\eEffectType.cs') -Raw
$damagePath=Join-Path $root 'Game.Logic\Effects\DamageEffect.cs'
$guardPath=Join-Path $root 'Game.Logic\Effects\GuardEffect.cs'
$proj=Get-Content (Join-Path $root 'Game.Logic\Game.Logic.csproj') -Raw
function Need([string]$text,[string]$p,[string]$m){if($text -notmatch $p){throw $m}}
Need $enum 'DamageEffect\s*=\s*41' 'DamageEffect enum slot 41 missing'
Need $enum 'GuardEffect\s*=\s*42' 'GuardEffect enum slot 42 missing'
if(!(Test-Path $damagePath)){throw 'DamageEffect source missing'}
if(!(Test-Path $guardPath)){throw 'GuardEffect source missing'}
$d=Get-Content $damagePath -Raw; $g=Get-Content $guardPath -Raw
Need $d 'DamageEffect\s*\(\s*int\s+count\s*\)' 'DamageEffect ctor missing'
Need $d 'base\s*\(\s*eEffectType\.DamageEffect\s*\)' 'DamageEffect enum binding missing'
Need $d 'BeginSelfTurn\s*\+=' 'DamageEffect turn attach missing'
Need $d 'SendPlayerPicture\s*\(\s*living\s*,\s*29\s*,' 'DamageEffect picture 29 missing'
Need $g 'GuardEffect\s*\(\s*int\s+count\s*\)' 'GuardEffect ctor missing'
Need $g 'base\s*\(\s*eEffectType\.GuardEffect\s*\)' 'GuardEffect enum binding missing'
Need $g 'BeginSelfTurn\s*\+=' 'GuardEffect turn attach missing'
Need $g 'SendPlayerPicture\s*\(\s*living\s*,\s*30\s*,' 'GuardEffect picture 30 missing'
Need $proj 'Compile Include="Effects\\DamageEffect\.cs"' 'DamageEffect csproj mapping missing'
Need $proj 'Compile Include="Effects\\GuardEffect\.cs"' 'GuardEffect csproj mapping missing'
Write-Output 'RUNTIME_SEMANTIC_DAMAGE_GUARD_EFFECTS_SMOKE=PASS'