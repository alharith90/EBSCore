USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AuditPlanSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @AuditID BIGINT = NULL,
    @AuditTitle NVARCHAR(250) = NULL,
    @AuditType NVARCHAR(100) = NULL,
    @ObjectivesScope NVARCHAR(MAX) = NULL,
    @Auditee NVARCHAR(250) = NULL,
    @LeadAuditor NVARCHAR(250) = NULL,
    @AuditTeam NVARCHAR(MAX) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @AuditCriteria NVARCHAR(MAX) = NULL,
    @FindingsCount INT = NULL,
    @OverallResult NVARCHAR(100) = NULL,
    @ReportIssuedDate DATETIME = NULL,
    @RelatedRisksReviewed NVARCHAR(MAX) = NULL,
    @RelatedControlsReviewed NVARCHAR(MAX) = NULL,
    @RelatedObligationsReviewed NVARCHAR(MAX) = NULL,
    @NextAuditDate DATETIME = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveAuditPlan'
    BEGIN
        IF EXISTS(SELECT 1 FROM AuditPlan WHERE AuditID = @AuditID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'AuditPlan', @AuditID,
                    (SELECT * FROM AuditPlan WHERE AuditID = @AuditID FOR XML AUTO), GETDATE())

            UPDATE [AuditPlan]
            SET AuditTitle = @AuditTitle,
                AuditType = @AuditType,
                ObjectivesScope = @ObjectivesScope,
                Auditee = @Auditee,
                LeadAuditor = @LeadAuditor,
                AuditTeam = @AuditTeam,
                StartDate = @StartDate,
                EndDate = @EndDate,
                AuditCriteria = @AuditCriteria,
                FindingsCount = @FindingsCount,
                OverallResult = @OverallResult,
                ReportIssuedDate = @ReportIssuedDate,
                RelatedRisksReviewed = @RelatedRisksReviewed,
                RelatedControlsReviewed = @RelatedControlsReviewed,
                RelatedObligationsReviewed = @RelatedObligationsReviewed,
                NextAuditDate = @NextAuditDate,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE AuditID = @AuditID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [AuditPlan]
            (
                CompanyID,
                AuditTitle,
                AuditType,
                ObjectivesScope,
                Auditee,
                LeadAuditor,
                AuditTeam,
                StartDate,
                EndDate,
                AuditCriteria,
                FindingsCount,
                OverallResult,
                ReportIssuedDate,
                RelatedRisksReviewed,
                RelatedControlsReviewed,
                RelatedObligationsReviewed,
                NextAuditDate,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @AuditTitle,
                @AuditType,
                @ObjectivesScope,
                @Auditee,
                @LeadAuditor,
                @AuditTeam,
                @StartDate,
                @EndDate,
                @AuditCriteria,
                @FindingsCount,
                @OverallResult,
                @ReportIssuedDate,
                @RelatedRisksReviewed,
                @RelatedControlsReviewed,
                @RelatedObligationsReviewed,
                @NextAuditDate,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'AuditPlan', @AuditID,
                    (SELECT * FROM AuditPlan WHERE AuditID = (SELECT TOP 1 @@IDENTITY FROM AuditPlan) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvAuditPlans'
    BEGIN
        SELECT AuditID, CompanyID, AuditTitle, AuditType, ObjectivesScope, Auditee, LeadAuditor, AuditTeam, StartDate, EndDate, AuditCriteria, FindingsCount, OverallResult, ReportIssuedDate, RelatedRisksReviewed, RelatedControlsReviewed, RelatedObligationsReviewed, NextAuditDate, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditPlan]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'AuditPlan', NULL,
                (SELECT * FROM AuditPlan FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvAuditPlan'
    BEGIN
        SELECT AuditID, CompanyID, AuditTitle, AuditType, ObjectivesScope, Auditee, LeadAuditor, AuditTeam, StartDate, EndDate, AuditCriteria, FindingsCount, OverallResult, ReportIssuedDate, RelatedRisksReviewed, RelatedControlsReviewed, RelatedObligationsReviewed, NextAuditDate, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditPlan]
        WHERE AuditID = @AuditID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'AuditPlan', @AuditID,
                (SELECT * FROM AuditPlan WHERE AuditID = @AuditID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteAuditPlan'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'AuditPlan', @AuditID,
                (SELECT * FROM AuditPlan WHERE AuditID = @AuditID FOR XML AUTO), GETDATE())

        DELETE FROM [AuditPlan] WHERE AuditID = @AuditID AND CompanyID = @CompanyID
    END
END
GO
