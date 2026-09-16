$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$path = Join-Path $root 'ops\Start-GunnyServer.ps1'
$text = [IO.File]::ReadAllText($path)
$required = @(
  'Global\GunnyServerStartOrder',
  'MutexSecurity',
  'AuthenticatedUserSid',
  'MutexRights]::Synchronize',
  'MutexRights]::Modify',
  'MutexAccessRule'
)
foreach ($needle in $required) {
  if (-not $text.Contains($needle)) { throw "Missing cross-account mutex ACL guard: $needle" }
}
Write-Host 'SERVER_START_MUTEX_ACL_SMOKE=PASS'
