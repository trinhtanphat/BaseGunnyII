$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$configs = @(
    'Center.Server/config/logconfig.xml',
    'Fighting.Server/Config/logconfig.xml',
    'Fighting.Service/logconfig.xml',
    'Game.Server/Config/logconfig.xml',
    'Game.Service/logconfig.xml'
)
foreach ($relative in $configs) {
    $path = Join-Path $root $relative
    [xml]$xml = Get-Content -LiteralPath $path -Raw
    $appender = @($xml.log4net.appender) | Where-Object { $_.name -eq 'ErrorLogFile' }
    if (-not $appender) { throw "Missing ErrorLogFile appender: $relative" }
    if ($appender.appendToFile.value -ne 'true') {
        throw "ErrorLogFile must append across restarts to avoid rollover gaps: $relative"
    }
    if ([int]$appender.maxSizeRollBackups.value -lt 1) {
        throw "ErrorLogFile must retain at least one size backup: $relative"
    }
}
Write-Host 'LOG4NET_ERROR_ROLLOVER_SMOKE=PASS'
