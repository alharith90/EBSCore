/****** Template reference: mirrors ProcessSP permission/history flow ******/
IF OBJECT_ID('dbo.HSRiskAssessment', 'U') IS NOT NULL
    DROP TABLE dbo.HSRiskAssessment;
GO

CREATE TABLE [dbo].[HSRiskAssessment]
(
    [HazardID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [HazardDescription] NVARCHAR(MAX) NOT NULL,
    [LocationArea] NVARCHAR(250) NULL,
    [RelatedActivity] NVARCHAR(250) NULL,
    [PotentialImpact] NVARCHAR(MAX) NULL,
    [Likelihood] NVARCHAR(100) NULL,
    [Severity] NVARCHAR(100) NULL,
    [RiskLevel] NVARCHAR(100) NULL,
    [ExistingControls] NVARCHAR(MAX) NULL,
    [AdditionalControlsNeeded] NVARCHAR(MAX) NULL,
    [RiskOwner] NVARCHAR(200) NULL,
    [NextReviewDate] DATETIME NULL,
    [Status] NVARCHAR(100) NULL,
    [CreatedBy] BIGINT NULL,
    [ModifiedBy] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_HSRiskAssessment_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_HSRiskAssessment_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[HSRiskAssessmentSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @HazardID INT = NULL,
    @HazardDescription NVARCHAR(MAX) = NULL,
    @LocationArea NVARCHAR(250) = NULL,
    @RelatedActivity NVARCHAR(250) = NULL,
    @PotentialImpact NVARCHAR(MAX) = NULL,
    @Likelihood NVARCHAR(100) = NULL,
    @Severity NVARCHAR(100) = NULL,
    @RiskLevel NVARCHAR(100) = NULL,
    @ExistingControls NVARCHAR(MAX) = NULL,
    @AdditionalControlsNeeded NVARCHAR(MAX) = NULL,
    @RiskOwner NVARCHAR(200) = NULL,
    @NextReviewDate DATETIME = NULL,
    @Status NVARCHAR(100) = NULL,
    @CreatedBy BIGINT = NULL,
    @ModifiedBy BIGINT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveHSRisk'
    BEGIN
        IF EXISTS(SELECT 1 FROM HSRiskAssessment WHERE HazardID = @HazardID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'HSRiskAssessment', @HazardID,
                    (SELECT * FROM HSRiskAssessment WHERE HazardID = @HazardID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[HSRiskAssessment]
            SET [HazardDescription] = @HazardDescription,
                [LocationArea] = @LocationArea,
                [RelatedActivity] = @RelatedActivity,
                [PotentialImpact] = @PotentialImpact,
                [Likelihood] = @Likelihood,
                [Severity] = @Severity,
                [RiskLevel] = @RiskLevel,
                [ExistingControls] = @ExistingControls,
                [AdditionalControlsNeeded] = @AdditionalControlsNeeded,
                [RiskOwner] = @RiskOwner,
                [NextReviewDate] = @NextReviewDate,
                [Status] = @Status,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE HazardID = @HazardID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[HSRiskAssessment]
                ([CompanyID], [HazardDescription], [LocationArea], [RelatedActivity], [PotentialImpact], [Likelihood], [Severity], [RiskLevel], [ExistingControls], [AdditionalControlsNeeded], [RiskOwner], [NextReviewDate], [Status], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @HazardDescription, @LocationArea, @RelatedActivity, @PotentialImpact, @Likelihood, @Severity, @RiskLevel, @ExistingControls, @AdditionalControlsNeeded, @RiskOwner, @NextReviewDate, @Status, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewHazardID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'HSRiskAssessment', @NewHazardID,
                    (SELECT * FROM HSRiskAssessment WHERE HazardID = @NewHazardID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvHSRisks'
    BEGIN
        SELECT
            HazardID,
            CompanyID,
            HazardDescription,
            LocationArea,
            RelatedActivity,
            PotentialImpact,
            Likelihood,
            Severity,
            RiskLevel,
            ExistingControls,
            AdditionalControlsNeeded,
            RiskOwner,
            NextReviewDate,
            Status,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [HSRiskAssessment]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'HSRiskAssessment', NULL,
                (SELECT * FROM HSRiskAssessment WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvHSRisk'
    BEGIN
        SELECT
            HazardID,
            CompanyID,
            HazardDescription,
            LocationArea,
            RelatedActivity,
            PotentialImpact,
            Likelihood,
            Severity,
            RiskLevel,
            ExistingControls,
            AdditionalControlsNeeded,
            RiskOwner,
            NextReviewDate,
            Status,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [HSRiskAssessment]
        WHERE HazardID = @HazardID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'HSRiskAssessment', @HazardID,
                (SELECT * FROM HSRiskAssessment WHERE HazardID = @HazardID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteHSRisk'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'HSRiskAssessment', @HazardID,
                (SELECT * FROM HSRiskAssessment WHERE HazardID = @HazardID FOR XML AUTO), GETDATE());

        DELETE FROM [HSRiskAssessment]
        WHERE HazardID = @HazardID
          AND CompanyID = @CompanyID;
    END
END
GO
