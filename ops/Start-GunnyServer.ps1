param(
    [switch]$PlanOnly,
    [switch]$Restart,
    [int]$TimeoutSeconds = 30
)
$ErrorActionPreference = 'Stop'
$serverRoot = 'C:\Gunny\GunnyFileExe\SERVER'
$steps = @(
    @{ Step = 1; Name = 'Center.Service.exe'; Dir = 'center'; Port = 9202 },
    @{ Step = 2; Name = 'Fighting.Service.exe'; Dir = 'Fight'; Port = 9208 },
    @{ Step = 3; Name = 'Road.Service.exe'; Dir = 'Road'; Port = 9200 }
)
foreach ($step in $steps) {
    Write-Host ("STEP {0} {1} port={2}" -f $step.Step,$step.Name,$step.Port)
}
Write-Host 'VERIFY Road->Fight established'
if ($PlanOnly) { exit 0 }

function Wait-ListenPort([int]$Port,[int]$Seconds) {
    $deadline = (Get-Date).AddSeconds($Seconds)
    do {
        $listener = Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue
        if ($listener) { return $true }
        Start-Sleep -Milliseconds 250
    } while ((Get-Date) -lt $deadline)
    return $false
}
function Stop-GunnyProcess([string]$Name) {
    $base = [IO.Path]::GetFileNameWithoutExtension($Name)
    Get-Process -Name $base -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host ("STOP {0} pid={1}" -f $Name,$_.Id)
        Stop-Process -Id $_.Id -Force
        Wait-Process -Id $_.Id -Timeout 10 -ErrorAction SilentlyContinue
    }
}

if ($Restart) {
    foreach ($step in @($steps[2],$steps[1],$steps[0])) {
        Stop-GunnyProcess $step.Name
    }
    Start-Sleep -Seconds 1
}

foreach ($step in $steps) {
    $base = [IO.Path]::GetFileNameWithoutExtension($step.Name)
    $running = Get-Process -Name $base -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $running) {
        $dir = Join-Path $serverRoot $step.Dir
        $exe = Join-Path $dir $step.Name
        if (-not (Test-Path $exe)) { throw "Missing server executable: $exe" }
        $running = Start-Process -FilePath $exe -WorkingDirectory $dir -PassThru
        Write-Host ("START {0} pid={1}" -f $step.Name,$running.Id)
    } else {
        Write-Host ("ALREADY {0} pid={1}" -f $step.Name,$running.Id)
    }
    if (-not (Wait-ListenPort $step.Port $TimeoutSeconds)) {
        throw ("{0} did not listen on port {1}" -f $step.Name,$step.Port)
    }
    Write-Host ("LISTEN {0} port={1}" -f $step.Name,$step.Port)
}
$road = Get-Process -Name 'Road.Service' -ErrorAction Stop | Select-Object -First 1
$fight = Get-Process -Name 'Fighting.Service' -ErrorAction Stop | Select-Object -First 1
$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
$linked = $false
do {
    $connections = @(Get-NetTCPConnection -State Established -ErrorAction SilentlyContinue |
        Where-Object {
            ($_.OwningProcess -eq $road.Id -and $_.RemotePort -eq 9208) -or
            ($_.OwningProcess -eq $fight.Id -and $_.LocalPort -eq 9208)
        })
    if ($connections.Count -ge 2) { $linked = $true; break }
    Start-Sleep -Milliseconds 250
} while ((Get-Date) -lt $deadline)
if (-not $linked) { throw 'Road->Fight established connection was not observed' }
Write-Host ("ROAD_FIGHT_LINK=PASS roadPid={0} fightPid={1}" -f $road.Id,$fight.Id)
Write-Host 'GUNNY_SERVER_START=PASS'
