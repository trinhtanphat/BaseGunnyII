$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bot = Get-Content (Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs') -Raw
if ($bot -notmatch 'GetShootPoint\(\)') { throw 'trajectory validation must originate from the actual shoot point' }
if ($bot -match 'IsTrajectoryViable\(player, target,\s*candidateX,\s*candidateY') { throw 'trajectory validation still starts at target coordinates' }
Write-Output 'PVP_BOT_TRAJECTORY_ORIGIN_SMOKE=PASS'
