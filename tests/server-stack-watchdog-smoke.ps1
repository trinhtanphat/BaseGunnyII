$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$wrapper = Join-Path $root 'ops\start-gunny-stack.ps1'
if (-not (Test-Path $wrapper)) { throw "watchdog wrapper missing: $wrapper" }
$text = [IO.File]::ReadAllText($wrapper)
function Require([string]$needle,[string]$message) {
    if (-not $text.Contains($needle)) { throw $message }
}
Require 'C:\Gunny\_ops\Start-GunnyServer.ps1' 'wrapper does not delegate to canonical launcher'
Require 'Another Gunny server startup is already running' 'wrapper does not handle startup mutex contention'
Require 'GUNNY_STACK_READY ports=2009,9200,9202,9208' 'wrapper readiness marker missing'
if ($text.Contains('Start-Process -FilePath $exe')) { throw 'legacy direct service spawning remains in wrapper' }
if ($text.Contains('function EnsureGame')) { throw 'legacy EnsureGame path remains in wrapper' }
Write-Host 'SERVER_STACK_WATCHDOG_SMOKE=PASS'
