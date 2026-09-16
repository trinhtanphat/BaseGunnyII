$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$helper = Join-Path $root 'Fighting.Server\GameObjects\BotAimTrajectory.cs'
$bot = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
if (-not (Test-Path $helper)) { throw 'terrain-aware trajectory helper missing' }
$botText = Get-Content $bot -Raw
if (-not $botText.Contains('BotAimTrajectory.IsViable')) { throw 'bot aim does not reject terrain-blocked candidates' }
$helperText = Get-Content $helper -Raw
if (-not $botText.Contains('target.Distance(new Point(impactX, impactY))')) { throw 'bot splash validation does not use runtime Living.Distance' }
if (-not $helperText.Contains('targetDamageDistance(px, py) < blastRadius')) { throw 'trajectory helper does not use runtime splash distance callback' }
if ($helperText.Contains('BoundDistance(') -or $helperText.Contains('DistanceToRectangleCenter(')) { throw 'trajectory helper must not approximate runtime splash distance' }
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v3.5\csc.exe'
if (-not (Test-Path $csc)) { throw 'NET35 csc.exe missing' }
$temp = Join-Path $env:TEMP ('bot-trajectory-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
try {
    $exe = Join-Path $temp 'probe.exe'
    & $csc /nologo /target:exe /out:$exe /r:System.Drawing.dll $helper (Join-Path $PSScriptRoot 'fixtures\BotAimTrajectoryHarness.cs')
    if ($LASTEXITCODE -ne 0) { throw "trajectory harness compile failed: $LASTEXITCODE" }
    & $exe
    if ($LASTEXITCODE -ne 0) { throw "trajectory harness failed: $LASTEXITCODE" }
} finally {
    Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Output 'PVP_BOT_TRAJECTORY_SMOKE=PASS'