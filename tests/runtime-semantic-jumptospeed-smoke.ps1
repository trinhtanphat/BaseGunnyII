$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$living = Get-Content -Raw -Path (Join-Path $root 'Game.Logic\Phy\Object\Living.cs')

function Assert-Match([string]$text, [string]$pattern, [string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}

Assert-Match $living 'public bool JumpToSpeed\(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback\)' 'Living.JumpToSpeed overload missing'
Assert-Match $living 'FindYLineNotEmptyPoint\(x, y\)' 'JumpToSpeed must resolve terrain Y'
Assert-Match $living 'new LivingJumpAction\(this, p\.X, p\.Y, speed, action, delay, type, callback\)' 'JumpToSpeed must enqueue LivingJumpAction'
Assert-Match $living 'public bool JumpTo\(int x, int y, string action, int delay, int type\)' 'legacy JumpTo overload missing'
Write-Host 'RUNTIME_SEMANTIC_JUMPTOSPEED_SMOKE=PASS'
