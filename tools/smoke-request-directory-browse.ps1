param([string]$RequestRoot = (Join-Path (Split-Path $PSScriptRoot -Parent) 'Tank.Request'))
$ErrorActionPreference = 'Stop'
$configPath = Join-Path $RequestRoot 'Web.config'
if (!(Test-Path $configPath)) { throw 'Tank.Request/Web.config is missing' }
[xml]$config = Get-Content -Raw -LiteralPath $configPath
$node = $config.configuration.'system.webServer'.directoryBrowse
if ($null -eq $node) { throw 'directoryBrowse setting is missing' }
if ([string]$node.enabled -ne 'false') { throw "directoryBrowse must be false, got '$($node.enabled)'" }
Write-Output 'REQUEST_DIRECTORY_BROWSE_SMOKE=PASS'
