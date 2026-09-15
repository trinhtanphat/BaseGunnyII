$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$msbuild = 'C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
$refs = 'C:\Gunny\toolchain\reference-root\'
if (-not (Test-Path $msbuild)) { throw 'MSBuild 17 not found' }
if (-not (Test-Path (Join-Path $refs '.NETFramework\v3.5\mscorlib.dll'))) { throw 'net35 reference pack missing' }
& $msbuild (Join-Path $root 'Fighting.Server\Fighting.Server.csproj') /t:Rebuild /p:Configuration=Release /p:TargetFrameworkRootPath=$refs /p:BypassFrameworkInstallChecks=true /v:minimal /m:1
if ($LASTEXITCODE -ne 0) { throw "canonical Fighting.Server build failed: $LASTEXITCODE" }
$dll = Join-Path $root 'Fighting.Server\bin\Release\Fighting.Server.dll'
if (-not (Test-Path $dll)) { throw 'canonical Fighting.Server.dll missing after build' }
Write-Host 'PVP_CANONICAL_BUILD_SMOKE=PASS'
