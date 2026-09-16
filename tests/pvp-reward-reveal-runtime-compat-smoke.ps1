$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$pvpPath = Join-Path $root 'Game.Logic\PVPGame.cs'
$source = Get-Content -Raw $pvpPath
$match = [regex]::Match($source, '(?s)private\s+void\s+SendShowCards\s*\(Player\s+player\)\s*\{(?<body>.*?)\n\s*\}\s*\n\s*public\s+void\s+GameOver\s*\(')
if (-not $match.Success) { throw 'SendShowCards body not found' }
$body = $match.Groups['body'].Value
if ($body -match '(?:ItemInfo|ShopMgr)\.FindSpecialItemInfo\s*\(') { throw 'SendShowCards must not depend on runtime-lineage special-item helpers' }
if ($body -match 'switch\s*\(info\.TemplateID\)') { throw 'SendShowCards must not reinterpret preview currency ids' }
if ($body -match '(?:gold|money|giftToken|medal)\s*\+=\s*info\.Count') { throw 'SendShowCards must not remap preview counts through legacy currency accumulators' }
if ($body -notmatch 'templateID\s*=\s*info\.TemplateID\s*;') { throw 'SendShowCards must pass through dropped TemplateID' }
if ($body -notmatch 'itemCount\s*=\s*info\.Count\s*;') { throw 'SendShowCards must pass through dropped Count' }
if ($body -notmatch 'pkg\.WriteInt\(templateID\)') { throw 'SHOW_CARDS packet does not emit preview TemplateID' }
if ($body -notmatch 'pkg\.WriteInt\(itemCount\)') { throw 'SHOW_CARDS packet does not emit preview Count' }
Write-Host 'PVP_REWARD_REVEAL_RUNTIME_COMPAT_SMOKE=PASS'
