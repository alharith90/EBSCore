/****** Object:  StoredProcedure [dbo].[BCMImpactAspectSP] ******/
IF OBJECT_ID(N'[dbo].[BCMImpactAspectSP]', N'P') IS NOT NULL DROP PROCEDURE [dbo].[BCMImpactAspectSP];
GO
CREATE PROCEDURE [dbo].[BCMImpactAspectSP]
    @Operation NVARCHAR(50),
    @CurrentUserID INT = NULL,
    @ImpactAspectID INT = NULL,
    @AspectName NVARCHAR(100) = NULL,
    @Description NVARCHAR(MAX) = NULL,
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
        IF NOT EXISTS(SELECT 1 FROM dbo.BCMImpactAspects WHERE ImpactAspectID=@ImpactAspectID)
        BEGIN
            INSERT INTO dbo.BCMImpactAspects(AspectName,Description) VALUES(@AspectName,@Description);
            SELECT SCOPE_IDENTITY() AS NewImpactAspectID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.BCMImpactAspects SET AspectName=@AspectName, Description=@Description WHERE ImpactAspectID=@ImpactAspectID; RETURN;
        END
    END
    ELSE IF @Operation='rtvImpactAspect'
    BEGIN SELECT ImpactAspectID,AspectName,Description FROM dbo.BCMImpactAspects WHERE ImpactAspectID=@ImpactAspectID; RETURN; END
    ELSE IF @Operation='rtvImpactAspects'
    BEGIN
        SELECT ROW_NUMBER() OVER(ORDER BY ImpactAspectID) AS SerialNo, ImpactAspectID,AspectName,Description
          FROM dbo.BCMImpactAspects
         WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR AspectName LIKE '%' + @SearchQuery + '%');
        SELECT CEILING(COUNT(1) * 1.0 / @PageSize) AS PageCount, @PageNumber AS CurrentPage FROM dbo.BCMImpactAspects WHERE (@SearchQuery IS NULL OR @SearchQuery='' OR AspectName LIKE '%' + @SearchQuery + '%');
        RETURN;
    END
    ELSE IF @Operation='Delete' BEGIN DELETE FROM dbo.BCMImpactAspects WHERE ImpactAspectID=@ImpactAspectID; RETURN; END
END
GO
