$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$launcher = Join-Path $repo 'ops\Start-GunnyServer.ps1'
$text = [IO.File]::ReadAllText($launcher)
$match = [regex]::Match($text, '\[int\]\$TimeoutSeconds\s*=\s*(\d+)')
if (-not $match.Success) { throw 'TimeoutSeconds default missing' }
$seconds = [int]$match.Groups[1].Value
if ($seconds -lt 90) { throw "cold-start timeout too short: $seconds" }
Write-Host "SERVER_START_SLOW_INIT_SMOKE=PASS timeout=$seconds"
