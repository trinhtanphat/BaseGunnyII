$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$src = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
function Assert-Match([string]$text,[string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match $src 'private\s+int\s+m_doAction\s*;' 'm_doAction field missing'
Assert-Match $src 'm_doAction\s*=\s*-1\s*;' 'm_doAction default -1 missing'
Assert-Match $src 'public\s+int\s+DoAction\s*\{[\s\S]*?get\s*\{\s*return\s+m_doAction\s*;\s*\}[\s\S]*?set' 'DoAction property missing'
Write-Output 'RUNTIME_SEMANTIC_LIVING_DO_ACTION_SMOKE=PASS'
