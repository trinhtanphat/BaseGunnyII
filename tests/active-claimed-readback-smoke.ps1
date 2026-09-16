$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$handler = Get-Content (Join-Path $root 'Tank.Request\UserGetActiveState.ashx.cs') -Raw

function Require([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}

Require (-not $handler.Contains('Hello World')) 'stub UserGetActiveState still present'
Require ($handler.Contains('context.Request["selfid"]')) 'selfid is not read'
Require ($handler.Contains('context.Request["activeID"]')) 'activeID is not read'
Require ($handler.Contains('context.Request["key"]')) 'request key is not read'
Require ($handler.Contains('GetUserSingleByUserID')) 'user lookup/auth source missing'
Require ($handler.Contains('MD5.Create()')) 'password MD5 verification missing'
Require ($handler.Contains('StringComparison.OrdinalIgnoreCase')) 'key comparison must be case-insensitive'
Require ($handler.Contains('Active_Number')) 'persistent claim table is not queried'
Require ($handler.Contains('PullDown = 1')) 'claim query must require persisted PullDown=1'
Require ($handler.Contains('@ActiveID')) 'parameterized ActiveID query missing'
Require ($handler.Contains('@UserID')) 'parameterized UserID query missing'
Require ($handler.Contains('new XElement("Result"')) 'XML Result response missing'
Require ($handler.Contains('new XAttribute("value"')) 'response value attribute missing'
Require ($handler.Contains('new XAttribute("isAttend"')) 'isAttend attribute missing'
Require ($handler.Contains('new XAttribute("activeID"')) 'activeID echo missing'
Require ($handler.Contains('ContentType = "text/plain"')) 'response content type drifted'

Write-Output 'ACTIVE_CLAIMED_READBACK_SMOKE=PASS'
