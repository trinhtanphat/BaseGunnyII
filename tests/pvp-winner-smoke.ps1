$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$helper = Join-Path $root 'Game.Logic\PvpWinnerResolver.cs'
if (-not (Test-Path $helper)) { throw 'PvpWinnerResolver.cs missing' }
$tmp = Join-Path $env:TEMP 'gunny-pvp-winner-smoke'
New-Item -ItemType Directory -Force -Path $tmp | Out-Null
$test = Join-Path $tmp 'Program.cs'
@'
using System;
using Game.Logic;
class Program {
  static void Check(int actual, int expected, string name) {
    if (actual != expected) throw new Exception(name + ": expected " + expected + ", got " + actual);
  }
  static void Main() {
    Check(PvpWinnerResolver.Resolve(true, false), 1, "red survives");
    Check(PvpWinnerResolver.Resolve(false, true), 2, "blue survives");
    Check(PvpWinnerResolver.Resolve(false, false), 0, "both dead is draw");
    Check(PvpWinnerResolver.Resolve(true, true), 0, "both alive is unresolved");
    Console.WriteLine("PVP_WINNER_SMOKE=PASS");
  }
}
'@ | Set-Content -Path $test -Encoding UTF8
$csc = 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'
& $csc /nologo /out:"$tmp\winner-smoke.exe" $helper $test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& "$tmp\winner-smoke.exe"
exit $LASTEXITCODE