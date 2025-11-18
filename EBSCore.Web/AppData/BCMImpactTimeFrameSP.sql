/****** Object:  StoredProcedure [dbo].[BCMImpactTimeFrameSP] ******/
IF OBJECT_ID(N'[dbo].[BCMImpactTimeFrameSP]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[BCMImpactTimeFrameSP];
GO
CREATE PROCEDURE [dbo].[BCMImpactTimeFrameSP]
    @Operation NVARCHAR(50),
    @CurrentUserID INT = NULL,
    @ImpactTimeFrameID INT = NULL,
    @TimeLabel NVARCHAR(50) = NULL,
    @MinHours INT = NULL,
    @MaxHours INT = NULL,
    @PageSize INT = 10,
    @PageNumber INT = 1,
    @SortColumn NVARCHAR(50) = NULL,
    @SortDirection NVARCHAR(10) = 'ASC',
    @SearchFields NVARCHAR(MAX) = NULL,
    @SearchQuery NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operation='Save'
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM dbo.BCMImpactTimeFrames WHERE ImpactTimeFrameID=@ImpactTimeFrameID)
        BEGIN
            INSERT INTO dbo.BCMImpactTimeFrames(TimeLabel,MinHours,MaxHours) VALUES(@TimeLabel,@MinHours,@MaxHours);
            SELECT SCOPE_IDENTITY() AS NewImpactTimeFrameID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.BCMImpactTimeFrames SET TimeLabel=@TimeLabel, MinHours=@MinHours, MaxHours=@MaxHours WHERE ImpactTimeFrameID=@ImpactTimeFrameID; RETURN;
        END
    END
    ELSE IF @Operation='rtvImpactTimeFrame'
    BEGIN SELECT ImpactTimeFrameID,TimeLabel,MinHours,MaxHours FROM dbo.BCMImpactTimeFrames WHERE ImpactTimeFrameID=@ImpactTimeFrameID; RETURN; END
    ELSE IF @Operation='rtvImpactTimeFrames'
    BEGIN
        SELECT ROW_NUMBER() OVER(ORDER BY ImpactTimeFrameID) AS SerialNo, ImpactTimeFrameID,TimeLabel,MinHours,MaxHours
          FROM dbo.BCMImpactTimeFrames
         WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR TimeLabel LIKE '%' + @SearchQuery + '%');
        SELECT CEILING(COUNT(1) * 1.0 / @PageSize) AS PageCount, @PageNumber AS CurrentPage FROM dbo.BCMImpactTimeFrames WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR TimeLabel LIKE '%' + @SearchQuery + '%');
        RETURN;
    END
    ELSE IF @Operation='Delete' BEGIN DELETE FROM dbo.BCMImpactTimeFrames WHERE ImpactTimeFrameID=@ImpactTimeFrameID; RETURN; END
END
GO
