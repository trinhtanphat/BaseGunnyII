$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$path = Join-Path $root 'Fighting.Server\Servers\ServerClient.cs'
$text = Get-Content $path -Raw
$sender = Get-Content (Join-Path $root 'Game.Server\Battle\FightServerConnector.cs') -Raw
if ($text -notmatch 'const\s+int\s+MaxRoomPlayers\s*=\s*8') { throw 'ROOM_CREATE must cap players at the server room maximum before allocation' }
if ($text -notmatch 'count\s*<\s*1\s*\|\|\s*count\s*>\s*MaxRoomPlayers') { throw 'ROOM_CREATE player count is not validated' }
$guard = $text.IndexOf('count < 1 || count > MaxRoomPlayers')
$alloc = $text.IndexOf('new IGamePlayer[count]')
if ($guard -lt 0 -or $alloc -lt 0 -or $guard -gt $alloc) { throw 'ROOM_CREATE count guard must run before the player array allocation' }
if ($text -notmatch 'buffercout\s*<\s*0\s*\|\|\s*buffercout\s*>\s*pkg\.DataLeft\s*/\s*24') { throw 'ROOM_CREATE buffer count is not bounded by bytes remaining after the count field' }
if ($text -notmatch 'ec\s*<\s*0\s*\|\|\s*ec\s*>\s*pkg\.DataLeft\s*/\s*4') { throw 'ROOM_CREATE equip-effect count is not bounded by remaining wire bytes' }
if ($sender -notmatch 'WriteInt\(info\.ValidCount\)') { throw 'Sender no longer contains BufferInfo.ValidCount on the wire' }
if ($text -notmatch 'buffinfo\.ValidCount\s*=\s*pkg\.ReadInt\(\)') { throw 'Receiver does not consume BufferInfo.ValidCount, desynchronizing ROOM_CREATE' }
if ($text -notmatch 'RejectRoomCreate\(') { throw 'Malformed ROOM_CREATE packets must use a fail-closed rejection path' }
Write-Host 'FIGHT_ROOM_PACKET_GUARD_SMOKE=PASS'
