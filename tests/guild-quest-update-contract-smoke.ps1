$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$packetPath = Join-Path $root 'Game.Server\Packets\Server\AbstractPacketLib.cs'
$finishPath = Join-Path $root 'Game.Server\Packets\Client\QuestFinishHandler.cs'
$questPath = Join-Path $root 'Game.Server\Quests\BaseQuest.cs'
$packet = Get-Content $packetPath -Raw
$finish = Get-Content $finishPath -Raw
$quest = Get-Content $questPath -Raw

$extraQuestLevel = '(?s)WriteBoolean\(info\.Data\.IsExist\).*?WriteInt\(3\)\s*;\s*//.*?QuestLevel'
if ($packet -match $extraQuestLevel) {
    throw 'QUEST_UPDATE writes an extra QuestLevel int not consumed by the deployed 4.1 client parser.'
}

if ($finish -notmatch 'QuestInventory\.Finish\(_baseQuest, rewardItemID\)') {
    throw 'QuestFinishHandler must delegate reward claims to authoritative QuestInventory.Finish.'
}
if ($quest -notmatch '(?s)public bool Finish\(GamePlayer player\).*?if \(CanCompleted\(player\)\)') {
    throw 'BaseQuest.Finish must revalidate live quest conditions before finishing.'
}
Write-Output 'GUILD_QUEST_UPDATE_CONTRACT_SMOKE=PASS'