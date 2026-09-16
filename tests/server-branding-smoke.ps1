$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$targets = @(
  'Game.Service/actions/ConsoleStart.cs',
  'Center.Service/actions/ConsoleStart.cs',
  'Fighting.Service/action/ConsoleStart.cs',
  'recovery/runtime-source/by-service/Road/Road.Service/Game/Service/actions/ConsoleStart.cs',
  'recovery/runtime-source/by-service/center/Center.Service/Game/Service/actions/ConsoleStart.cs',
  'recovery/runtime-source/by-service/Fight/Fighting.Service/Fighting/Service/action/ConsoleStart.cs'
)
foreach ($relative in $targets) {
  $path = Join-Path $root $relative
  $text = [IO.File]::ReadAllText($path)
  if ($text -notmatch [regex]::Escape('GUNNY II fix by trinhtanphat!')) { throw "Missing canonical trinhtanphat banner: $relative" }
  if ($text -match 'hoang7625|edit and build by Trminhpc') { throw "Legacy server credit remains: $relative" }
}
Write-Host 'SERVER_BRANDING_SMOKE=PASS'
