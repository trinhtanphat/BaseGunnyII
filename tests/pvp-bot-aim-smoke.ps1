$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
$text = Get-Content $file -Raw
function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}
Assert-True ($text.Contains('private static Player FindBestTarget')) 'deterministic target selector missing'
Assert-True ($text.Contains('private static bool TryFindAccurateShot')) 'accurate ballistic planner missing'
Assert-True ($text.Contains('float[] timeSeeds = { 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f };')) 'fine-grained ballistic time seeds missing'
Assert-True ($text.Contains('int[] yOffsets = { 0, -8, 8, -16, 16 };')) 'fallback target hitbox offsets missing'
$direction = $text.IndexOf('player.Direction = target.X >= player.X ? 1 : -1;')
$solve = $text.IndexOf('TryFindAccurateShot(player, target')
Assert-True ($direction -ge 0 -and $solve -gt $direction) 'bot direction must be set before ballistic solve'
Assert-True ($text.Contains('candidate.Blood < target.Blood')) 'low-HP tie-breaker missing'
Write-Output 'PVP_BOT_AIM_SMOKE=PASS'