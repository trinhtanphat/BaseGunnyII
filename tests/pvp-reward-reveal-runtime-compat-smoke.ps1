$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$pvpPath = Join-Path $root 'Game.Logic\PVPGame.cs'
$source = Get-Content -Raw $pvpPath
$match = [regex]::Match($source, '(?s)private\s+void\s+SendShowCards\s*\(Player\s+player\)\s*\{(?<body>.*?)\n\s*\}\s*\n\s*public\s+void\s+GameOver\s*\(')
if (-not $match.Success) { throw 'SendShowCards body not found' }
$body = $match.Groups['body'].Value
if ($body -match 'ItemInfo\.FindSpecialItemInfo\s*\(') { throw 'SendShowCards still depends on ItemInfo.FindSpecialItemInfo, which is absent from the deployed SqlDataProvider runtime' }
if ($body -match 'ShopMgr\.FindSpecialItemInfo\s*\(') { throw 'SendShowCards must not use runtime-lineage-dependent ShopMgr special-item semantics' }
if ($body -notmatch '(?s)foreach\s*\(ItemInfo\s+info\s+in\s+infos\).*?if\s*\(info\s*==\s*null\)\s*continue;\s*switch\s*\(info\.TemplateID\)') { throw 'SendShowCards must null-check dropped preview entries before reading TemplateID' }
$required = @(
    'case\s+-100\s*:\s*gold\s*\+=\s*info\.Count\s*;',
    'case\s+-200\s*:\s*money\s*\+=\s*info\.Count\s*;',
    'case\s+-300\s*:\s*giftToken\s*\+=\s*info\.Count\s*;'
)
foreach ($pattern in $required) {
    if ($body -notmatch $pattern) { throw "SendShowCards direct reward mapping missing: $pattern" }
}
Write-Host 'PVP_REWARD_REVEAL_RUNTIME_COMPAT_SMOKE=PASS'
