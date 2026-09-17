$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$path = Join-Path $root 'Game.Logic\PVPGame.cs'
$text = Get-Content $path -Raw
$start = $text.IndexOf('public void StartLoading()')
if ($start -lt 0) { throw 'PVPGame.StartLoading missing' }
$tail = $text.Substring($start)
$end = $tail.IndexOf('public void StartGame()')
if ($end -lt 0) { throw 'PVPGame.StartLoading boundary missing' }
$method = $tail.Substring(0,$end)
foreach ($token in @(
    'p.LoadingProcess = 100;',
    'new GSPacketIn((short)ePackageType.GAME_CMD)',
    'WriteByte((byte)eTankCmdType.LOAD)',
    'WriteInt(p.LoadingProcess)',
    'WriteInt(p.PlayerDetail.PlayerCharacter.ID)',
    'SendToAll('
)) {
    if (-not $method.Contains($token)) { throw "BOT loading ready broadcast missing: $token" }
}
$set = $method.IndexOf('p.LoadingProcess = 100;')
$send = $method.IndexOf('SendToAll(', $set)
if ($send -lt $set) { throw 'BOT loading ready broadcast must occur after LoadingProcess=100' }
Write-Host 'PVP_BOT_LOADING_READY_SMOKE=PASS'
