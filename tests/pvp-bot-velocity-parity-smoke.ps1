$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$helper = Get-Content (Join-Path $root 'Fighting.Server\GameObjects\BotAimTrajectory.cs') -Raw
if ($helper -notmatch 'float\s+vx\s*=\s*\(int\)\(force\s*\*\s*Math\.Cos\(radians\)\)\s*;') {
    throw 'bot trajectory initial vx must truncate to int like Living.ShootImp'
}
if ($helper -notmatch 'float\s+vy\s*=\s*\(int\)\(force\s*\*\s*Math\.Sin\(radians\)\)\s*;') {
    throw 'bot trajectory initial vy must truncate to int like Living.ShootImp'
}
Write-Host 'PVP_BOT_VELOCITY_PARITY_SMOKE=PASS'
