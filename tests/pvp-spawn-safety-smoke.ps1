$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$helper = Join-Path $root 'Game.Logic\PvpSpawnResolver.cs'
if (-not (Test-Path $helper)) { throw 'PvpSpawnResolver.cs missing' }
$tmp = Join-Path $env:TEMP 'gunny-pvp-spawn-safety-smoke'
New-Item -ItemType Directory -Force -Path $tmp | Out-Null
$test = Join-Path $tmp 'Program.cs'
@"
using System; using System.Collections.Generic; using System.Drawing; using Game.Logic;
class Program {
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);}
 static void Main(){
  var points=new List<Point>{new Point(800,400),new Point(700,400)};
  Point safe=PvpSpawnResolver.TakeSafeSpawn(points,delegate(Point p){return p.X==700;},delegate(int count){return 0;});
  Check(safe==new Point(700,400),"unsafe spawn was not skipped");
  Check(points.Count==0,"tested spawn candidates were not consumed");
  var bad=new List<Point>{new Point(600,400)};
  Point none=PvpSpawnResolver.TakeSafeSpawn(bad,delegate(Point p){return false;},delegate(int count){return 0;});
  Check(none==Point.Empty,"all-unsafe set must fail closed");
  Console.WriteLine("PVP_SPAWN_SAFETY_SMOKE=PASS");
 }
}
"@ | Set-Content -Path $test -Encoding UTF8
$csc='C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'
& $csc /nologo /r:System.Drawing.dll /out:"$tmp\spawn-smoke.exe" $helper $test
if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}; & "$tmp\spawn-smoke.exe"; exit $LASTEXITCODE
