$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'recovery\runtime-source\by-service\Fight\SqlDataProvider\SqlDataProvider\Data\PlayerInfo.cs'
$text = Get-Content $file -Raw
$method = [regex]::Match($text, '(?s)public bool bit\(int param1\)\s*\{(?<body>.*?)\r?\n\t\}')
if (-not $method.Success) { throw 'Fight recovered PlayerInfo.bit(int) not found.' }
$body = $method.Groups['body'].Value
if ($body -notmatch '_weaklessGuildProgress\s*==\s*null') {
    throw 'PlayerInfo.bit must tolerate null weak-guild progress.'
}
if ($body -notmatch 'num\s*<\s*0\s*\|\|\s*num\s*>=\s*_weaklessGuildProgress\.Length') {
    throw 'PlayerInfo.bit must reject weak-guild byte indexes outside the available array.'
}
if ($body -notmatch 'return\s+false\s*;') {
    throw 'Out-of-range weak-guild progress must be treated as unfinished.'
}
Write-Output 'PLAYERINFO_WEAKGUILD_BOUNDS_SMOKE=PASS'