$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$helper = Join-Path $root 'Game.Logic\PvpSpawnResolver.cs'
$pvp = Join-Path $root 'Game.Logic\PVPGame.cs'
foreach ($f in @($helper,$pvp)) { if (-not (Test-Path $f)) { throw "missing file: $f" } }
$pvpText = Get-Content $pvp -Raw
if ($pvpText -notmatch 'HashSet<int>\s+reservedSpawnX\s*=\s*new HashSet<int>\(\)') { throw 'shared spawn reservation missing' }
if ($pvpText -notmatch 'FindFallbackSpawn\(p\.Team,\s*reservedSpawnX\)') { throw 'fallback does not share spawn reservation' }
if ($pvpText -notmatch 'reservedSpawnX\.Contains\(x\)') { throw 'fallback does not reject occupied spawn lane' }
if ($pvpText -notmatch 'reservedSpawnX\.Add\(x\)') { throw 'fallback does not reserve selected lane' }
$tmp = Join-Path $env:TEMP 'gunny-pvp-distinct-spawn-smoke'
New-Item -ItemType Directory -Force -Path $tmp | Out-Null
$test = Join-Path $tmp 'Program.cs'
@"
using System; using System.Collections.Generic; using System.Drawing; using Game.Logic;
class Program {
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);}
 static void Main(){
  var reserved=new HashSet<int>();
  var firstPool=new List<Point>{new Point(500,400),new Point(600,400)};
  Point first=PvpSpawnResolver.TakeSafeSpawn(firstPool,p=>true,c=>0,reserved);
  Check(first.X==500,"first spawn mismatch");
  var secondPool=new List<Point>{new Point(500,400),new Point(600,400)};
  Point second=PvpSpawnResolver.TakeSafeSpawn(secondPool,p=>true,c=>0,reserved);
  Check(second.X==600,"duplicate spawn lane was not skipped");
  Check(reserved.Contains(500)&&reserved.Contains(600),"selected lanes were not reserved");
  Console.WriteLine("PVP_DISTINCT_SPAWN_BEHAVIOR=PASS");
 }
}
"@ | Set-Content -Path $test -Encoding UTF8
$csc='C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'
$out = Join-Path $tmp 'spawn-distinct-smoke.exe'
& $csc /nologo /r:System.Core.dll /r:System.Drawing.dll /out:$out $helper $test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& $out
exit $LASTEXITCODE