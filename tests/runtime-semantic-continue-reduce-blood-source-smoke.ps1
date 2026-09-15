$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$effect = Get-Content (Join-Path $root 'Game.Logic\Effects\ContinueReduceBloodEffect.cs') -Raw
function Assert-Match([string]$text,[string]$pattern,[string]$message) {
    if ($text -notmatch $pattern) { throw $message }
}
Assert-Match $effect 'ContinueReduceBloodEffect\s*\(\s*int\s+count\s*,\s*int\s+blood\s*\)' 'legacy 2-arg ctor missing'
Assert-Match $effect 'ContinueReduceBloodEffect\s*\(\s*int\s+count\s*,\s*int\s+blood\s*,\s*Living\s+liv\s*\)' 'source-aware 3-arg ctor missing'
Assert-Match $effect 'private\s+bool\s+m_sourceAware' 'source-aware discriminator missing'
Assert-Match $effect 'if\s*\(\s*m_sourceAware\s*\)' 'source-aware tick branch missing'
Assert-Match $effect 'living\.AddBlood\s*\(\s*-m_blood\s*,\s*1\s*\)' 'typed periodic damage missing'
Assert-Match $effect 'OnKillingLiving\s*\(' 'source kill-credit callback missing'
Assert-Match $effect 'p\.AddBlood\s*\(\s*m_blood\s*\)' 'legacy 2-arg tick behavior missing'
Write-Output 'RUNTIME_SEMANTIC_CONTINUE_REDUCE_BLOOD_SOURCE_SMOKE=PASS'
