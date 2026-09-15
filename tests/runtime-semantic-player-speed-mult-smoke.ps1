$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
$baseGame=Get-Content (Join-Path $root 'Game.Logic\BaseGame.cs') -Raw
$living=Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Living.cs') -Raw
$player=Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Player.cs') -Raw
$actionPath=Join-Path $root 'Game.Logic\Actions\PlayerSpeedMultAction.cs'
$proj=Get-Content (Join-Path $root 'Game.Logic\Game.Logic.csproj') -Raw
function Need([string]$text,[string]$p,[string]$m){ if($text -notmatch $p){ throw $m } }
Need $baseGame 'SendGamePlayerProperty\s*\(\s*Living\s+living\s*,\s*string\s+type\s*,\s*string\s+state\s*\)' 'SendGamePlayerProperty missing'
Need $baseGame 'WriteByte\s*\(\s*41\s*\)' 'property packet subcommand missing'
Need $baseGame '(?s)WriteString\s*\(\s*type\s*\).*WriteString\s*\(\s*state\s*\)' 'property packet payload missing'
Need $living 'public\s+void\s+SpeedMultX\s*\(\s*int\s+value\s*\)' 'Living.SpeedMultX missing'
Need $living 'SendGamePlayerProperty\s*\(\s*this\s*,\s*"speedX"\s*,\s*value\.ToString\(\)' 'SpeedMultX wire property missing'
Need $player 'public\s+void\s+StartSpeedMult\s*\(\s*int\s+x\s*,\s*int\s+y\s*\)' 'StartSpeedMult 2-arg missing'
Need $player 'public\s+void\s+StartSpeedMult\s*\(\s*int\s+x\s*,\s*int\s+y\s*,\s*int\s+delay\s*\)' 'StartSpeedMult 3-arg missing'
if(!(Test-Path $actionPath)){ throw 'PlayerSpeedMultAction source missing' }
$action=Get-Content $actionPath -Raw
Need $action 'class\s+PlayerSpeedMultAction\s*:\s*BaseAction' 'PlayerSpeedMultAction type missing'
Need $action 'm_player\.SpeedMultX\s*\(\s*18\s*\)' 'PlayerSpeedMultAction speed packet missing'
Need $action 'SendPlayerMove\s*\(' 'PlayerSpeedMultAction move packet missing'
Need $proj 'Compile Include="Actions\\PlayerSpeedMultAction\.cs"' 'PlayerSpeedMultAction csproj mapping missing'
Write-Output 'RUNTIME_SEMANTIC_PLAYER_SPEED_MULT_SMOKE=PASS'