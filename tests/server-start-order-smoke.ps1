$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$launcher = Join-Path $repo 'ops\Start-GunnyServer.ps1'

if (-not (Test-Path $launcher)) {
    throw "Launcher missing: $launcher"
}

$output = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $launcher -PlanOnly 2>&1
if ($LASTEXITCODE -ne 0) {
    throw "PlanOnly failed with exit $LASTEXITCODE`n$($output -join [Environment]::NewLine)"
}

$text = $output -join [Environment]::NewLine
$expected = @(
    'STEP 1 Center.Service.exe port=9202',
    'STEP 2 Fighting.Service.exe port=9208',
    'STEP 3 Road.Service.exe port=9200',
    'VERIFY Road->Fight established'
)

$last = -1
foreach ($token in $expected) {
    $index = $text.IndexOf($token, [StringComparison]::Ordinal)
    if ($index -lt 0) { throw "Missing plan token: $token" }
    if ($index -le $last) { throw "Startup plan is out of order at: $token" }
    $last = $index
}

Write-Host 'SERVER_START_ORDER_SMOKE=PASS'