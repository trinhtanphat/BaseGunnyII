$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$launcher = Join-Path $root 'ops\Start-GunnyServer.ps1'
$text = [IO.File]::ReadAllText($launcher)
function Assert-Has([string]$needle,[string]$message) {
    if (-not $text.Contains($needle)) { throw $message }
}
Assert-Has 'System.Threading.Mutex' 'single-instance mutex guard missing'
Assert-Has 'Global\GunnyServerStartOrder' 'global Gunny startup mutex name missing'
Assert-Has 'WaitOne(0)' 'non-blocking startup lock acquisition missing'
Assert-Has 'STOP_DUPLICATE' 'stale duplicate service cleanup missing'
Assert-Has 'OwningProcess' 'listener PID ownership validation missing'
Write-Host 'SERVER_START_RACE_SMOKE=PASS'