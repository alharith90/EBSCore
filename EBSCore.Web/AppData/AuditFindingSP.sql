USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AuditFindingSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @FindingID BIGINT = NULL,
    @AuditID BIGINT = NULL,
    @FindingTitle NVARCHAR(250) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @Condition NVARCHAR(MAX) = NULL,
    @Cause NVARCHAR(MAX) = NULL,
    @Effect NVARCHAR(MAX) = NULL,
    @Recommendation NVARCHAR(MAX) = NULL,
    @Category NVARCHAR(100) = NULL,
    @Severity NVARCHAR(50) = NULL,
    @RelatedRiskControl NVARCHAR(MAX) = NULL,
    @ResponsibleOwner NVARCHAR(250) = NULL,
    @ActionPlanLink NVARCHAR(500) = NULL,
    @DueDate DATETIME = NULL,
    @Status NVARCHAR(50) = NULL,
    @DateClosed DATETIME = NULL,
    @VerifiedBy NVARCHAR(250) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveAuditFinding'
    BEGIN
        IF EXISTS(SELECT 1 FROM AuditFinding WHERE FindingID = @FindingID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'AuditFinding', @FindingID,
                    (SELECT * FROM AuditFinding WHERE FindingID = @FindingID FOR XML AUTO), GETDATE())

            UPDATE [AuditFinding]
            SET AuditID = @AuditID,
                FindingTitle = @FindingTitle,
                Criteria = @Criteria,
                [Condition] = @Condition,
                Cause = @Cause,
                Effect = @Effect,
                Recommendation = @Recommendation,
                Category = @Category,
                Severity = @Severity,
                RelatedRiskControl = @RelatedRiskControl,
                ResponsibleOwner = @ResponsibleOwner,
                ActionPlanLink = @ActionPlanLink,
                DueDate = @DueDate,
                Status = @Status,
                DateClosed = @DateClosed,
                VerifiedBy = @VerifiedBy,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE FindingID = @FindingID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [AuditFinding]
            (
                CompanyID,
                AuditID,
                FindingTitle,
                Criteria,
                [Condition],
                Cause,
                Effect,
                Recommendation,
                Category,
                Severity,
                RelatedRiskControl,
                ResponsibleOwner,
                ActionPlanLink,
                DueDate,
                Status,
                DateClosed,
                VerifiedBy,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @AuditID,
                @FindingTitle,
                @Criteria,
                @Condition,
                @Cause,
                @Effect,
                @Recommendation,
                @Category,
                @Severity,
                @RelatedRiskControl,
                @ResponsibleOwner,
                @ActionPlanLink,
                @DueDate,
                @Status,
                @DateClosed,
                @VerifiedBy,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'AuditFinding', @FindingID,
                    (SELECT * FROM AuditFinding WHERE FindingID = (SELECT TOP 1 @@IDENTITY FROM AuditFinding) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvAuditFindings'
    BEGIN
        SELECT FindingID, CompanyID, AuditID, FindingTitle, Criteria, [Condition], Cause, Effect, Recommendation, Category, Severity, RelatedRiskControl, ResponsibleOwner, ActionPlanLink, DueDate, Status, DateClosed, VerifiedBy, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditFinding]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'AuditFinding', NULL,
                (SELECT * FROM AuditFinding FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvAuditFinding'
    BEGIN
        SELECT FindingID, CompanyID, AuditID, FindingTitle, Criteria, [Condition], Cause, Effect, Recommendation, Category, Severity, RelatedRiskControl, ResponsibleOwner, ActionPlanLink, DueDate, Status, DateClosed, VerifiedBy, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditFinding]
        WHERE FindingID = @FindingID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'AuditFinding', @FindingID,
                (SELECT * FROM AuditFinding WHERE FindingID = @FindingID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteAuditFinding'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'AuditFinding', @FindingID,
                (SELECT * FROM AuditFinding WHERE FindingID = @FindingID FOR XML AUTO), GETDATE())

        DELETE FROM [AuditFinding] WHERE FindingID = @FindingID AND CompanyID = @CompanyID
    END
END
GO
