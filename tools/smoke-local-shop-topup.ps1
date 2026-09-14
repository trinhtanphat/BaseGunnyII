param(
    [string]$RequestRoot = 'C:\Gunny\GunnyFileExe\request',
    [string]$LocalBase = 'http://127.0.0.1/Request',
    [string]$PublicBase = 'http://103.9.156.182/Request'
)
$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]
$repoRoot = Split-Path $PSScriptRoot -Parent

function Get-AppSetting([xml]$Config, [string]$Key) {
    $node = $Config.configuration.appSettings.add | Where-Object { $_.key -eq $Key } | Select-Object -First 1
    if ($null -eq $node) { return $null }
    return [string]$node.value
}

[xml]$config = Get-Content (Join-Path $RequestRoot 'Web.config') -Raw
$chargeIp = Get-AppSetting $config 'ChargeIP'
$adminIp = Get-AppSetting $config 'AdminIP'
if ($chargeIp -ne '127.0.0.1|::1') { $failures.Add("ChargeIP must be loopback-only, got '$chargeIp'") }
if ($adminIp -notmatch '(^|\|)127\.0\.0\.1(\||$)') { $failures.Add('AdminIP must include 127.0.0.1') }

try {
    $shop = Invoke-WebRequest -UseBasicParsing -Uri "$LocalBase/ShopItemList.ashx?rnd=$([DateTime]::UtcNow.Ticks)" -TimeoutSec 15
    $shopBody = ([string]$shop.Content).Trim()
    $shopFile = Get-Item (Join-Path $RequestRoot 'ShopItemList.xml') -ErrorAction Stop
    if ($shop.StatusCode -ne 200 -or $shopBody -ne 'Build: ShopItemList.xml, Success!' -or $shopFile.Length -lt 1000) {
        $failures.Add("ShopItemList smoke failed: status=$($shop.StatusCode) bytes=$($shopFile.Length) body='$shopBody'")
    }
} catch { $failures.Add("ShopItemList local request failed: $($_.Exception.Message)") }

$topupTool = Join-Path $repoRoot 'tools\local-topup.ps1'
if (!(Test-Path $topupTool)) {
    $failures.Add('local-topup.ps1 is missing')
} else {
    $probeId = 'local-smoke-' + [Guid]::NewGuid().ToString('N').Substring(0,12)
    $probe = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $topupTool -UserName win -Money 1 -ChargeId $probeId -ValidateOnly 2>&1
    if ($LASTEXITCODE -ne 0 -or (($probe -join "`n") -notmatch 'LOCAL_TOPUP=VALID')) {
        $failures.Add("local-topup validate-only probe failed: exit=$LASTEXITCODE output='$($probe -join ' ')'")
    }
}

try {
    $blocked = Invoke-WebRequest -UseBasicParsing -Uri "$PublicBase/ChargeMoney.aspx?content=x%7Cy%7C1%7Cz%7C0%7Cbad" -TimeoutSec 15
    if (([string]$blocked.Content).Trim() -ne '5') {
        $failures.Add("Public ChargeMoney must be IP-blocked with result 5, got '$(([string]$blocked.Content).Trim())'")
    }
} catch { $failures.Add("Public ChargeMoney probe failed: $($_.Exception.Message)") }

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ -ErrorAction Continue }
    throw "LOCAL_SHOP_TOPUP_SMOKE=FAIL count=$($failures.Count)"
}
Write-Host 'LOCAL_SHOP_TOPUP_SMOKE=PASS'
