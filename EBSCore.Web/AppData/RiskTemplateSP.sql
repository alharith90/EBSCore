USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RiskTemplateSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @TemplateID INT = NULL,
    @TemplateNameEN NVARCHAR(250) = NULL,
    @TemplateNameAR NVARCHAR(250) = NULL,
    @DefaultCategoryID INT = NULL,
    @DefaultImpact NVARCHAR(50) = NULL,
    @DefaultLikelihood NVARCHAR(50) = NULL,
    @DefaultRiskLevel NVARCHAR(50) = NULL,
    @GuidanceEN NVARCHAR(MAX) = NULL,
    @GuidanceAR NVARCHAR(MAX) = NULL,
    @StatusID NVARCHAR(50) = NULL,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveRiskTemplate'
    BEGIN
        IF EXISTS(SELECT 1 FROM RiskTemplate WHERE TemplateID = @TemplateID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'RiskTemplate', @TemplateID,
            (SELECT * FROM RiskTemplate WHERE TemplateID = @TemplateID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[RiskTemplate]
            SET TemplateNameEN = @TemplateNameEN,
                TemplateNameAR = @TemplateNameAR,
                DefaultCategoryID = @DefaultCategoryID,
                DefaultImpact = @DefaultImpact,
                DefaultLikelihood = @DefaultLikelihood,
                DefaultRiskLevel = @DefaultRiskLevel,
                GuidanceEN = @GuidanceEN,
                GuidanceAR = @GuidanceAR,
                StatusID = @StatusID,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE TemplateID = @TemplateID
              AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[RiskTemplate]
            (
                CompanyID,
                TemplateNameEN,
                TemplateNameAR,
                DefaultCategoryID,
                DefaultImpact,
                DefaultLikelihood,
                DefaultRiskLevel,
                GuidanceEN,
                GuidanceAR,
                StatusID,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @TemplateNameEN,
                @TemplateNameAR,
                @DefaultCategoryID,
                @DefaultImpact,
                @DefaultLikelihood,
                @DefaultRiskLevel,
                @GuidanceEN,
                @GuidanceAR,
                @StatusID,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'RiskTemplate', @TemplateID,
            (SELECT * FROM RiskTemplate WHERE TemplateID = (SELECT TOP 1 @@IDENTITY FROM RiskTemplate) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvRiskTemplates'
    BEGIN
        SELECT TemplateID, CompanyID, TemplateNameEN, TemplateNameAR, DefaultCategoryID, DefaultImpact, DefaultLikelihood, DefaultRiskLevel, GuidanceEN, GuidanceAR, StatusID, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [RiskTemplate]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RiskTemplate', NULL,
        (SELECT * FROM RiskTemplate FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvRiskTemplate'
    BEGIN
        SELECT TemplateID, CompanyID, TemplateNameEN, TemplateNameAR, DefaultCategoryID, DefaultImpact, DefaultLikelihood, DefaultRiskLevel, GuidanceEN, GuidanceAR, StatusID, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [RiskTemplate]
        WHERE TemplateID = @TemplateID
          AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'RiskTemplate', @TemplateID,
        (SELECT * FROM RiskTemplate WHERE TemplateID = @TemplateID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteRiskTemplate'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'RiskTemplate', @TemplateID,
        (SELECT * FROM RiskTemplate WHERE TemplateID = @TemplateID FOR XML AUTO), GETDATE())

        DELETE FROM [RiskTemplate]
        WHERE TemplateID = @TemplateID
          AND CompanyID = @CompanyID
    END
END
GO
