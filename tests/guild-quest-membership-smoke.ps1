$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$conditionPath = Join-Path $root 'Game.Server\Quests\OwnConsortiaCondition.cs'
$playerPath = Join-Path $root 'Game.Server\GameObjects\GamePlayer.cs'
$connectorPath = Join-Path $root 'Game.Server\LoginServerConnector.cs'
$condition = Get-Content $conditionPath -Raw
$player = Get-Content $playerPath -Raw
$connector = Get-Content $connectorPath -Raw
if ($condition -notmatch '!player\.PlayerCharacter\.IsConsortia\s*\|\|\s*player\.PlayerCharacter\.ConsortiaID\s*<=\s*0') { throw 'Guild quest condition must reject players who are not currently in a guild.' }
if ($condition -notmatch '(?s)AddTrigger\(GamePlayer player\).*?RefreshValue\(player\)') { throw 'Guild quest condition must refresh persisted progress when attached on login.' }
if ($condition -notmatch '(?s)override void Reset\(GamePlayer player\).*?base\.Reset\(player\).*?RefreshValue\(player\)') { throw 'Freshly accepted guild quests must recompute membership after Reset.' }
if ($condition -notmatch '(?s)player_OwnConsortia\(\).*?RefreshValue') { throw 'GuildChanged callback must refresh the quest condition instead of being empty.' }
if ($player -notmatch '(?s)public void ClearConsortia\(\).*?OnGuildChanged\(\)') { throw 'Leaving/disbanding a guild must notify guild quest conditions.' }
if ($connector -notmatch '(?s)HandleConsortiaUserPass\(GSPacketIn packet\).*?p\.PlayerCharacter\.ConsortiaID\s*=\s*consortiaID.*?p\.OnGuildChanged\(\)') { throw 'Joining a guild must notify guild quest conditions.' }
Write-Output 'GUILD_QUEST_MEMBERSHIP_SMOKE=PASS'
