$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$bot = Join-Path $root 'Fighting.Server\GameObjects\BotProxyPlayer.cs'
$botInterface = Join-Path $root 'Game.Logic\IBotGamePlayer.cs'
$room = Join-Path $root 'Fighting.Server\Rooms\ProxyRoom.cs'
$mgr = Join-Path $root 'Fighting.Server\Rooms\ProxyRoomMgr.cs'
$pvp = Join-Path $root 'Game.Logic\PVPGame.cs'
foreach ($f in @($bot,$botInterface,$room,$mgr,$pvp)) {
  if (-not (Test-Path $f)) { throw "missing bot-fill file: $f" }
}
$botText = Get-Content $bot -Raw
$ifaceText = Get-Content $botInterface -Raw
$roomText = Get-Content $room -Raw
$mgrText = Get-Content $mgr -Raw
$pvpText = Get-Content $pvp -Raw
if ($ifaceText -notmatch 'interface\s+IBotGamePlayer') { throw 'bot interface missing' }
if ($botText -notmatch 'class\s+BotProxyPlayer\s*:\s*IGamePlayer\s*,\s*IBotGamePlayer') { throw 'bot player contract missing' }
if ($botText -notmatch 'TakeTurn|GetShootForceAndAngle|\.Shoot\(') { throw 'server bot combat path missing' }
if ($mgrText -notmatch 'Interlocked\.Decrement') { throw 'negative runtime bot id allocation missing' }
if ($roomText -notmatch 'IsSyntheticBotRoom') { throw 'synthetic room guard missing' }
if ($roomText -notmatch 'BotFillEligibleTick.*5000L') { throw '5-second room deadline missing' }
if ($mgrText -notmatch 'PICK_UP_INTERVAL\s*=\s*1000') { throw '1-second polling missing' }
if ($mgrText -notmatch 'BOT_FILL_WAIT_MS\s*=\s*5000') { throw '5-second fill constant missing' }
if ($mgrText -match 'MatchWaitCycles') { throw 'old cycle-based timing still present' }
if ($mgrText -notmatch 'RoomType\s*==\s*eRoomType\.Match') { throw 'match-only fill guard missing' }
if ($mgrText -notmatch 'PlayerCount\s*==\s*1') { throw 'one-human fill guard missing' }
if ($mgrText -notmatch 'GameType\s*!=\s*eGameType\.Guild') { throw 'guild exclusion missing' }
if ($mgrText -notmatch 'tick\s*>=\s*red\.BotFillEligibleTick') { throw 'eligible timestamp gate missing' }
if ($mgrText -notmatch 'MainWeapon\s*==\s*null') { throw 'weapon safety guard missing' }
if ($mgrText -notmatch 'CreateBotRoom') { throw 'bot room factory missing' }
if ($pvpText -notmatch 'LoadingProcess\s*=\s*100') { throw 'bot auto-loading missing' }
if ($pvpText -notmatch 'bot\.TakeTurn') { throw 'PVP bot turn hook missing' }
if ($pvpText -notmatch 'hasBot') { throw 'bot reward suppression marker missing' }
Write-Host 'PVP_BOT_FILL_SMOKE=PASS'
