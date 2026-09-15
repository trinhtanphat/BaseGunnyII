$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$living = Get-Content -Raw -Path (Join-Path $root 'Game.Logic\Phy\Object\Living.cs')
function Assert-Match([string]$text,[string]$pattern,[string]$message){ if($text -notmatch $pattern){ throw $message } }
Assert-Match $living 'public bool FallFromTo\(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback\)' 'Living.FallFromTo missing'
Assert-Match $living 'new LivingFallingAction\(this, x, y, speed, action, delay, type, callback\)' 'FallFromTo must target explicit coordinates'
Assert-Match $living 'public bool FallFrom\(int x, int y, string action, int delay, int type, int speed\)' 'legacy FallFrom overload missing'
Write-Host 'RUNTIME_SEMANTIC_FALLFROMTO_SMOKE=PASS'
