$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$baseGame = Get-Content (Join-Path $root 'Game.Logic\BaseGame.cs') -Raw
$player = Get-Content (Join-Path $root 'Game.Logic\Phy\Object\Player.cs') -Raw
function Assert-True([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}
$tokens = @(
    'pkg.WriteInt(p.Energy);',
    'pkg.WriteInt(p.psychic);',
    'pkg.WriteInt(p.Dander);',
    'pkg.WriteInt(p.PetMaxMP);',
    'pkg.WriteInt(p.PetMP);',
    'pkg.WriteInt(p.ShootCount);',
    'pkg.WriteInt(p.flyCount);'
)
$last = -1
foreach ($token in $tokens) {
    $pos = $baseGame.IndexOf($token)
    Assert-True ($pos -gt $last) "TURN packet field missing or out of order: $token"
    $last = $pos
}
Assert-True ($player.Contains('public int flyCount')) 'Player.flyCount protocol property missing'
Assert-True ($player.Contains('get { return m_flyCoolDown; }')) 'Player.flyCount must expose the live fly cooldown'
Write-Output 'PVP_TURN_PACKET_CONTRACT_SMOKE=PASS'