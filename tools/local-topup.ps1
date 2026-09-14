param(
    [Parameter(Mandatory=$true)][string]$UserName,
    [Parameter(Mandatory=$true)][int]$Money,
    [string]$ChargeId,
    [switch]$ValidateOnly,
    [string]$RequestRoot = 'C:\Gunny\GunnyFileExe\request',
    [string]$SqlServer = '.\SQLEXPRESS',
    [string]$Database = 'Db_Tank',
    [string]$CenterEndpoint = 'net.tcp://127.0.0.1:2009/',
    [string]$AuditPath = 'C:\Gunny\logs\local-topup-audit.log'
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ($UserName -notmatch '^[A-Za-z0-9_.-]{1,64}$') { throw 'Invalid UserName.' }
if ($Money -lt 1 -or $Money -gt 1000000) { throw 'Money must be between 1 and 1000000.' }
if ([string]::IsNullOrWhiteSpace($ChargeId)) {
    $ChargeId = 'local-' + (Get-Date).ToUniversalTime().ToString('yyyyMMddHHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0,8)
}
if ($ChargeId -notmatch '^local-[A-Za-z0-9-]{8,44}$' -or $ChargeId.Length -gt 50) { throw 'Invalid ChargeId.' }

$connectionString = "Data Source=$SqlServer;Initial Catalog=$Database;Integrated Security=True;MultipleActiveResultSets=True"
$conn = New-Object System.Data.SqlClient.SqlConnection $connectionString
$conn.Open()
try {
    $lookup = $conn.CreateCommand()
    $lookup.CommandText = 'SELECT TOP 1 UserID,UserName,NickName,Money FROM dbo.Sys_Users_Detail WHERE UserName=@u'
    [void]$lookup.Parameters.Add('@u',[System.Data.SqlDbType]::NVarChar,200)
    $lookup.Parameters['@u'].Value = $UserName
    $reader = $lookup.ExecuteReader()
    if (!$reader.Read()) {
        $reader.Close()
        Write-Host 'LOCAL_TOPUP=USER_NOT_FOUND'
        exit 2
    }
    $userId = [int]$reader['UserID']
    $nickName = [string]$reader['NickName']
    $beforeMoney = [int]$reader['Money']
    $reader.Close()

    if ($ValidateOnly) {
        if (!(Test-Path (Join-Path $RequestRoot 'bin\Bussiness.dll'))) { throw 'Bussiness.dll not found.' }
        Write-Host "LOCAL_TOPUP=VALID user=$UserName userId=$userId money=$Money chargeId=$ChargeId"
        exit 0
    }

    $existing = $conn.CreateCommand()
    $existing.CommandText = 'SELECT TOP 1 UserName,Money,PayWay,CanUse FROM dbo.Charge_Money WHERE ChargeID=@id'
    [void]$existing.Parameters.Add('@id',[System.Data.SqlDbType]::VarChar,50)
    $existing.Parameters['@id'].Value = $ChargeId
    $existingReader = $existing.ExecuteReader()
    $alreadyExists = $existingReader.Read()
    if ($alreadyExists) {
        $existingUser = [string]$existingReader['UserName']
        $existingMoney = [int]$existingReader['Money']
        $existingPayWay = [string]$existingReader['PayWay']
        $existingCanUse = [int]$existingReader['CanUse']
    }
    $existingReader.Close()

    $created = $false
    if ($alreadyExists) {
        if ($existingUser -ne $UserName -or $existingMoney -ne $Money -or $existingPayWay -ne 'LOCAL_ADMIN') {
            throw 'ChargeId already exists with different transaction data.'
        }
    } else {
        $cmd = $conn.CreateCommand()
        $cmd.CommandType = [System.Data.CommandType]::StoredProcedure
        $cmd.CommandText = 'SP_Charge_Money_Add'
        [void]$cmd.Parameters.Add('@ChargeID',[System.Data.SqlDbType]::VarChar,50)
        [void]$cmd.Parameters.Add('@UserName',[System.Data.SqlDbType]::NVarChar,200)
        [void]$cmd.Parameters.Add('@Money',[System.Data.SqlDbType]::Int)
        [void]$cmd.Parameters.Add('@Date',[System.Data.SqlDbType]::NVarChar,50)
        [void]$cmd.Parameters.Add('@PayWay',[System.Data.SqlDbType]::NVarChar,200)
        [void]$cmd.Parameters.Add('@NeedMoney',[System.Data.SqlDbType]::Decimal)
        $uidParam = $cmd.Parameters.Add('@UserID',[System.Data.SqlDbType]::Int)
        $uidParam.Direction = [System.Data.ParameterDirection]::InputOutput
        [void]$cmd.Parameters.Add('@IP',[System.Data.SqlDbType]::NVarChar,50)
        [void]$cmd.Parameters.Add('@NickName',[System.Data.SqlDbType]::NVarChar,200)
        $returnParam = $cmd.Parameters.Add('@RETURN_VALUE',[System.Data.SqlDbType]::Int)
        $returnParam.Direction = [System.Data.ParameterDirection]::ReturnValue

        $cmd.Parameters['@ChargeID'].Value = $ChargeId
        $cmd.Parameters['@UserName'].Value = $UserName
        $cmd.Parameters['@Money'].Value = $Money
        $cmd.Parameters['@Date'].Value = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss')
        $cmd.Parameters['@PayWay'].Value = 'LOCAL_ADMIN'
        $cmd.Parameters['@NeedMoney'].Value = [decimal]0
        $cmd.Parameters['@UserID'].Value = 0
        $cmd.Parameters['@IP'].Value = '127.0.0.1'
        $cmd.Parameters['@NickName'].Value = $nickName
        [void]$cmd.ExecuteNonQuery()
        $resultCode = [int]$returnParam.Value
        if ($resultCode -ne 0) { throw "SP_Charge_Money_Add failed with code $resultCode." }
        $created = $true
    }

    $auditDir = Split-Path $AuditPath -Parent
    if (!(Test-Path $auditDir)) { [void][System.IO.Directory]::CreateDirectory($auditDir) }
    $state = $conn.CreateCommand()
    $state.CommandText = 'SELECT d.Money,c.CanUse FROM dbo.Sys_Users_Detail d CROSS JOIN dbo.Charge_Money c WHERE d.UserName=@u AND c.ChargeID=@id'
    [void]$state.Parameters.Add('@u',[System.Data.SqlDbType]::NVarChar,200)
    [void]$state.Parameters.Add('@id',[System.Data.SqlDbType]::VarChar,50)
    $state.Parameters['@u'].Value = $UserName
    $state.Parameters['@id'].Value = $ChargeId
    $stateReader = $state.ExecuteReader()
    if (!$stateReader.Read()) { throw 'Charge row missing after enqueue.' }
    $afterMoney = [int]$stateReader['Money']
    $canUse = [int]$stateReader['CanUse']
    $stateReader.Close()

    $status = if ($canUse -eq 1) { 'QUEUED_RELOGIN' } else { 'ALREADY_APPLIED' }
    $auditLine = '{0}`t{1}`t{2}`t{3}`tcreated={4}`tstatus={5}`tbefore={6}`tafter={7}' -f (Get-Date).ToUniversalTime().ToString('o'), $ChargeId, $UserName, $Money, $created, $status, $beforeMoney, $afterMoney
    Add-Content -LiteralPath $AuditPath -Value $auditLine -Encoding UTF8
    Write-Host "LOCAL_TOPUP=OK chargeId=$ChargeId user=$UserName amount=$Money status=$status created=$created before=$beforeMoney after=$afterMoney"
}
finally {
    if ($null -ne $conn) { $conn.Close(); $conn.Dispose() }
}
