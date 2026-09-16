$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$runtime = Join-Path $root 'recovery\runtime-source\by-service\Road\Game.Server\Game\Server'
$inventory = Get-Content (Join-Path $runtime 'GameUtils\PlayerFarmInventory.cs') -Raw
$farm = Get-Content (Join-Path $runtime 'GameUtils\PlayerFarm.cs') -Raw
$treasure = Get-Content (Join-Path $runtime 'Packets\Client\TreasureHandler.cs') -Raw
$player = Get-Content (Join-Path $runtime 'GameObjects\GamePlayer.cs') -Raw
if ($inventory -notmatch '(?s)AddFieldTo\(UserFieldInfo item, int place\).*?m_fields\[place\] != null.*?return false;.*?m_fields\[place\] = item;.*?item\.FieldID = place;') {
    throw 'Farm field insert must reject an occupied slot before assigning it.'
}
if ($inventory -notmatch '(?s)AddOtherFieldTo\(UserFieldInfo item, int place\).*?m_otherFields\[place\] != null.*?return false;.*?m_otherFields\[place\] = item;.*?item\.FieldID = place;') {
    throw 'Friend farm field insert must reject an occupied slot before assigning it.'
}
if ($inventory -notmatch '(?s)GrowField\(int fieldId, int templateID\).*?fieldId < 0.*?fieldId >= m_fields\.Length.*?m_fields\[fieldId\] == null.*?itemTemplateInfo == null.*?m_fields\[fieldId\]\.SeedID != 0') {
    throw 'Planting must validate field bounds, field existence, seed template, and empty state.'
}
if ($farm -notmatch '(?s)GainField\(int fieldId\).*?fieldId < 0.*?fieldId >= CurrentFields\.Length.*?GetFieldAt\(fieldId\) == null.*?!GetFieldAt\(fieldId\)\.isDig\(\)') {
    throw 'Harvest must reject invalid/unripe fields and allow mature fields.'
}
if ($treasure -notmatch '(?s)case 3:.*?num2 <= 0.*?num2 > client\.Player\.Treasure\.TreasureData\.Count') {
    throw 'Treasure dig must validate the requested position before indexing reward data.'
}
if ($treasure -notmatch '(?s)ItemTemplateInfo rewardTemplate.*?rewardTemplate == null.*?return 0;.*?ItemInfo\.CreateFromTemplate\(rewardTemplate') {
    throw 'Treasure dig must validate the reward template before consuming a dig.'
}
if ($player -notmatch '(?s)public bool AddTemplate\(ItemInfo cloneItem, eBageType bagType.*?BagFullSendToMail\(list\);.*?return true;') {
    throw 'Treasure AddTemplate contract must preserve bag-full rewards through mail fallback.'
}
if ($farm -match '(?s)SaveToDatabase\(\).*?if \(m_farm == null \|\| !m_farm\.IsDirty\).*?return;') {
    throw 'Farm persistence must not skip dirty fields just because the farm header is clean.'
}
if ($farm -notmatch '(?s)SaveToDatabase\(\).*?if \(m_farm != null && m_farm\.IsDirty\).*?for \(int i = 0; i < m_fields\.Length; i\+\+\)') {
    throw 'Farm persistence must save the farm header conditionally and still scan dirty fields.'
}
if ($farm -notmatch '(?s)GainFriendFields\(int userId, int fieldId\).*?fieldId < 0.*?fieldId >= OtherFields\.Length.*?GetOtherFieldAt\(fieldId\) == null.*?!GetOtherFieldAt\(fieldId\)\.isDig\(\)') {
    throw 'Friend harvest must validate bounds/existence and require a mature crop.'
}
if ($inventory -match 'm_(other)?fields\[i\]\.AccelerateTime\s*=\s*AccelerateTimeFields\(') {
    throw 'Elapsed wall-clock growth must not be folded into AccelerateTime.'
}
if ($treasure -notmatch '(?s)TreasureDataInfo treasureDataInfo.*?treasureDataInfo\.pos > 0.*?return 0;') {
    throw 'Treasure dig must reject a reward position that was already claimed.'
}
Write-Output 'FARM_TREASURE_RUNTIME_SMOKE=PASS'
