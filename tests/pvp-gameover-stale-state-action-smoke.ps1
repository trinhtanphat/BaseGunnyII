$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$actionPath = Join-Path $root 'Game.Logic\Actions\CheckPVPGameStateAction.cs'
$pvpPath = Join-Path $root 'Game.Logic\PVPGame.cs'
$action = Get-Content $actionPath -Raw
$pvp = Get-Content $pvpPath -Raw
if ($action -notmatch 'private\s+eGameState\s+m_scheduledState\s*;') { throw 'PVP state action must remember the state that scheduled it.' }
if ($action -notmatch 'CheckPVPGameStateAction\(int delay,\s*eGameState scheduledState\)') { throw 'PVP state action constructor must receive the scheduled state.' }
if ($action -notmatch '(?s)case\s+eGameState\.GameOver\s*:.*?m_scheduledState\s*==\s*eGameState\.GameOver.*?pvp\.Stop\(\)') { throw 'A stale Playing-state action must not immediately stop a newly GameOver match.' }
if ($pvp -notmatch 'new\s+CheckPVPGameStateAction\(delay,\s*GameState\)') { throw 'PVP CheckState must bind each action to its scheduling state.' }
Write-Output 'PVP_GAMEOVER_STALE_STATE_ACTION_SMOKE=PASS'
