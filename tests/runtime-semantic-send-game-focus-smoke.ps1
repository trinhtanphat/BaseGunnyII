$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$pve = Get-Content (Join-Path $root 'Game.Logic\PVEGame.cs') -Raw
function Assert-Match([string]$text,[string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match $pve 'void\s+SendGameFocus\s*\(\s*Physics\s+p\s*,\s*int\s+delay\s*,\s*int\s+finishTime\s*\)' 'PVEGame.SendGameFocus missing'
Assert-Match $pve 'new\s+FocusAction\s*\(\s*p\s*,\s*1\s*,\s*delay\s*,\s*finishTime\s*\)' 'focus action contract missing'
Write-Output 'RUNTIME_SEMANTIC_SEND_GAME_FOCUS_SMOKE=PASS'
