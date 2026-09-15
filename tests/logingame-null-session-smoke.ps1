$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$sourcePath = Join-Path $root 'Tank.Flash\LoginGame.aspx.cs'
$source = Get-Content -LiteralPath $sourcePath -Raw

$unsafe = 'if ((Session["username"] == null) && string.IsNullOrEmpty(Session["username"].ToString()))'
$safe = 'if ((Session["username"] == null) || string.IsNullOrEmpty(Session["username"].ToString()))'
if ($source.Contains($unsafe)) {
    throw 'LoginGame dereferences Session["username"] when the session value is null'
}
if (-not $source.Contains($safe)) {
    throw 'LoginGame must short-circuit the empty-username check when Session["username"] is null'
}
if ($source -notmatch 'Response\.Redirect\("~/Login\.htm",\s*false\);\s*return;') {
    throw 'LoginGame must stop request execution immediately after redirecting an anonymous session'
}

Write-Host 'LOGIN_GAME_NULL_SESSION_SMOKE=PASS'
