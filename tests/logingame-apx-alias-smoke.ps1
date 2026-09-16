$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$alias = Join-Path $root 'Tank.Flash\LoginGame.apx'
$proj = Get-Content (Join-Path $root 'Tank.Flash\Tank.Flash.csproj') -Raw
$config = Get-Content (Join-Path $root 'Tank.Flash\Web.config') -Raw
if (-not (Test-Path $alias)) { throw 'LoginGame.apx compatibility page is missing' }
$page = Get-Content $alias -Raw
if ($page -notmatch 'Inherits="Tank\.Flash\.logingame"') { throw 'LoginGame.apx must reuse Tank.Flash.logingame' }
if ($proj -notmatch 'Content Include="LoginGame\.apx"') { throw 'LoginGame.apx must be packaged by Tank.Flash.csproj' }
if ($config -notmatch 'name="LoginGameApxAlias"[^>]+path="LoginGame\.apx"[^>]+System\.Web\.UI\.PageHandlerFactory') { throw 'LoginGame.apx requires a path-specific ASP.NET PageHandlerFactory mapping' }
if ($config -notmatch 'extension="\.apx"[^>]+System\.Web\.Compilation\.PageBuildProvider') { throw 'LoginGame.apx requires ASP.NET PageBuildProvider registration' }
Write-Host 'LOGIN_GAME_APX_ALIAS_SMOKE=PASS'
