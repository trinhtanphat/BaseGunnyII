param(
    [switch]$PlanOnly,
    [switch]$Restart,
    [int]$TimeoutSeconds = 120
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

function Get-ExpectedListenerPid([int]$Port,[string]$ExpectedPath) {
    $expected = [IO.Path]::GetFullPath($ExpectedPath)
    $listeners = @(Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue)
    foreach ($listener in $listeners) {
        $proc = Get-CimInstance Win32_Process -Filter ("ProcessId={0}" -f $listener.OwningProcess) -ErrorAction SilentlyContinue
        if ($proc -and $proc.ExecutablePath -and [IO.Path]::GetFullPath($proc.ExecutablePath) -ieq $expected) {
            return [int]$listener.OwningProcess
        }
    }
    return 0
}
function Wait-ListenPort([int]$Port,[string]$ExpectedPath,[int]$Seconds) {
    $deadline = (Get-Date).AddSeconds($Seconds)
    do {
        $listenerPidCandidate = Get-ExpectedListenerPid $Port $ExpectedPath
        if ($listenerPidCandidate -gt 0) { return $listenerPidCandidate }
        Start-Sleep -Milliseconds 250
    } while ((Get-Date) -lt $deadline)
    return 0
}

function Stop-GunnyProcess([string]$Name) {
    $base = [IO.Path]::GetFileNameWithoutExtension($Name)
    Get-Process -Name $base -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host ("STOP {0} pid={1}" -f $Name,$_.Id)
        Stop-Process -Id $_.Id -Force
        Wait-Process -Id $_.Id -Timeout 10 -ErrorAction SilentlyContinue
    }
}

function Remove-DuplicateGunnyProcesses([string]$Name,[string]$ExpectedPath,[int]$ListenerPid) {
    $expected = [IO.Path]::GetFullPath($ExpectedPath)
    $dupes = @(Get-CimInstance Win32_Process -Filter ("Name='{0}'" -f $Name) -ErrorAction SilentlyContinue |
        Where-Object { $_.ProcessId -ne $ListenerPid -and $_.ExecutablePath -and [IO.Path]::GetFullPath($_.ExecutablePath) -ieq $expected })
    foreach ($proc in $dupes) {
        Write-Host ("STOP_DUPLICATE {0} pid={1} listenerPid={2}" -f $Name,$proc.ProcessId,$ListenerPid)
        Stop-Process -Id $proc.ProcessId -Force -ErrorAction SilentlyContinue
        Wait-Process -Id $proc.ProcessId -Timeout 10 -ErrorAction SilentlyContinue
    }
}
function New-GunnyStartupMutex {
    $name = 'Global\GunnyServerStartOrder'
    $sharedRights = [System.Security.AccessControl.MutexRights]::Synchronize -bor [System.Security.AccessControl.MutexRights]::Modify
    try {
        return [System.Threading.Mutex]::OpenExisting($name,$sharedRights)
    }
    catch [System.Threading.WaitHandleCannotBeOpenedException] {
        # No existing mutex: create it with an ACL shared by SYSTEM and interactive users.
    }

    $security = New-Object System.Security.AccessControl.MutexSecurity
    $authenticatedUsers = New-Object System.Security.Principal.SecurityIdentifier([System.Security.Principal.WellKnownSidType]::AuthenticatedUserSid,$null)
    $system = New-Object System.Security.Principal.SecurityIdentifier([System.Security.Principal.WellKnownSidType]::LocalSystemSid,$null)
    $allow = [System.Security.AccessControl.AccessControlType]::Allow
    $security.AddAccessRule((New-Object System.Security.AccessControl.MutexAccessRule($authenticatedUsers,$sharedRights,$allow)))
    $security.AddAccessRule((New-Object System.Security.AccessControl.MutexAccessRule($system,[System.Security.AccessControl.MutexRights]::FullControl,$allow)))

    $createdNew = $false
    try {
        return [System.Threading.Mutex]::new($false,$name,[ref]$createdNew,$security)
    }
    catch [System.UnauthorizedAccessException] {
        # Another compatible creator may have won the race between OpenExisting and create.
        return [System.Threading.Mutex]::OpenExisting($name,$sharedRights)
    }
}
$mutex = New-GunnyStartupMutex
$lockAcquired = $false
try {
    try {
        $lockAcquired = $mutex.WaitOne(0)
    }
    catch [System.Threading.AbandonedMutexException] {
        $lockAcquired = $true
    }
    if (-not $lockAcquired) {
        throw 'Another Gunny server startup is already running'
    }

    if ($Restart) {
        foreach ($step in @($steps[2],$steps[1],$steps[0])) {
            Stop-GunnyProcess $step.Name
        }
        Start-Sleep -Seconds 1
    }

    foreach ($step in $steps) {
        $base = [IO.Path]::GetFileNameWithoutExtension($step.Name)
        $dir = Join-Path $serverRoot $step.Dir
        $exe = Join-Path $dir $step.Name
        if (-not (Test-Path $exe)) { throw "Missing server executable: $exe" }
        $running = Get-Process -Name $base -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $running) {
            $running = Start-Process -FilePath $exe -WorkingDirectory $dir -PassThru
            Write-Host ("START {0} pid={1}" -f $step.Name,$running.Id)
        } else {
            Write-Host ("ALREADY {0} pid={1}" -f $step.Name,$running.Id)
        }        $listenerPid = Wait-ListenPort $step.Port $exe $TimeoutSeconds
        if ($listenerPid -le 0) {
            throw ("{0} did not listen on port {1}" -f $step.Name,$step.Port)
        }
        Remove-DuplicateGunnyProcesses $step.Name $exe $listenerPid
        Write-Host ("LISTEN {0} port={1} pid={2}" -f $step.Name,$step.Port,$listenerPid)
    }

    $roadExe = Join-Path $serverRoot 'Road\Road.Service.exe'
    $fightExe = Join-Path $serverRoot 'Fight\Fighting.Service.exe'
    $roadPid = Get-ExpectedListenerPid 9200 $roadExe
    $fightPid = Get-ExpectedListenerPid 9208 $fightExe
    if ($roadPid -le 0 -or $fightPid -le 0) { throw 'Road or Fight listener PID missing' }

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $linked = $false
    do {
        $connections = @(Get-NetTCPConnection -State Established -ErrorAction SilentlyContinue |
            Where-Object {
                ($_.OwningProcess -eq $roadPid -and $_.RemotePort -eq 9208) -or
                ($_.OwningProcess -eq $fightPid -and $_.LocalPort -eq 9208)
            })
        if ($connections.Count -ge 2) { $linked = $true; break }
        Start-Sleep -Milliseconds 250
    } while ((Get-Date) -lt $deadline)
    if (-not $linked) { throw 'Road->Fight established connection was not observed' }
    Write-Host ("ROAD_FIGHT_LINK=PASS roadPid={0} fightPid={1}" -f $roadPid,$fightPid)
    Write-Host 'GUNNY_SERVER_START=PASS'
}
finally {
    if ($lockAcquired) {
        try { $mutex.ReleaseMutex() } catch { }
    }
    $mutex.Dispose()
}