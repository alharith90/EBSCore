/****** Object:  StoredProcedure [dbo].[BCMAspectTimeImpactLevelSP] ******/
IF OBJECT_ID(N'[dbo].[BCMAspectTimeImpactLevelSP]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[BCMAspectTimeImpactLevelSP];
GO
CREATE PROCEDURE [dbo].[BCMAspectTimeImpactLevelSP]
    @Operation NVARCHAR(50),
    @CurrentUserID INT = NULL,
    @AspectTimeImpactLevelID INT = NULL,
    @ImpactAspectID INT = NULL,
    @ImpactTimeFrameID INT = NULL,
    @LevelID INT = NULL,
    @ImpactLevel NVARCHAR(100) = NULL,
    @Justification NVARCHAR(MAX) = NULL,
    @ImpactColor NVARCHAR(15) = NULL,
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
        IF NOT EXISTS(SELECT 1 FROM dbo.BCMAspectTimeImpactLevels WHERE AspectTimeImpactLevelID=@AspectTimeImpactLevelID)
        BEGIN
            INSERT INTO dbo.BCMAspectTimeImpactLevels(ImpactAspectID,ImpactTimeFrameID,LevelID,ImpactLevel,Justification,ImpactColor,CreatedBy,CreatedAt)
            VALUES(@ImpactAspectID,@ImpactTimeFrameID,@LevelID,@ImpactLevel,@Justification,@ImpactColor,ISNULL(@CurrentUserID,0),GETUTCDATE());
            SELECT SCOPE_IDENTITY() AS NewAspectTimeImpactLevelID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.BCMAspectTimeImpactLevels
               SET ImpactAspectID=@ImpactAspectID,
                   ImpactTimeFrameID=@ImpactTimeFrameID,
                   LevelID=@LevelID,
                   ImpactLevel=@ImpactLevel,
                   Justification=@Justification,
                   ImpactColor=@ImpactColor,
                   UpdateBy=@CurrentUserID,
                   UpdatedAt=GETUTCDATE()
             WHERE AspectTimeImpactLevelID=@AspectTimeImpactLevelID; RETURN;
        END
    END
    ELSE IF @Operation='rtvAspectTimeImpactLevel'
    BEGIN SELECT AspectTimeImpactLevelID,ImpactAspectID,ImpactTimeFrameID,LevelID,ImpactLevel,Justification,ImpactColor,CreatedBy,UpdateBy,CreatedAt,UpdatedAt FROM dbo.BCMAspectTimeImpactLevels WHERE AspectTimeImpactLevelID=@AspectTimeImpactLevelID; RETURN; END
    ELSE IF @Operation='rtvAspectTimeImpactLevels'
    BEGIN
        SELECT ROW_NUMBER() OVER(ORDER BY AspectTimeImpactLevelID) AS SerialNo, AspectTimeImpactLevelID,ImpactAspectID,ImpactTimeFrameID,LevelID,ImpactLevel,Justification,ImpactColor,CreatedBy,UpdateBy,CreatedAt,UpdatedAt
          FROM dbo.BCMAspectTimeImpactLevels
         WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR ImpactLevel LIKE '%' + @SearchQuery + '%');
        SELECT CEILING(COUNT(1) * 1.0 / @PageSize) AS PageCount, @PageNumber AS CurrentPage FROM dbo.BCMAspectTimeImpactLevels WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR ImpactLevel LIKE '%' + @SearchQuery + '%');
        RETURN;
    END
    ELSE IF @Operation='Delete' BEGIN DELETE FROM dbo.BCMAspectTimeImpactLevels WHERE AspectTimeImpactLevelID=@AspectTimeImpactLevelID; RETURN; END
END
GO
