ALTER PROCEDURE [dbo].[SP_DailyLogList_Update]
    @UserID int,
    @UserAwardLog int,
    @DayLog nvarchar(2000),
    @LastDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now datetime = GETDATE();
    DECLARE @Today int = DAY(@Now);
    DECLARE @StoredDayLog nvarchar(2000);
    DECLARE @StoredAwardLog int;
    DECLARE @StoredLastDate datetime;
    DECLARE @Exists bit = 0;

    BEGIN TRANSACTION;

    SELECT
        @StoredDayLog = DayLog,
        @StoredAwardLog = UserAwardLog,
        @StoredLastDate = LastDate,
        @Exists = 1
    FROM dbo.DailyLogList WITH (UPDLOCK, HOLDLOCK)
    WHERE UserID = @UserID;
    IF @Exists = 0
    BEGIN
        SET @StoredDayLog = N'';
        SET @StoredAwardLog = 0;
        SET @StoredLastDate = @Now;
    END
    ELSE IF YEAR(@StoredLastDate) <> YEAR(@Now)
         OR MONTH(@StoredLastDate) <> MONTH(@Now)
    BEGIN
        SET @StoredDayLog = N'';
        SET @StoredAwardLog = 0;
        SET @StoredLastDate = @Now;
    END

    DECLARE @Rest nvarchar(2000) = ISNULL(@StoredDayLog, N'');
    DECLARE @Normalized nvarchar(2000) = N'';
    DECLARE @Index int = 1;
    DECLARE @Comma int;
    DECLARE @Token nvarchar(20);
    DECLARE @ExistingTrueCount int = 0;
    DECLARE @TodayChecked bit = 0;

    WHILE @Index <= @Today
    BEGIN
        IF LEN(@Rest) = 0
        BEGIN
            SET @Token = N'';
        END
        ELSE
        BEGIN
            SET @Comma = CHARINDEX(N',', @Rest);
            IF @Comma = 0
            BEGIN
                SET @Token = @Rest;
                SET @Rest = N'';
            END
            ELSE
            BEGIN
                SET @Token = LEFT(@Rest, @Comma - 1);
                SET @Rest = SUBSTRING(@Rest, @Comma + 1, 2000);
            END
        END

        SET @Token = CASE
            WHEN LOWER(LTRIM(RTRIM(ISNULL(@Token, N'')))) = N'true' THEN N'True'
            ELSE N'False'
        END;

        IF @Index < @Today
        BEGIN
            SET @Normalized = @Normalized
                + CASE WHEN LEN(@Normalized) = 0 THEN N'' ELSE N',' END
                + @Token;
            IF @Token = N'True'
                SET @ExistingTrueCount = @ExistingTrueCount + 1;
        END
        ELSE IF @Token = N'True'
        BEGIN
            SET @TodayChecked = 1;
            SET @ExistingTrueCount = @ExistingTrueCount + 1;
        END

        SET @Index = @Index + 1;
    END

    DECLARE @RequestedDayLog nvarchar(2000) = ISNULL(@DayLog, N'');
    DECLARE @RequestedTrueCount int =
        (LEN(@RequestedDayLog) - LEN(REPLACE(LOWER(@RequestedDayLog), N'true', N''))) / 4;
    DECLARE @WantsCheckin bit = CASE
        WHEN @RequestedTrueCount > @ExistingTrueCount THEN 1
        WHEN ISNULL(@UserAwardLog, 0) > @ExistingTrueCount THEN 1
        ELSE 0
    END;

    IF @TodayChecked = 0 AND @WantsCheckin = 1
    BEGIN
        SET @Normalized = @Normalized
            + CASE WHEN LEN(@Normalized) = 0 THEN N'' ELSE N',' END
            + N'True';
        SET @ExistingTrueCount = @ExistingTrueCount + 1;
        SET @TodayChecked = 1;
        SET @StoredLastDate = @Now;
    END
    ELSE IF @TodayChecked = 1
    BEGIN
        SET @Normalized = @Normalized
            + CASE WHEN LEN(@Normalized) = 0 THEN N'' ELSE N',' END
            + N'True';
    END

    IF @TodayChecked = 1
       AND CONVERT(date, @StoredLastDate) <> CONVERT(date, @Now)
    BEGIN
        SET @StoredLastDate = @Now;
    END

    IF @Exists = 1
    BEGIN
        UPDATE dbo.DailyLogList
        SET UserAwardLog = @ExistingTrueCount,
            DayLog = @Normalized,
            LastDate = @StoredLastDate
        WHERE UserID = @UserID;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.DailyLogList(UserID, UserAwardLog, DayLog, LastDate)
        VALUES(@UserID, @ExistingTrueCount, @Normalized, @StoredLastDate);
    END

    COMMIT TRANSACTION;
END
GO
