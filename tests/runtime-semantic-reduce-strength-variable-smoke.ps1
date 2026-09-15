$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$text = Get-Content (Join-Path $root 'Game.Logic\Effects\ReduceStrengthEffect.cs') -Raw
function Assert-Match([string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match 'ReduceStrengthEffect\s*\(\s*int\s+count\s*\)' 'legacy 1-arg ctor missing'
Assert-Match 'ReduceStrengthEffect\s*\(\s*int\s+count\s*,\s*int\s+reduce\s*\)' 'variable 2-arg ctor missing'
Assert-Match 'm_reduce\s*=\s*50\s*;' 'legacy 50-energy default missing'
Assert-Match 'Energy\s*-=\s*m_reduce\s*;' 'variable energy reduction missing'
Write-Output 'RUNTIME_SEMANTIC_REDUCE_STRENGTH_VARIABLE_SMOKE=PASS'
