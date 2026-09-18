$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

function Require([bool]$condition, [string]$message) {
    if (-not $condition) { throw "VIP20_SERVER_SMOKE_FAIL: $message" }
}

$player = Get-Content (Join-Path $root 'SqlDataProvider\Data\PlayerInfo.cs') -Raw
$gamePlayer = Get-Content (Join-Path $root 'Game.Server\GameObjects\GamePlayer.cs') -Raw
$handler = Get-Content (Join-Path $root 'Game.Server\Packets\Client\OpenVipHandler.cs') -Raw
$business = Get-Content (Join-Path $root 'Bussiness\PlayerBussiness.cs') -Raw
$migration = Get-Content (Join-Path $root 'database\20260918_vip20.sql') -Raw

Require ($player -match 'MaxVipLevel\s*=\s*20') 'server max VIP must be 20'
Require ($player -match '7800000') 'VIP20 cumulative EXP floor is missing'
Require ($player -match 'EnsureVipExpFloor') 'VIP EXP floor normalization helper is missing'
Require ($player -match 'while\s*\(VIPLevel\s*<\s*MaxVipLevel') 'VIP level-up loop is missing'
Require ($gamePlayer -match 'db\.UpdateVIPInfo\(m_character\)') 'normalized VIP state must persist after login update'
Require ($gamePlayer -match 'Math\.Max\(0,\s*m_character\.VIPExp\s*-\s*5\)') 'expired VIP EXP decay must be clamped at zero'
Require ($handler -match 'requestedNickName.*PlayerCharacter\.NickName') 'renewal nickname ownership check is missing'
Require ($handler -match 'PayWithGold') 'Gold payment mode is missing'
Require ($handler -match 'FindShopbyTemplatID\(VipTemplateId\)') 'VIP product must be located by template ID'
Require ($handler -match 'AddGold\(charge\)') 'Gold refund path is missing'
Require ($handler -match 'AddMoney\(charge\)') 'Xu refund path is missing'
Require ($business -match 'VIPRenewal\(int userId, int renewalDays, out DateTime expireDay\)') 'VIP renewal must use authenticated user ID'
Require ($business -match 'new SqlParameter\("@typeVIP"') 'VIP type persistence is missing'
Require ($migration -match "VIPMaxLevel'.*20") 'database VIPMaxLevel migration is missing'
Require ($migration -match 'CREATE OR ALTER PROCEDURE dbo\.SP_VIPRenewal_Single') 'safe renewal procedure migration is missing'
Require ($migration -notmatch 'SET\s+VIPLevel\s*=\s*1') 'renewal migration must not reset existing VIP level'
Require ($migration -match 'VIPExp=@VIPExp') 'VIP EXP persistence migration is missing'

Write-Host 'VIP20_SERVER_SMOKE=PASS'
