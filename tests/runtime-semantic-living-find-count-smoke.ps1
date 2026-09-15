$ErrorActionPreference = 'Stop'
$path = Join-Path $PSScriptRoot '..\Game.Logic\Phy\Object\Living.cs'
$text = Get-Content $path -Raw
function Need([string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Need 'private\s+int\s+m_FindCount\s*;' 'm_FindCount field missing'
Need 'public\s+int\s+FindCount\s*\{[\s\S]*?get\s*\{\s*return\s+m_FindCount\s*;\s*\}[\s\S]*?set\s*\{\s*m_FindCount\s*=\s*value\s*;\s*\}' 'FindCount read/write property missing'
Write-Output 'RUNTIME_SEMANTIC_LIVING_FIND_COUNT_SMOKE=PASS'
