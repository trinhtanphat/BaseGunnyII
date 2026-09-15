$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$npcPath = Join-Path $root 'SqlDataProvider\Data\NPCInfo.cs'
$producePath = Join-Path $root 'Bussiness\ProduceBussiness.cs'
$npc = Get-Content -Raw $npcPath
$produce = Get-Content -Raw $producePath

function Assert-Match([string]$text, [string]$pattern, [string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}

Assert-Match $npc 'public\s+int\s+speed\s*\{\s*get;\s*set;\s*\}' 'NpcInfo.speed property missing'
Assert-Match $produce 'info\.speed\s*=\s*\(int\)reader\["speed"\]' 'GetAllNPCInfo does not hydrate speed'
Write-Output 'RUNTIME_SEMANTIC_NPC_SPEED_SMOKE=PASS'
