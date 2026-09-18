/*
  VIP 20 migration for the Gunny 5-year server.
  - Keeps the live VIP 1-12 cumulative EXP thresholds unchanged.
  - Extends the ladder to VIP 20.
  - Normalizes invalid VIP EXP floors so the client never renders negative progress.
  - Replaces the legacy renewal proc that reset VIPLevel to 1.
  - Aligns SP_UpdateVIPInfo with PlayerBussiness.UpdateVIPInfo.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

UPDATE dbo.Server_Config SET Value='20' WHERE Name='VIPMaxLevel';
UPDATE dbo.Server_Config
SET Value='0|200|400|800|2000|4000|8000|20000|40000|80000|200000|400000|800000|1200000|1800000|2600000|3600000|4800000|6200000|7800000'
WHERE Name='VIPExpForEachLv';

-- VIPExpNeededForEachLv is a legacy 1-9 tuning table used by older UI paths.
-- Keep its existing semantics untouched; the VIP20 client reads VIPExpForEachLv.

UPDATE dbo.Server_Config SET Value='1.1|1.2|1.3|1.4|1.5|1.8|1.8|1.8|1.8|1.9|1.9|2.0|2.0|2.0|2.0|2.0|2.0|2.0|2.0|2.0' WHERE Name='VIPRateForGP';
UPDATE dbo.Server_Config SET Value='3|4|5|6|7|8|9|10|11|12|13|15|15|15|15|15|15|15|15|15' WHERE Name='VIPLotteryCountMaxPerDay';
UPDATE dbo.Server_Config SET Value='0|0|0|9999|9999|12999|12999|12999|12999|19999|19999|19999|19999|19999|19999|19999|19999|19999|19999|19999' WHERE Name='VIPExtraBindMoneyUpper';
UPDATE dbo.Server_Config SET Value='0|0|0|0|0|2|2|2|2|3|3|4|4|4|4|4|4|4|4|4' WHERE Name='VIPQuestFinishDirect';
UPDATE dbo.Server_Config SET Value='2|2|2|2|2|3|3|3|3|3|3|3|3|3|3|3|3|3|3|3' WHERE Name='VIPQuestStar';
UPDATE dbo.Server_Config SET Value='25|25|25|35|35|50|50|50|50|50|50|50|50|50|50|50|50|50|50|50' WHERE Name='VIPStrengthenEx';
UPDATE dbo.Server_Config SET Value='0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0' WHERE Name='VIPOfferDecreaseRate';
UPDATE dbo.Server_Config SET Value='112112|112113|112114|112115|112116|112117|112118|112119|112120|112204|112205|112206|112206|112206|112206|112206|112206|112206|112206|112206' WHERE Name='VIPDailyPackID';
UPDATE dbo.Server_Config SET Value='100|100|100|90|90|80|80|80|80|80|70|70|70|70|70|70|70|70|70|70' WHERE Name='VIPTakeCardDisCount';
UPDATE dbo.Server_Config SET Value='120|120|120|120|120|50|50|50|50|30|30|30|30|30|30|30|30|30|30|30' WHERE Name='VIPPayAimEnergy';
UPDATE dbo.Server_Config SET Value='0,|0,|1,|2,|3,|4,13,|5,11,|6,|7,8,|10,|9,|12,|12,|12,|12,|12,|12,|12,|12,' WHERE Name='VIPPrivilege';

UPDATE dbo.Sys_VIP_Info
SET VIPLevel = CASE WHEN VIPLevel < 1 THEN 1 WHEN VIPLevel > 20 THEN 20 ELSE VIPLevel END;

UPDATE dbo.Sys_VIP_Info
SET VIPExp = CASE
    WHEN VIPLevel=1  AND VIPExp < 0       THEN 0
    WHEN VIPLevel=2  AND VIPExp < 200     THEN 200
    WHEN VIPLevel=3  AND VIPExp < 400     THEN 400
    WHEN VIPLevel=4  AND VIPExp < 800     THEN 800
    WHEN VIPLevel=5  AND VIPExp < 2000    THEN 2000
    WHEN VIPLevel=6  AND VIPExp < 4000    THEN 4000
    WHEN VIPLevel=7  AND VIPExp < 8000    THEN 8000
    WHEN VIPLevel=8  AND VIPExp < 20000   THEN 20000
    WHEN VIPLevel=9  AND VIPExp < 40000   THEN 40000
    WHEN VIPLevel=10 AND VIPExp < 80000   THEN 80000
    WHEN VIPLevel=11 AND VIPExp < 200000  THEN 200000
    WHEN VIPLevel=12 AND VIPExp < 400000  THEN 400000
    WHEN VIPLevel=13 AND VIPExp < 800000  THEN 800000
    WHEN VIPLevel=14 AND VIPExp < 1200000 THEN 1200000
    WHEN VIPLevel=15 AND VIPExp < 1800000 THEN 1800000
    WHEN VIPLevel=16 AND VIPExp < 2600000 THEN 2600000
    WHEN VIPLevel=17 AND VIPExp < 3600000 THEN 3600000
    WHEN VIPLevel=18 AND VIPExp < 4800000 THEN 4800000
    WHEN VIPLevel=19 AND VIPExp < 6200000 THEN 6200000
    WHEN VIPLevel=20 AND VIPExp < 7800000 THEN 7800000
    ELSE VIPExp END;

UPDATE dbo.Sys_VIP_Info
SET VIPNextLevelDaysNeeded = CASE VIPLevel
    WHEN 1 THEN CASE WHEN VIPExp>=200 THEN 0 ELSE (200-VIPExp+9)/10 END
    WHEN 2 THEN CASE WHEN VIPExp>=400 THEN 0 ELSE (400-VIPExp+9)/10 END
    WHEN 3 THEN CASE WHEN VIPExp>=800 THEN 0 ELSE (800-VIPExp+9)/10 END
    WHEN 4 THEN CASE WHEN VIPExp>=2000 THEN 0 ELSE (2000-VIPExp+9)/10 END
    WHEN 5 THEN CASE WHEN VIPExp>=4000 THEN 0 ELSE (4000-VIPExp+9)/10 END
    WHEN 6 THEN CASE WHEN VIPExp>=8000 THEN 0 ELSE (8000-VIPExp+9)/10 END
    WHEN 7 THEN CASE WHEN VIPExp>=20000 THEN 0 ELSE (20000-VIPExp+9)/10 END
    WHEN 8 THEN CASE WHEN VIPExp>=40000 THEN 0 ELSE (40000-VIPExp+9)/10 END
    WHEN 9 THEN CASE WHEN VIPExp>=80000 THEN 0 ELSE (80000-VIPExp+9)/10 END
    WHEN 10 THEN CASE WHEN VIPExp>=200000 THEN 0 ELSE (200000-VIPExp+9)/10 END
    WHEN 11 THEN CASE WHEN VIPExp>=400000 THEN 0 ELSE (400000-VIPExp+9)/10 END
    WHEN 12 THEN CASE WHEN VIPExp>=800000 THEN 0 ELSE (800000-VIPExp+9)/10 END
    WHEN 13 THEN CASE WHEN VIPExp>=1200000 THEN 0 ELSE (1200000-VIPExp+9)/10 END
    WHEN 14 THEN CASE WHEN VIPExp>=1800000 THEN 0 ELSE (1800000-VIPExp+9)/10 END
    WHEN 15 THEN CASE WHEN VIPExp>=2600000 THEN 0 ELSE (2600000-VIPExp+9)/10 END
    WHEN 16 THEN CASE WHEN VIPExp>=3600000 THEN 0 ELSE (3600000-VIPExp+9)/10 END
    WHEN 17 THEN CASE WHEN VIPExp>=4800000 THEN 0 ELSE (4800000-VIPExp+9)/10 END
    WHEN 18 THEN CASE WHEN VIPExp>=6200000 THEN 0 ELSE (6200000-VIPExp+9)/10 END
    WHEN 19 THEN CASE WHEN VIPExp>=7800000 THEN 0 ELSE (7800000-VIPExp+9)/10 END
    ELSE 0 END;

COMMIT TRANSACTION;
GO

CREATE OR ALTER PROCEDURE dbo.SP_VIPRenewal_Single
    @UserID int,
    @RenewalDays int,
    @ExpireDayOut datetime OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @RenewalDays <= 0 OR @RenewalDays > 744
        RETURN -2;

    IF NOT EXISTS (SELECT 1 FROM dbo.Sys_Users_Detail WHERE UserID=@UserID AND IsExist=1)
        RETURN -1;

    DECLARE @Now datetime = GETDATE();
    DECLARE @CurrentExpire datetime;

    BEGIN TRANSACTION;
    SELECT @CurrentExpire=VIPExpireDay
    FROM dbo.Sys_VIP_Info WITH (UPDLOCK,HOLDLOCK)
    WHERE UserID=@UserID;

    SET @ExpireDayOut = DATEADD(day,@RenewalDays,
        CASE WHEN @CurrentExpire IS NOT NULL AND @CurrentExpire>@Now THEN @CurrentExpire ELSE @Now END);

    IF @CurrentExpire IS NULL
    BEGIN
        INSERT dbo.Sys_VIP_Info
            (UserID,typeVIP,VIPLevel,VIPExp,VIPOnlineDays,VIPOfflineDays,VIPExpireDay,
             LastVIPPackTime,VIPLastdate,VIPNextLevelDaysNeeded,CanTakeVipReward)
        VALUES
            (@UserID,1,1,0,0,0,@ExpireDayOut,@Now,@Now,20,1);
    END
    ELSE
    BEGIN
        UPDATE dbo.Sys_VIP_Info
        SET typeVIP=CASE WHEN typeVIP<1 THEN 1 ELSE typeVIP END,
            VIPExpireDay=@ExpireDayOut,
            LastVIPPackTime=@Now,
            VIPLastdate=@Now,
            CanTakeVipReward=1
        WHERE UserID=@UserID;
    END

    COMMIT TRANSACTION;
    RETURN 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_UpdateVIPInfo
    @UserID int,
    @typeVIP int,
    @VIPLevel int,
    @VIPExp int,
    @VIPOnlineDays int,
    @VIPOfflineDays int,
    @VIPExpireDay datetime,
    @VIPLastDate datetime,
    @VIPNextLevelDaysNeeded int,
    @CanTakeVipReward bit
AS
BEGIN
    SET NOCOUNT ON;

    SET @VIPLevel = CASE WHEN @VIPLevel<1 THEN 1 WHEN @VIPLevel>20 THEN 20 ELSE @VIPLevel END;

    DECLARE @FloorExp int = CASE @VIPLevel
        WHEN 1 THEN 0 WHEN 2 THEN 200 WHEN 3 THEN 400 WHEN 4 THEN 800
        WHEN 5 THEN 2000 WHEN 6 THEN 4000 WHEN 7 THEN 8000 WHEN 8 THEN 20000
        WHEN 9 THEN 40000 WHEN 10 THEN 80000 WHEN 11 THEN 200000 WHEN 12 THEN 400000
        WHEN 13 THEN 800000 WHEN 14 THEN 1200000 WHEN 15 THEN 1800000 WHEN 16 THEN 2600000
        WHEN 17 THEN 3600000 WHEN 18 THEN 4800000 WHEN 19 THEN 6200000 WHEN 20 THEN 7800000
        ELSE 0 END;
    IF @VIPExp < @FloorExp SET @VIPExp=@FloorExp;
    IF @VIPNextLevelDaysNeeded < 0 SET @VIPNextLevelDaysNeeded=0;

    UPDATE dbo.Sys_VIP_Info
    SET typeVIP=@typeVIP,
        VIPLevel=@VIPLevel,
        VIPExp=@VIPExp,
        VIPOnlineDays=@VIPOnlineDays,
        VIPOfflineDays=@VIPOfflineDays,
        VIPExpireDay=@VIPExpireDay,
        VIPLastdate=@VIPLastDate,
        VIPNextLevelDaysNeeded=@VIPNextLevelDaysNeeded,
        CanTakeVipReward=@CanTakeVipReward
    WHERE UserID=@UserID;

    IF @@ROWCOUNT=0
    BEGIN
        INSERT dbo.Sys_VIP_Info
            (UserID,typeVIP,VIPLevel,VIPExp,VIPOnlineDays,VIPOfflineDays,VIPExpireDay,
             LastVIPPackTime,VIPLastdate,VIPNextLevelDaysNeeded,CanTakeVipReward)
        VALUES
            (@UserID,@typeVIP,@VIPLevel,@VIPExp,@VIPOnlineDays,@VIPOfflineDays,@VIPExpireDay,
             GETDATE(),@VIPLastDate,@VIPNextLevelDaysNeeded,@CanTakeVipReward);
    END

    RETURN 0;
END
GO
