USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ComplianceIssueSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @IssueID BIGINT = NULL,
    @IssueDescription NVARCHAR(MAX) = NULL,
    @DateDetected DATETIME = NULL,
    @Source NVARCHAR(250) = NULL,
    @RelatedObligation NVARCHAR(250) = NULL,
    @Severity NVARCHAR(50) = NULL,
    @Impact NVARCHAR(MAX) = NULL,
    @RootCause NVARCHAR(MAX) = NULL,
    @CorrectiveActionPlan NVARCHAR(MAX) = NULL,
    @TargetCompletionDate DATETIME = NULL,
    @ActionStatus NVARCHAR(100) = NULL,
    @RegulatoryReportingDate DATETIME = NULL,
    @Status NVARCHAR(50) = NULL,
    @LessonsLearned NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveComplianceIssue'
    BEGIN
        IF EXISTS(SELECT 1 FROM ComplianceIssue WHERE IssueID = @IssueID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ComplianceIssue', @IssueID,
                    (SELECT * FROM ComplianceIssue WHERE IssueID = @IssueID FOR XML AUTO), GETDATE())

            UPDATE [ComplianceIssue]
            SET IssueDescription = @IssueDescription,
                DateDetected = @DateDetected,
                Source = @Source,
                RelatedObligation = @RelatedObligation,
                Severity = @Severity,
                Impact = @Impact,
                RootCause = @RootCause,
                CorrectiveActionPlan = @CorrectiveActionPlan,
                TargetCompletionDate = @TargetCompletionDate,
                ActionStatus = @ActionStatus,
                RegulatoryReportingDate = @RegulatoryReportingDate,
                Status = @Status,
                LessonsLearned = @LessonsLearned,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE IssueID = @IssueID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [ComplianceIssue]
            (
                CompanyID, IssueDescription, DateDetected, Source, RelatedObligation, Severity, Impact, RootCause, CorrectiveActionPlan, TargetCompletionDate, ActionStatus, RegulatoryReportingDate, Status, LessonsLearned, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @IssueDescription, @DateDetected, @Source, @RelatedObligation, @Severity, @Impact, @RootCause, @CorrectiveActionPlan, @TargetCompletionDate, @ActionStatus, @RegulatoryReportingDate, @Status, @LessonsLearned, @UserID, GETUTCDATE(), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ComplianceIssue', @IssueID,
                    (SELECT * FROM ComplianceIssue WHERE IssueID = (SELECT TOP 1 @@IDENTITY FROM ComplianceIssue) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvComplianceIssues'
    BEGIN
        SELECT IssueID, CompanyID, IssueDescription, DateDetected, Source, RelatedObligation, Severity, Impact, RootCause, CorrectiveActionPlan, TargetCompletionDate, ActionStatus, RegulatoryReportingDate, Status, LessonsLearned, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceIssue
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ComplianceIssue', NULL,
                (SELECT * FROM ComplianceIssue FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvComplianceIssue'
    BEGIN
        SELECT IssueID, CompanyID, IssueDescription, DateDetected, Source, RelatedObligation, Severity, Impact, RootCause, CorrectiveActionPlan, TargetCompletionDate, ActionStatus, RegulatoryReportingDate, Status, LessonsLearned, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ComplianceIssue
        WHERE IssueID = @IssueID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ComplianceIssue', @IssueID,
                (SELECT * FROM ComplianceIssue WHERE IssueID = @IssueID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteComplianceIssue'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ComplianceIssue', @IssueID,
                (SELECT * FROM ComplianceIssue WHERE IssueID = @IssueID FOR XML AUTO), GETDATE())

        DELETE FROM ComplianceIssue WHERE IssueID = @IssueID AND CompanyID = @CompanyID
    END
END
