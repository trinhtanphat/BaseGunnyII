$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'local-topup.ps1'
$tokens = $null; $errors = $null
$ast = [System.Management.Automation.Language.Parser]::ParseFile($scriptPath,[ref]$tokens,[ref]$errors)
if ($errors.Count -gt 0) { throw "local-topup.ps1 parse errors: $($errors.Count)" }
$paramNames = @($ast.ParamBlock.Parameters | ForEach-Object { $_.Name.VariablePath.UserPath })
if ($paramNames -contains 'CenterEndpoint') { throw 'unused CenterEndpoint parameter must be removed' }
$auditAssign = $ast.Find({ param($n) $n -is [System.Management.Automation.Language.AssignmentStatementAst] -and $n.Left.Extent.Text -eq '$auditLine' }, $true)
if ($null -eq $auditAssign) { throw 'auditLine assignment is missing' }
$auditString = @($auditAssign.Right.FindAll({ param($n) $n -is [System.Management.Automation.Language.StringConstantExpressionAst] }, $true))[0]
if ($null -eq $auditString -or $auditString.Value.IndexOf([char]9) -lt 0) { throw 'auditLine must contain real tab separators' }
Write-Output 'LOCAL_TOPUP_MAINTENANCE_SMOKE=PASS'
