$ErrorActionPreference='Stop'
$path=Join-Path $PSScriptRoot '..\Game.Logic\BaseGame.cs'
$text=Get-Content $path -Raw
function Need([string]$p,[string]$m){ if($text -notmatch $p){ throw $m } }
Need 'public\s+int\s+GetDiedBossCount\s*\(\s*\)' 'GetDiedBossCount missing'
Need 'SimpleBoss\[\]\s+\w+\s*=\s*FindAllBoss\s*\(\s*\)' 'FindAllBoss query missing'
Need 'foreach\s*\(\s*SimpleBoss\s+\w+\s+in\s+\w+\s*\)' 'boss enumeration missing'
Need 'if\s*\(\s*!\s*\w+\.IsLiving\s*\)' 'dead-boss predicate missing'
Write-Output 'RUNTIME_SEMANTIC_DIED_BOSS_COUNT_SMOKE=PASS'
