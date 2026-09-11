param(
    [string]$Destination = "$PSScriptRoot\_deps\ruffle-android"
)

$ErrorActionPreference = 'Stop'
$revision = (Get-Content "$PSScriptRoot\ruffle-android.revision" -Raw).Trim()
if (Test-Path $Destination) {
    throw "Destination already exists: $Destination"
}

git clone https://github.com/ruffle-rs/ruffle-android.git $Destination
if ($LASTEXITCODE -ne 0) { throw 'git clone failed' }

git -C $Destination checkout --detach $revision
if ($LASTEXITCODE -ne 0) { throw 'git checkout failed' }

$actual = (git -C $Destination rev-parse HEAD).Trim()
if ($actual -ne $revision) {
    throw "Pinned revision mismatch: expected=$revision actual=$actual"
}
Write-Output "RUFFLE_ANDROID_REVISION=$actual"
