$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$packetPath = Join-Path $root 'Game.Server\Packets\Server\AbstractPacketLib.cs'
$packet = Get-Content $packetPath -Raw
$method = [regex]::Match($packet, '(?s)public GSPacketIn SendUpdateQuests\(GamePlayer player, byte\[\] states, BaseQuest\[\] infos\).*?#region')
if (-not $method.Success) { throw 'Could not locate SendUpdateQuests serializer.' }
$text = $method.Value
if ($text -match '(?s)var info = infos\[j\];\s*if \(info\.Data\.IsExist\)\s*\{') {
    throw 'QUEST_UPDATE advertises each slot in length but omits the payload when IsExist=false, desynchronizing the client reader.'
}
$required = @(
    'pkg.WriteInt(info.Data.QuestID)',
    'pkg.WriteBoolean(info.Data.IsComplete)',
    'pkg.WriteInt(info.Data.Condition1)',
    'pkg.WriteInt(info.Data.Condition2)',
    'pkg.WriteInt(info.Data.Condition3)',
    'pkg.WriteInt(info.Data.Condition4)',
    'pkg.WriteDateTime(info.Data.CompletedDate)',
    'pkg.WriteInt(info.Data.RepeatFinish)',
    'pkg.WriteInt(info.Data.RandDobule)',
    'pkg.WriteBoolean(info.Data.IsExist)'
)
foreach ($write in $required) {
    if (-not $text.Contains($write)) { throw "QUEST_UPDATE missing deployed 4.1 field: $write" }
}
if ($text -notmatch 'pkg\.WriteInt\(length\);') { throw 'QUEST_UPDATE must advertise the serialized record count.' }
Write-Output 'QUEST_UPDATE_PACKET_CARDINALITY_SMOKE=PASS'
