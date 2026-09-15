$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$path = Join-Path $root 'Game.Logic\Actions\CheckPVPGameStateAction.cs'
if (-not (Test-Path $path)) { throw "missing CheckPVPGameStateAction.cs: $path" }
$text = Get-Content $path -Raw
$start = $text.IndexOf('case eGameState.Playing:')
$stop = $text.IndexOf('case eGameState.GameOver:', $start)
if ($start -lt 0 -or $stop -le $start) { throw 'PVP Playing state block not found' }
$block = $text.Substring($start, $stop - $start)
$can = $block.IndexOf('pvp.CanGameOver()')
$attack = $block.IndexOf('pvp.CurrentPlayer == null || pvp.CurrentPlayer.IsAttacking == false')
$gameOver = $block.IndexOf('pvp.GameOver()')
$nextTurn = $block.IndexOf('pvp.NextTurn()')
if ($can -lt 0 -or $attack -lt 0 -or $gameOver -lt 0 -or $nextTurn -lt 0) { throw 'expected PVP state transitions are missing' }
if ($can -gt $attack) { throw 'team-death game-over is still blocked behind CurrentPlayer.IsAttacking' }
if ($gameOver -gt $attack) { throw 'GameOver must be reachable before the attack-complete gate' }
if ($nextTurn -lt $attack) { throw 'NextTurn must remain guarded by attack completion' }
Write-Host 'PVP_IMMEDIATE_TEAM_DEATH_SMOKE=PASS'
