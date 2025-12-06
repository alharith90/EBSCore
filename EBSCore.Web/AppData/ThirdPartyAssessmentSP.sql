USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ThirdPartyAssessmentSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @AssessmentID INT = NULL,
    @ThirdPartyID INT = NULL,
    @AssessmentType NVARCHAR(250) = NULL,
    @DateOfAssessment DATETIME = NULL,
    @AreasAssessed NVARCHAR(MAX) = NULL,
    @QuestionnaireScore NVARCHAR(250) = NULL,
    @OverallRiskRating NVARCHAR(250) = NULL,
    @FindingsIssues NVARCHAR(MAX) = NULL,
    @RequiredMitigations NVARCHAR(MAX) = NULL,
    @ResidualRiskRating NVARCHAR(250) = NULL,
    @ApprovedForOnboarding BIT = NULL,
    @NextAssessmentDue DATETIME = NULL,
    @AssessedBy NVARCHAR(250) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveThirdPartyAssessment'
    BEGIN
        IF EXISTS(SELECT 1 FROM ThirdPartyAssessment WHERE AssessmentID = @AssessmentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ThirdPartyAssessment', @AssessmentID,
                    (SELECT * FROM ThirdPartyAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[ThirdPartyAssessment]
            SET ThirdPartyID = @ThirdPartyID,
                AssessmentType = @AssessmentType,
                DateOfAssessment = @DateOfAssessment,
                AreasAssessed = @AreasAssessed,
                QuestionnaireScore = @QuestionnaireScore,
                OverallRiskRating = @OverallRiskRating,
                FindingsIssues = @FindingsIssues,
                RequiredMitigations = @RequiredMitigations,
                ResidualRiskRating = @ResidualRiskRating,
                ApprovedForOnboarding = @ApprovedForOnboarding,
                NextAssessmentDue = @NextAssessmentDue,
                AssessedBy = @AssessedBy,
                Notes = @Notes,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE AssessmentID = @AssessmentID
              AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[ThirdPartyAssessment]
            (
                CompanyID,
                ThirdPartyID,
                AssessmentType,
                DateOfAssessment,
                AreasAssessed,
                QuestionnaireScore,
                OverallRiskRating,
                FindingsIssues,
                RequiredMitigations,
                ResidualRiskRating,
                ApprovedForOnboarding,
                NextAssessmentDue,
                AssessedBy,
                Notes,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @ThirdPartyID,
                @AssessmentType,
                @DateOfAssessment,
                @AreasAssessed,
                @QuestionnaireScore,
                @OverallRiskRating,
                @FindingsIssues,
                @RequiredMitigations,
                @ResidualRiskRating,
                @ApprovedForOnboarding,
                @NextAssessmentDue,
                @AssessedBy,
                @Notes,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'ThirdPartyAssessment', @AssessmentID,
                    (SELECT * FROM ThirdPartyAssessment WHERE AssessmentID = (SELECT TOP 1 @@IDENTITY FROM ThirdPartyAssessment) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvThirdPartyAssessments'
    BEGIN
        SELECT AssessmentID, CompanyID, ThirdPartyID, AssessmentType, DateOfAssessment, AreasAssessed, QuestionnaireScore, OverallRiskRating,
               FindingsIssues, RequiredMitigations, ResidualRiskRating, ApprovedForOnboarding, NextAssessmentDue, AssessedBy, Notes, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyAssessment]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ThirdPartyAssessment', NULL,
                (SELECT * FROM ThirdPartyAssessment FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvThirdPartyAssessment'
    BEGIN
        SELECT AssessmentID, CompanyID, ThirdPartyID, AssessmentType, DateOfAssessment, AreasAssessed, QuestionnaireScore, OverallRiskRating,
               FindingsIssues, RequiredMitigations, ResidualRiskRating, ApprovedForOnboarding, NextAssessmentDue, AssessedBy, Notes, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyAssessment]
        WHERE AssessmentID = @AssessmentID
          AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ThirdPartyAssessment', @AssessmentID,
                (SELECT * FROM ThirdPartyAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteThirdPartyAssessment'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ThirdPartyAssessment', @AssessmentID,
                (SELECT * FROM ThirdPartyAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())

        DELETE FROM [ThirdPartyAssessment]
        WHERE AssessmentID = @AssessmentID
          AND CompanyID = @CompanyID
    END
END
GO
