SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Risk management schema extensions
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SRiskCategory')
BEGIN
    CREATE TABLE [dbo].[S7SRiskCategory](
        [RiskCategoryID] INT IDENTITY(1,1) NOT NULL,
        [CategoryName] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL DEFAULT(1),
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
        [UpdatedBy] INT NULL,
        [UpdatedAt] DATETIME NULL,
        CONSTRAINT [PK_S7SRiskCategory] PRIMARY KEY CLUSTERED ([RiskCategoryID] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SRiskLikelihood')
BEGIN
    CREATE TABLE [dbo].[S7SRiskLikelihood](
        [LikelihoodID] INT IDENTITY(1,1) NOT NULL,
        [LikelihoodName] NVARCHAR(200) NOT NULL,
        [LikelihoodValue] INT NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL DEFAULT(1),
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
        [UpdatedBy] INT NULL,
        [UpdatedAt] DATETIME NULL,
        CONSTRAINT [PK_S7SRiskLikelihood] PRIMARY KEY CLUSTERED ([LikelihoodID] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SRiskMatrixConfig')
BEGIN
    CREATE TABLE [dbo].[S7SRiskMatrixConfig](
        [RiskMatrixConfigID] INT IDENTITY(1,1) NOT NULL,
        [MatrixName] NVARCHAR(200) NOT NULL,
        [MatrixSize] INT NOT NULL DEFAULT(5),
        [IsDynamic] BIT NOT NULL DEFAULT(0),
        [ConfigJson] NVARCHAR(MAX) NULL,
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
        [UpdatedBy] INT NULL,
        [UpdatedAt] DATETIME NULL,
        CONSTRAINT [PK_S7SRiskMatrixConfig] PRIMARY KEY CLUSTERED ([RiskMatrixConfigID] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SRiskTolerance')
BEGIN
    CREATE TABLE [dbo].[S7SRiskTolerance](
        [RiskToleranceID] INT IDENTITY(1,1) NOT NULL,
        [RiskMatrixConfigID] INT NULL,
        [HighThreshold] INT NOT NULL DEFAULT(15),
        [MediumThreshold] INT NOT NULL DEFAULT(8),
        [LowLabel] NVARCHAR(50) NOT NULL DEFAULT('Low'),
        [MediumLabel] NVARCHAR(50) NOT NULL DEFAULT('Medium'),
        [HighLabel] NVARCHAR(50) NOT NULL DEFAULT('High'),
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
        [UpdatedBy] INT NULL,
        [UpdatedAt] DATETIME NULL,
        CONSTRAINT [PK_S7SRiskTolerance] PRIMARY KEY CLUSTERED ([RiskToleranceID] ASC)
    );
    ALTER TABLE [dbo].[S7SRiskTolerance] WITH CHECK ADD CONSTRAINT [FK_S7SRiskTolerance_Matrix] FOREIGN KEY([RiskMatrixConfigID])
    REFERENCES [dbo].[S7SRiskMatrixConfig]([RiskMatrixConfigID]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SRisk')
BEGIN
    CREATE TABLE [dbo].[S7SRisk](
        [RiskID] INT IDENTITY(1,1) NOT NULL,
        [BCMRiskAssessmentId] INT NULL,
        [ImpactAspectID] INT NULL,
        [ImpactTimeFrameID] INT NULL,
        [ImpactID] INT NOT NULL,
        [LikelihoodID] INT NOT NULL,
        [RiskCategoryID] INT NULL,
        [RiskTitle] NVARCHAR(200) NOT NULL,
        [RiskDescription] NVARCHAR(MAX) NULL,
        [MitigationPlan] NVARCHAR(MAX) NULL,
        [RiskScore] INT NOT NULL,
        [RiskLevel] NVARCHAR(50) NOT NULL,
        [NCEMAPillar] NVARCHAR(200) NOT NULL DEFAULT('Risk Assessment'),
        [IsDeleted] BIT NOT NULL DEFAULT(0),
        [CreatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
        [UpdatedBy] INT NULL,
        [UpdatedAt] DATETIME NULL,
        CONSTRAINT [PK_S7SRisk] PRIMARY KEY CLUSTERED ([RiskID] ASC)
    );
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_RiskAssessment] FOREIGN KEY([BCMRiskAssessmentId])
    REFERENCES [dbo].[BCMRiskAssessment]([BCMRiskAssessmentId]);
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_ImpactAspect] FOREIGN KEY([ImpactAspectID])
    REFERENCES [dbo].[BCMImpactAspects]([ImpactAspectID]);
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_ImpactTimeFrame] FOREIGN KEY([ImpactTimeFrameID])
    REFERENCES [dbo].[BCMImpactTimeFrames]([ImpactTimeFrameID]);
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_Impact] FOREIGN KEY([ImpactID])
    REFERENCES [dbo].[Impact]([ImpactID]);
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_Likelihood] FOREIGN KEY([LikelihoodID])
    REFERENCES [dbo].[S7SRiskLikelihood]([LikelihoodID]);
    ALTER TABLE [dbo].[S7SRisk] WITH CHECK ADD CONSTRAINT [FK_S7SRisk_Category] FOREIGN KEY([RiskCategoryID])
    REFERENCES [dbo].[S7SRiskCategory]([RiskCategoryID]);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[S7SRisk_SP]
    @Operation NVARCHAR(50),
    @CurrentUserID INT = NULL,
    @RiskID INT = NULL,
    @BCMRiskAssessmentId INT = NULL,
    @ImpactAspectID INT = NULL,
    @ImpactTimeFrameID INT = NULL,
    @ImpactID INT = NULL,
    @LikelihoodID INT = NULL,
    @RiskCategoryID INT = NULL,
    @RiskTitle NVARCHAR(200) = NULL,
    @RiskDescription NVARCHAR(MAX) = NULL,
    @MitigationPlan NVARCHAR(MAX) = NULL,
    @MatrixName NVARCHAR(200) = NULL,
    @MatrixSize INT = NULL,
    @IsDynamic BIT = NULL,
    @ConfigJson NVARCHAR(MAX) = NULL,
    @RiskMatrixConfigID INT = NULL,
    @RiskToleranceID INT = NULL,
    @HighThreshold INT = NULL,
    @MediumThreshold INT = NULL,
    @LowLabel NVARCHAR(50) = NULL,
    @MediumLabel NVARCHAR(50) = NULL,
    @HighLabel NVARCHAR(50) = NULL,
    @LikelihoodName NVARCHAR(200) = NULL,
    @LikelihoodValue INT = NULL,
    @LikelihoodDescription NVARCHAR(MAX) = NULL,
    @CategoryName NVARCHAR(200) = NULL,
    @CategoryDescription NVARCHAR(MAX) = NULL,
    @PageSize INT = 10,
    @PageNumber INT = 1,
    @SearchQuery NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ResolvedHigh INT = COALESCE(@HighThreshold, 15);
    DECLARE @ResolvedMedium INT = COALESCE(@MediumThreshold, 8);
    DECLARE @ToleranceLowLabel NVARCHAR(50) = COALESCE(@LowLabel, 'Low');
    DECLARE @ToleranceMediumLabel NVARCHAR(50) = COALESCE(@MediumLabel, 'Medium');
    DECLARE @ToleranceHighLabel NVARCHAR(50) = COALESCE(@HighLabel, 'High');

    IF @Operation = 'SaveRisk'
    BEGIN
        DECLARE @ImpactVal INT = (SELECT ImpactValue FROM dbo.Impact WHERE ImpactID = @ImpactID);
        DECLARE @LikelihoodVal INT = (SELECT LikelihoodValue FROM dbo.S7SRiskLikelihood WHERE LikelihoodID = @LikelihoodID);
        DECLARE @ComputedScore INT = ISNULL(@ImpactVal,0) * ISNULL(@LikelihoodVal,0);

        DECLARE @ToleranceHigh INT = (SELECT TOP 1 HighThreshold FROM dbo.S7SRiskTolerance WHERE (@RiskMatrixConfigID IS NULL OR RiskMatrixConfigID = @RiskMatrixConfigID) ORDER BY CreatedAt DESC);
        DECLARE @ToleranceMedium INT = (SELECT TOP 1 MediumThreshold FROM dbo.S7SRiskTolerance WHERE (@RiskMatrixConfigID IS NULL OR RiskMatrixConfigID = @RiskMatrixConfigID) ORDER BY CreatedAt DESC);
        DECLARE @RiskLevel NVARCHAR(50) = CASE
            WHEN @ComputedScore >= ISNULL(@ToleranceHigh, @ResolvedHigh) THEN COALESCE((SELECT TOP 1 HighLabel FROM dbo.S7SRiskTolerance ORDER BY CreatedAt DESC), @ToleranceHighLabel)
            WHEN @ComputedScore >= ISNULL(@ToleranceMedium, @ResolvedMedium) THEN COALESCE((SELECT TOP 1 MediumLabel FROM dbo.S7SRiskTolerance ORDER BY CreatedAt DESC), @ToleranceMediumLabel)
            ELSE COALESCE((SELECT TOP 1 LowLabel FROM dbo.S7SRiskTolerance ORDER BY CreatedAt DESC), @ToleranceLowLabel)
        END;

        IF NOT EXISTS(SELECT 1 FROM dbo.S7SRisk WHERE RiskID = @RiskID)
        BEGIN
            INSERT INTO dbo.S7SRisk(BCMRiskAssessmentId, ImpactAspectID, ImpactTimeFrameID, ImpactID, LikelihoodID, RiskCategoryID, RiskTitle, RiskDescription, MitigationPlan, RiskScore, RiskLevel, NCEMAPillar, CreatedBy)
            VALUES(@BCMRiskAssessmentId, @ImpactAspectID, @ImpactTimeFrameID, @ImpactID, @LikelihoodID, @RiskCategoryID, @RiskTitle, @RiskDescription, @MitigationPlan, @ComputedScore, @RiskLevel, 'Risk Assessment', ISNULL(@CurrentUserID,0));
            SELECT SCOPE_IDENTITY() AS NewRiskID, @ComputedScore AS RiskScore, @RiskLevel AS RiskLevel; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.S7SRisk
               SET BCMRiskAssessmentId = @BCMRiskAssessmentId,
                   ImpactAspectID = @ImpactAspectID,
                   ImpactTimeFrameID = @ImpactTimeFrameID,
                   ImpactID = @ImpactID,
                   LikelihoodID = @LikelihoodID,
                   RiskCategoryID = @RiskCategoryID,
                   RiskTitle = @RiskTitle,
                   RiskDescription = @RiskDescription,
                   MitigationPlan = @MitigationPlan,
                   RiskScore = @ComputedScore,
                   RiskLevel = @RiskLevel,
                   UpdatedBy = @CurrentUserID,
                   UpdatedAt = GETUTCDATE()
             WHERE RiskID = @RiskID; 
            SELECT @RiskID AS UpdatedRiskID, @ComputedScore AS RiskScore, @RiskLevel AS RiskLevel; RETURN;
        END
    END
    ELSE IF @Operation = 'rtvRisk'
    BEGIN
        SELECT r.RiskID, r.BCMRiskAssessmentId, r.ImpactAspectID, r.ImpactTimeFrameID, r.ImpactID, r.LikelihoodID, r.RiskCategoryID,
               r.RiskTitle, r.RiskDescription, r.MitigationPlan, r.RiskScore, r.RiskLevel, r.NCEMAPillar, r.CreatedAt, r.UpdatedAt,
               ia.AspectName, itf.TimeLabel, i.ImpactValue, i.ImpactText, l.LikelihoodName, l.LikelihoodValue, c.CategoryName
          FROM dbo.S7SRisk r
          LEFT JOIN dbo.BCMImpactAspects ia ON r.ImpactAspectID = ia.ImpactAspectID
          LEFT JOIN dbo.BCMImpactTimeFrames itf ON r.ImpactTimeFrameID = itf.ImpactTimeFrameID
          INNER JOIN dbo.Impact i ON r.ImpactID = i.ImpactID
          INNER JOIN dbo.S7SRiskLikelihood l ON r.LikelihoodID = l.LikelihoodID
          LEFT JOIN dbo.S7SRiskCategory c ON r.RiskCategoryID = c.RiskCategoryID
         WHERE r.RiskID = @RiskID AND r.IsDeleted = 0;
        RETURN;
    END
    ELSE IF @Operation = 'rtvRiskList'
    BEGIN
        SELECT ROW_NUMBER() OVER(ORDER BY r.CreatedAt DESC) AS SerialNo, r.RiskID, r.RiskTitle, r.RiskDescription, r.MitigationPlan,
               r.RiskScore, r.RiskLevel, r.NCEMAPillar, ia.AspectName, itf.TimeLabel, i.ImpactValue, l.LikelihoodValue, c.CategoryName
          FROM dbo.S7SRisk r
          LEFT JOIN dbo.BCMImpactAspects ia ON r.ImpactAspectID = ia.ImpactAspectID
          LEFT JOIN dbo.BCMImpactTimeFrames itf ON r.ImpactTimeFrameID = itf.ImpactTimeFrameID
          INNER JOIN dbo.Impact i ON r.ImpactID = i.ImpactID
          INNER JOIN dbo.S7SRiskLikelihood l ON r.LikelihoodID = l.LikelihoodID
          LEFT JOIN dbo.S7SRiskCategory c ON r.RiskCategoryID = c.RiskCategoryID
         WHERE r.IsDeleted = 0
           AND (@SearchQuery IS NULL OR @SearchQuery = '' OR r.RiskTitle LIKE '%' + @SearchQuery + '%' OR r.RiskDescription LIKE '%' + @SearchQuery + '%');
        SELECT CEILING(COUNT(1) * 1.0 / @PageSize) AS PageCount, @PageNumber AS CurrentPage
          FROM dbo.S7SRisk r
         WHERE r.IsDeleted = 0
           AND (@SearchQuery IS NULL OR @SearchQuery = '' OR r.RiskTitle LIKE '%' + @SearchQuery + '%' OR r.RiskDescription LIKE '%' + @SearchQuery + '%');
        RETURN;
    END
    ELSE IF @Operation = 'DeleteRisk'
    BEGIN
        UPDATE dbo.S7SRisk SET IsDeleted = 1, UpdatedBy = @CurrentUserID, UpdatedAt = GETUTCDATE() WHERE RiskID = @RiskID; RETURN;
    END
    ELSE IF @Operation = 'SaveCategory'
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM dbo.S7SRiskCategory WHERE RiskCategoryID = @RiskCategoryID)
        BEGIN
            INSERT INTO dbo.S7SRiskCategory(CategoryName, Description, CreatedBy)
            VALUES(@CategoryName, @CategoryDescription, ISNULL(@CurrentUserID,0));
            SELECT SCOPE_IDENTITY() AS NewRiskCategoryID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.S7SRiskCategory
               SET CategoryName = @CategoryName,
                   Description = @CategoryDescription,
                   UpdatedBy = @CurrentUserID,
                   UpdatedAt = GETUTCDATE()
             WHERE RiskCategoryID = @RiskCategoryID; RETURN;
        END
    END
    ELSE IF @Operation = 'rtvCategories'
    BEGIN
        SELECT RiskCategoryID, CategoryName, Description, IsActive, CreatedAt, UpdatedAt FROM dbo.S7SRiskCategory WHERE IsActive = 1;
        RETURN;
    END
    ELSE IF @Operation = 'SaveLikelihood'
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM dbo.S7SRiskLikelihood WHERE LikelihoodID = @LikelihoodID)
        BEGIN
            INSERT INTO dbo.S7SRiskLikelihood(LikelihoodName, LikelihoodValue, Description, CreatedBy)
            VALUES(@LikelihoodName, @LikelihoodValue, @LikelihoodDescription, ISNULL(@CurrentUserID,0));
            SELECT SCOPE_IDENTITY() AS NewLikelihoodID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.S7SRiskLikelihood
               SET LikelihoodName = @LikelihoodName,
                   LikelihoodValue = @LikelihoodValue,
                   Description = @LikelihoodDescription,
                   UpdatedBy = @CurrentUserID,
                   UpdatedAt = GETUTCDATE()
             WHERE LikelihoodID = @LikelihoodID; RETURN;
        END
    END
    ELSE IF @Operation = 'rtvLikelihoods'
    BEGIN
        SELECT LikelihoodID, LikelihoodName, LikelihoodValue, Description, IsActive, CreatedAt, UpdatedAt FROM dbo.S7SRiskLikelihood WHERE IsActive = 1;
        RETURN;
    END
    ELSE IF @Operation = 'SaveMatrixConfig'
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM dbo.S7SRiskMatrixConfig WHERE RiskMatrixConfigID = @RiskMatrixConfigID)
        BEGIN
            INSERT INTO dbo.S7SRiskMatrixConfig(MatrixName, MatrixSize, IsDynamic, ConfigJson, CreatedBy)
            VALUES(COALESCE(@MatrixName, 'Default Matrix'), COALESCE(@MatrixSize,5), ISNULL(@IsDynamic,0), @ConfigJson, ISNULL(@CurrentUserID,0));
            SELECT SCOPE_IDENTITY() AS NewRiskMatrixConfigID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.S7SRiskMatrixConfig
               SET MatrixName = COALESCE(@MatrixName, MatrixName),
                   MatrixSize = COALESCE(@MatrixSize, MatrixSize),
                   IsDynamic = ISNULL(@IsDynamic, IsDynamic),
                   ConfigJson = COALESCE(@ConfigJson, ConfigJson),
                   UpdatedBy = @CurrentUserID,
                   UpdatedAt = GETUTCDATE()
             WHERE RiskMatrixConfigID = @RiskMatrixConfigID; RETURN;
        END
    END
    ELSE IF @Operation = 'rtvMatrixConfig'
    BEGIN
        SELECT RiskMatrixConfigID, MatrixName, MatrixSize, IsDynamic, ConfigJson, CreatedAt, UpdatedAt FROM dbo.S7SRiskMatrixConfig;
        RETURN;
    END
    ELSE IF @Operation = 'SaveTolerance'
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM dbo.S7SRiskTolerance WHERE RiskToleranceID = @RiskToleranceID)
        BEGIN
            INSERT INTO dbo.S7SRiskTolerance(RiskMatrixConfigID, HighThreshold, MediumThreshold, LowLabel, MediumLabel, HighLabel, CreatedBy)
            VALUES(@RiskMatrixConfigID, @ResolvedHigh, @ResolvedMedium, @ToleranceLowLabel, @ToleranceMediumLabel, @ToleranceHighLabel, ISNULL(@CurrentUserID,0));
            SELECT SCOPE_IDENTITY() AS NewRiskToleranceID; RETURN;
        END
        ELSE
        BEGIN
            UPDATE dbo.S7SRiskTolerance
               SET RiskMatrixConfigID = @RiskMatrixConfigID,
                   HighThreshold = @ResolvedHigh,
                   MediumThreshold = @ResolvedMedium,
                   LowLabel = @ToleranceLowLabel,
                   MediumLabel = @ToleranceMediumLabel,
                   HighLabel = @ToleranceHighLabel,
                   UpdatedBy = @CurrentUserID,
                   UpdatedAt = GETUTCDATE()
             WHERE RiskToleranceID = @RiskToleranceID; RETURN;
        END
    END
    ELSE IF @Operation = 'rtvTolerance'
    BEGIN
        SELECT TOP 1 RiskToleranceID, RiskMatrixConfigID, HighThreshold, MediumThreshold, LowLabel, MediumLabel, HighLabel, CreatedAt, UpdatedAt
          FROM dbo.S7SRiskTolerance ORDER BY CreatedAt DESC;
        RETURN;
    END
    ELSE IF @Operation = 'rtvHeatmap'
    BEGIN
        SELECT i.ImpactValue AS Impact, l.LikelihoodValue AS Likelihood, COUNT(1) AS RiskCount
          FROM dbo.S7SRisk r
          INNER JOIN dbo.Impact i ON r.ImpactID = i.ImpactID
          INNER JOIN dbo.S7SRiskLikelihood l ON r.LikelihoodID = l.LikelihoodID
         WHERE r.IsDeleted = 0
         GROUP BY i.ImpactValue, l.LikelihoodValue;

        SELECT TOP 1 MatrixSize, IsDynamic FROM dbo.S7SRiskMatrixConfig ORDER BY CreatedAt DESC;
        SELECT TOP 1 HighThreshold, MediumThreshold FROM dbo.S7SRiskTolerance ORDER BY CreatedAt DESC;
        RETURN;
    END
END
GO
