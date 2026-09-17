$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$path = Join-Path $root 'Bussiness\ConsortiaBussiness.cs'
$text = Get-Content $path -Raw
$start = $text.IndexOf('public bool ScanConsortia(ref string noticeID)')
if ($start -lt 0) { throw 'ScanConsortia method missing' }
$tail = $text.Substring($start)
$end = $tail.IndexOf('#endregion')
if ($end -lt 0) { throw 'ScanConsortia region boundary missing' }
$method = $tail.Substring(0,$end)
if ($method -match '\(int\)para\[1\]\.Value') { throw 'ScanConsortia still unboxes @Result without null/DBNull guard' }
if ($method -match 'para\[0\]\.Value\.ToString\(\)') { throw 'ScanConsortia still calls ToString on @NoticeID without null/DBNull guard' }
if ($method -notmatch 'RunProcedure\("SP_Consortia_Scan",\s*para\)') { throw 'SP_Consortia_Scan call missing' }
if ($method -notmatch 'DBNull\.Value') { throw 'ScanConsortia must explicitly handle DBNull output values' }
if ($method -notmatch 'ScanConsortia"') { throw 'ScanConsortia-specific error label missing' }
Write-Host 'CENTER_SCANCONSORTIA_NULL_SAFETY_SMOKE=PASS'