USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ComplianceAssessmentSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @AssessmentID BIGINT = NULL,
    @ScopeCriteria NVARCHAR(500) = NULL,
    @Assessor NVARCHAR(250) = NULL,
    @AssessmentDate DATETIME = NULL,
    @ComplianceScore DECIMAL(18,2) = NULL,
    @FindingsCount INT = NULL,
    @Findings NVARCHAR(MAX) = NULL,
    @ActionPlanReference NVARCHAR(500) = NULL,
    @NextAssessmentDue DATETIME = NULL,
    @Status NVARCHAR(50) = NULL,
    @RelatedObligations NVARCHAR(MAX) = NULL,
    @RelatedControls NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveComplianceAssessment'
    BEGIN
        IF EXISTS(SELECT 1 FROM ComplianceAssessment WHERE AssessmentID = @AssessmentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ComplianceAssessment', @AssessmentID,
                    (SELECT * FROM ComplianceAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())

            UPDATE [ComplianceAssessment]
            SET ScopeCriteria = @ScopeCriteria,
                Assessor = @Assessor,
                AssessmentDate = @AssessmentDate,
                ComplianceScore = @ComplianceScore,
                FindingsCount = @FindingsCount,
                Findings = @Findings,
                ActionPlanReference = @ActionPlanReference,
                NextAssessmentDue = @NextAssessmentDue,
                Status = @Status,
                RelatedObligations = @RelatedObligations,
                RelatedControls = @RelatedControls,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE AssessmentID = @AssessmentID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [ComplianceAssessment]
            (
                CompanyID, ScopeCriteria, Assessor, AssessmentDate, ComplianceScore, FindingsCount, Findings, ActionPlanReference, NextAssessmentDue, Status, RelatedObligations, RelatedControls, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @ScopeCriteria, @Assessor, @AssessmentDate, @ComplianceScore, @FindingsCount, @Findings, @ActionPlanReference, @NextAssessmentDue, @Status, @RelatedObligations, @RelatedControls, @UserID, GETUTCDATE(), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ComplianceAssessment', @AssessmentID,
                    (SELECT * FROM ComplianceAssessment WHERE AssessmentID = (SELECT TOP 1 @@IDENTITY FROM ComplianceAssessment) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvComplianceAssessments'
    BEGIN
        SELECT AssessmentID, CompanyID, ScopeCriteria, Assessor, AssessmentDate, ComplianceScore, FindingsCount, Findings, ActionPlanReference, NextAssessmentDue, Status, RelatedObligations, RelatedControls, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceAssessment
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ComplianceAssessment', NULL,
                (SELECT * FROM ComplianceAssessment FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvComplianceAssessment'
    BEGIN
        SELECT AssessmentID, CompanyID, ScopeCriteria, Assessor, AssessmentDate, ComplianceScore, FindingsCount, Findings, ActionPlanReference, NextAssessmentDue, Status, RelatedObligations, RelatedControls, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceAssessment
        WHERE AssessmentID = @AssessmentID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ComplianceAssessment', @AssessmentID,
                (SELECT * FROM ComplianceAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteComplianceAssessment'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ComplianceAssessment', @AssessmentID,
                (SELECT * FROM ComplianceAssessment WHERE AssessmentID = @AssessmentID FOR XML AUTO), GETDATE())

        DELETE FROM ComplianceAssessment WHERE AssessmentID = @AssessmentID AND CompanyID = @CompanyID
    END
END
