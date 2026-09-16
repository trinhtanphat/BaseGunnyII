$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bot = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
$text = Get-Content $bot -Raw
if ($text.Contains('target.GetDirectBoudRect()')) {
    throw 'bot predictor still depends on stale runtime GetDirectBoudRect geometry'
}
foreach ($token in @('target.Bound', 'target.Bound1', '.Offset(target.X, target.Y)')) {
    if (-not $text.Contains($token)) { throw "runtime collision-bounds parity missing: $token" }
}
Write-Output 'PVP_BOT_RUNTIME_BOUNDS_COMPAT_SMOKE=PASS'
