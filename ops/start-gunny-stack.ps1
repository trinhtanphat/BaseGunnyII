param([switch]$SkipDelay)
$ErrorActionPreference='Stop'
$logDir='C:\Gunny\logs'
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
$log=Join-Path $logDir 'GunnyStackStartup.log'
$launcher='C:\Gunny\_ops\Start-GunnyServer.ps1'
function Log([string]$m){$line="$(Get-Date -Format s) $m"; $line | Tee-Object -FilePath $log -Append}
function PortUp([int]$p){[bool](Get-NetTCPConnection -State Listen -LocalPort $p -ErrorAction SilentlyContinue)}
if(!$SkipDelay){Start-Sleep -Seconds 20}
$svc=Get-Service 'MSSQL$SQLEXPRESS'
if($svc.Status -ne 'Running'){
  Start-Service $svc.Name
  $svc.WaitForStatus('Running','00:01:00')
}
Log 'SQL ready'
if(!(Test-Path $launcher)){throw "Canonical launcher missing: $launcher"}
try {
  & $launcher -TimeoutSeconds 120 *>&1 | Tee-Object -FilePath $log -Append
}
catch {
  if($_.Exception.Message -like '*Another Gunny server startup is already running*'){
    Log 'Canonical launcher already owns startup mutex; watchdog exits cleanly'
    exit 0
  }
  throw
}
if(!(PortUp 2009)){throw 'Center WCF port 2009 is not listening'}
Log 'GUNNY_STACK_READY ports=2009,9200,9202,9208'
