$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$pvp = Get-Content (Join-Path $root 'Game.Logic\PVPGame.cs') -Raw
if ($pvp -notmatch 'p\.GainGP\s*=\s*p\.PlayerDetail\.AddGP\(gp\)') { throw 'PVP GameOver does not persist calculated GP.' }
if ($pvp -notmatch 'p\.GainOffer\s*=\s*p\.PlayerDetail\.AddOffer\(') { throw 'PVP GameOver does not calculate winner offer reward.' }
if ($pvp -match 'p\.CanTakeOut\s*=\s*hasBot\s*\?\s*0') { throw 'Bot matches still disable card rewards for humans.' }
if ($pvp -notmatch 'if\s*\(!isBot\)\s*\{[\s\S]*?p\.PlayerDetail\.OnGameOver') { throw 'Real players are not notified at GameOver when a bot is present.' }
$proxy = Get-Content (Join-Path $root 'Fighting.Server\GameObjects\ProxyPlayer.cs') -Raw
if ($proxy -notmatch 'SendPlayerAddOffer\(m_character\.ID,\s*value\)') { throw 'Offer reward is not forwarded to Game.Server.' }
$protocol = Get-Content (Join-Path $root 'Game.Logic\Protocol\eFightPackageType.cs') -Raw
if ($protocol -notmatch 'PLAYER_ADD_OFFER\s*=') { throw 'PLAYER_ADD_OFFER protocol code is missing.' }
$connector = Get-Content (Join-Path $root 'Game.Server\Battle\FightServerConnector.cs') -Raw
if ($connector -notmatch 'case\s*\(int\)eFightPackageType\.PLAYER_ADD_OFFER') { throw 'Game.Server does not route PLAYER_ADD_OFFER.' }
if ($connector -notmatch 'player\.AddOffer\(pkg\.Parameter1\)') { throw 'Game.Server does not persist forwarded offer rewards.' }
Write-Output 'PVP_HUMAN_REWARDS_SMOKE=PASS'
