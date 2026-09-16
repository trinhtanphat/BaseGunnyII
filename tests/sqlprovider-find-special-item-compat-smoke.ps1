$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$path = Join-Path $root 'recovery\runtime-source\by-service\Fight\SqlDataProvider\SqlDataProvider\Data\ItemInfo.cs'
$text = [IO.File]::ReadAllText($path)
$required = @(
  'public static void FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int medal)',
  'case -100:',
  'gold += info.Count;',
  'case -200:',
  'money += info.Count;',
  'case -300:',
  'giftToken += info.Count;',
  'case 11408:',
  'medal += info.Count;'
)
foreach ($needle in $required) {
  if (-not $text.Contains($needle)) { throw "Missing ItemInfo compatibility behavior: $needle" }
}
Write-Host 'SQLPROVIDER_FIND_SPECIAL_ITEM_COMPAT_SMOKE=PASS'
