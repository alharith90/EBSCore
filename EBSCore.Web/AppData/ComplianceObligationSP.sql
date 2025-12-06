USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ComplianceObligationSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ObligationID BIGINT = NULL,
    @ObligationTitle NVARCHAR(250) = NULL,
    @ObligationDescription NVARCHAR(MAX) = NULL,
    @Source NVARCHAR(250) = NULL,
    @ClauseReference NVARCHAR(250) = NULL,
    @ResponsibleDepartment NVARCHAR(250) = NULL,
    @ObligationOwner NVARCHAR(250) = NULL,
    @ComplianceStatus NVARCHAR(50) = NULL,
    @LastAssessmentDate DATETIME = NULL,
    @NextReviewDate DATETIME = NULL,
    @RelatedControls NVARCHAR(MAX) = NULL,
    @RelatedRisks NVARCHAR(MAX) = NULL,
    @RelatedPolicy NVARCHAR(MAX) = NULL,
    @EvidenceOfCompliance NVARCHAR(MAX) = NULL,
    @Comments NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveObligation'
    BEGIN
        IF EXISTS(SELECT 1 FROM ComplianceObligation WHERE ObligationID = @ObligationID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ComplianceObligation', @ObligationID,
                    (SELECT * FROM ComplianceObligation WHERE ObligationID = @ObligationID FOR XML AUTO), GETDATE())

            UPDATE [ComplianceObligation]
            SET ObligationTitle = @ObligationTitle,
                ObligationDescription = @ObligationDescription,
                Source = @Source,
                ClauseReference = @ClauseReference,
                ResponsibleDepartment = @ResponsibleDepartment,
                ObligationOwner = @ObligationOwner,
                ComplianceStatus = @ComplianceStatus,
                LastAssessmentDate = @LastAssessmentDate,
                NextReviewDate = @NextReviewDate,
                RelatedControls = @RelatedControls,
                RelatedRisks = @RelatedRisks,
                RelatedPolicy = @RelatedPolicy,
                EvidenceOfCompliance = @EvidenceOfCompliance,
                Comments = @Comments,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE ObligationID = @ObligationID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [ComplianceObligation]
            (
                CompanyID, ObligationTitle, ObligationDescription, Source, ClauseReference, ResponsibleDepartment, ObligationOwner, ComplianceStatus, LastAssessmentDate, NextReviewDate, RelatedControls, RelatedRisks, RelatedPolicy, EvidenceOfCompliance, Comments, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @ObligationTitle, @ObligationDescription, @Source, @ClauseReference, @ResponsibleDepartment, @ObligationOwner, @ComplianceStatus, @LastAssessmentDate, @NextReviewDate, @RelatedControls, @RelatedRisks, @RelatedPolicy, @EvidenceOfCompliance, @Comments, @UserID, GETUTCDATE(), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ComplianceObligation', @ObligationID,
                    (SELECT * FROM ComplianceObligation WHERE ObligationID = (SELECT TOP 1 @@IDENTITY FROM ComplianceObligation) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvObligations'
    BEGIN
        SELECT ObligationID, CompanyID, ObligationTitle, ObligationDescription, Source, ClauseReference, ResponsibleDepartment, ObligationOwner, ComplianceStatus, LastAssessmentDate, NextReviewDate, RelatedControls, RelatedRisks, RelatedPolicy, EvidenceOfCompliance, Comments, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceObligation
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ComplianceObligation', NULL,
                (SELECT * FROM ComplianceObligation FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvObligation'
    BEGIN
        SELECT ObligationID, CompanyID, ObligationTitle, ObligationDescription, Source, ClauseReference, ResponsibleDepartment, ObligationOwner, ComplianceStatus, LastAssessmentDate, NextReviewDate, RelatedControls, RelatedRisks, RelatedPolicy, EvidenceOfCompliance, Comments, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceObligation
        WHERE ObligationID = @ObligationID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ComplianceObligation', @ObligationID,
                (SELECT * FROM ComplianceObligation WHERE ObligationID = @ObligationID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteObligation'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ComplianceObligation', @ObligationID,
                (SELECT * FROM ComplianceObligation WHERE ObligationID = @ObligationID FOR XML AUTO), GETDATE())

        DELETE FROM ComplianceObligation WHERE ObligationID = @ObligationID AND CompanyID = @CompanyID
    END
END
