$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$enum=Get-Content (Join-Path $root 'Game.Logic\eEffectType.cs') -Raw
$path=Join-Path $root 'Game.Logic\Effects\ContinueReduceBlood.cs'
$proj=Get-Content (Join-Path $root 'Game.Logic\Game.Logic.csproj') -Raw
function Need([string]$text,[string]$p,[string]$m){ if($text -notmatch $p){ throw $m } }
Need $enum 'ContinueReduceBlood\s*=\s*40' 'ContinueReduceBlood enum slot 40 missing'
if(!(Test-Path $path)){ throw 'ContinueReduceBlood source missing' }
$text=Get-Content $path -Raw
Need $text 'class\s+ContinueReduceBlood\s*:\s*AbstractEffect' 'ContinueReduceBlood type missing'
Need $text 'ContinueReduceBlood\s*\(\s*int\s+count\s*,\s*int\s+blood\s*,\s*Living\s+liv\s*\)' 'ContinueReduceBlood ctor missing'
Need $text 'base\s*\(\s*eEffectType\.ContinueReduceBlood\s*\)' 'ContinueReduceBlood enum binding missing'
Need $text 'BeginSelfTurn\s*\+=' 'BeginSelfTurn attach missing'
Need $text 'SendPlayerPicture\s*\(\s*living\s*,\s*28\s*,' 'picture attach missing'
Need $text 'AddBlood\s*\(\s*-m_blood\s*,\s*1\s*\)' 'typed periodic damage missing'
Need $text 'OnKillingLiving\s*\(' 'kill credit missing'
Need $proj 'Compile Include="Effects\\ContinueReduceBlood\.cs"' 'ContinueReduceBlood csproj mapping missing'
Write-Output 'RUNTIME_SEMANTIC_CONTINUE_REDUCE_BLOOD_SMOKE=PASS'
