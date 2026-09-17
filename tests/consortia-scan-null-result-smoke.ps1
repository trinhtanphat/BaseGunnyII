$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Bussiness\ConsortiaBussiness.cs'
$text = Get-Content $file -Raw
$method = [regex]::Match($text, '(?s)public bool ScanConsortia\(ref string noticeID\)\s*\{(?<body>.*?)\r?\n\s*#endregion')
if (-not $method.Success) { throw 'ConsortiaBussiness.ScanConsortia not found.' }
$body = $method.Groups['body'].Value
if ($body -notmatch 'if\s*\(\s*!db\.RunProcedure\("SP_Consortia_Scan",\s*para\)\s*\)') {
    throw 'ScanConsortia must stop when SP_Consortia_Scan execution fails.'
}
if ($body -notmatch 'para\[1\]\.Value\s*==\s*null.*?DBNull\.Value') {
    throw 'ScanConsortia must guard a missing stored-procedure return value.'
}
if ($body -notmatch 'para\[0\]\.Value\s*==\s*null.*?DBNull\.Value') {
    throw 'ScanConsortia must normalize a missing NoticeID output.'
}
Write-Output 'CONSORTIA_SCAN_NULL_RESULT_SMOKE=PASS'
