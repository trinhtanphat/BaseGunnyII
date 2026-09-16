$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$pvpPath = Join-Path $root 'Game.Logic\PVPGame.cs'
$pvp = [IO.File]::ReadAllText($pvpPath)
function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}
Assert-True ($pvp.Contains('SendShowCards(player);')) 'PVP does not reveal remaining cards after a successful take'
Assert-True ($pvp.Contains('private void SendShowCards(Player player)')) 'PVP player-scoped SHOW_CARDS helper missing'
Assert-True ($pvp.Contains('pkg.WriteByte((byte)eTankCmdType.SHOW_CARDS);')) 'SHOW_CARDS command is not emitted'
Assert-True ($pvp.Contains('player.PlayerDetail.SendTCP(pkg);')) 'SHOW_CARDS is not scoped to the player who picked'
Assert-True ($pvp.Contains('if (Cards[i] == 0)')) 'PVP reveal does not limit previews to unopened cards'
Assert-True ($pvp.Contains('DropInventory.CardDrop(RoomType, ref infos)')) 'PVP reveal does not use the canonical card-drop source'
Write-Host 'PVP_REWARD_REVEAL_SMOKE=PASS'