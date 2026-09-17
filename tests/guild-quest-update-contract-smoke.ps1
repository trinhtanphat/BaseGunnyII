$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$packet = Get-Content (Join-Path $root 'Game.Server\Packets\Server\AbstractPacketLib.cs') -Raw
$playerInfo = Get-Content (Join-Path $root 'SqlDataProvider\Data\PlayerInfo.cs') -Raw
$condition = Get-Content (Join-Path $root 'Game.Server\Quests\OwnConsortiaCondition.cs') -Raw
$login = Get-Content (Join-Path $root 'Game.Server\LoginServerConnector.cs') -Raw
$handler = Get-Content (Join-Path $root 'Game.Server\Packets\Client\ConsortiaHandler.cs') -Raw
$finish = Get-Content (Join-Path $root 'Game.Server\Packets\Client\QuestFinishHandler.cs') -Raw
$quest = Get-Content (Join-Path $root 'Game.Server\Quests\BaseQuest.cs') -Raw
if ($packet -match '(?s)WriteBoolean\(info\.Data\.IsExist\).*?WriteInt\(3\)\s*;\s*//.*?QuestLevel') { throw 'QUEST_UPDATE writes an extra QuestLevel int not consumed by the deployed 4.1 client parser.' }
if ($playerInfo -notmatch '(?s)void ClearConsortia\(\).*?IsConsortia\s*=\s*false') { throw 'ClearConsortia must clear the IsConsortia flag.' }
if ($condition -notmatch 'GetConsortiaUsersByUserID\(player\.PlayerCharacter\.ID\)') { throw 'Guild quest authority must verify the membership row.' }
if ($condition -notmatch 'membership\.ConsortiaID\s*!=\s*player\.PlayerCharacter\.ConsortiaID') { throw 'Guild quest authority must require membership to match the current guild id.' }
if ($login -notmatch '(?s)HandleConsortiaUserPass.*?IsConsortia\s*=\s*true.*?OnGuildChanged\(\)') { throw 'Join broadcast must set IsConsortia before GuildChanged.' }
if ($handler -notmatch '(?s)CONSORTIA_CREATE.*?IsConsortia\s*=\s*true.*?OnGuildChanged\(\)') { throw 'Guild creation must refresh guild state immediately.' }
if ($finish -notmatch 'QuestInventory\.Finish\(_baseQuest, rewardItemID\)') { throw 'Quest claims must delegate to authoritative QuestInventory.Finish.' }
if ($quest -notmatch '(?s)public bool Finish\(GamePlayer player\).*?if \(CanCompleted\(player\)\)') { throw 'BaseQuest.Finish must revalidate live conditions.' }
Write-Output 'GUILD_QUEST_AUTHORITY_SMOKE=PASS'
