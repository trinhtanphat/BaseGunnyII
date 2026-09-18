$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$file = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
$text = Get-Content $file -Raw
function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}
Assert-True ($text.Contains('private static bool TryFindTerrainClearShot')) 'terrain-clearing planner missing'
Assert-True ($text.Contains('BotTrajectoryOutcome.Terrain')) 'planner must distinguish terrain impact from a miss'
Assert-True ($text.Contains('double teamDistance = 0;')) 'team-wide target focus missing'
Assert-True ($text.Contains('TryUseSupportSkill(game, player);')) 'support skill policy missing'
Assert-True ($text.Contains('SelfHealTemplateId = 10012')) 'self-heal prop policy missing'
Assert-True ($text.Contains('TeamHealTemplateId = 10009')) 'team-heal prop policy missing'
Assert-True ($text.Contains('FlyTemplateId = 10016')) 'fly/reposition prop policy missing'
Assert-True ($text.Contains('DamageBoostTemplateId = 10004')) 'damage boost policy missing'
Assert-True ($text.Contains('MultiBallTemplateId = 10003')) 'multi-ball policy missing'
Assert-True ($text.Contains('m_selfHealUses < 2')) 'heal budget missing'
Assert-True ($text.Contains('m_flyUses < 2')) 'fly budget missing'
Assert-True ($text.Contains('clearProgress >= 45 || !canFly')) 'clear-vs-fly tactical comparison missing'
Assert-True ($text.Contains('game.GetAllFightPlayers()')) 'bot must reason over the full fight, not client viewport'
Write-Output 'PVP_BOT_TACTICAL_PLANNER_SMOKE=PASS'
