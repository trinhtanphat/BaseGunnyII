$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$data = Get-Content (Join-Path $root 'SqlDataProvider\Data\QuestDataInfo.cs') -Raw
$db = Get-Content (Join-Path $root 'Bussiness\PlayerBussiness.cs') -Raw
foreach ($name in @('QuestLevel','Condition5','Condition6','Condition7','Condition8')) {
    if (-not $data.Contains("public int $name")) { throw "QuestDataInfo missing $name" }
    if (-not $db.Contains("reader[`"$name`"]")) { throw "GetUserQuest missing persisted $name" }
    if (-not $db.Contains("new SqlParameter(`"@$name`", info.$name)")) { throw "UpdateDbQuestDataInfo missing @$name" }
}
if ($data -notmatch '(?s)setProgressConcoat\(\).*?Condition5.*?Condition6.*?Condition7.*?Condition8') {
    throw 'QuestDataInfo extra-progress projection must include Condition5-8.'
}
if (-not $db.Contains('SqlParameter[] para = new SqlParameter[16]')) {
    throw 'QuestData persistence must send the full 16-parameter SP_QuestData_Add contract.'
}
Write-Output 'QUEST_DATA_PERSISTENCE_SMOKE=PASS'
