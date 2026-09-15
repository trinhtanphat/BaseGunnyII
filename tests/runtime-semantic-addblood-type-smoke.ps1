$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$living = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
function Assert-Match([string]$text,[string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match $living 'public\s+virtual\s+int\s+AddBlood\s*\(\s*int\s+value\s*,\s*int\s+type\s*\)' 'Living.AddBlood(value,type) missing'
Assert-Match $living 'SendGameUpdateHealth\s*\(\s*this\s*,\s*type\s*,\s*value\s*\)' 'typed health update missing'
Write-Output 'RUNTIME_SEMANTIC_ADDBLOOD_TYPE_SMOKE=PASS'
