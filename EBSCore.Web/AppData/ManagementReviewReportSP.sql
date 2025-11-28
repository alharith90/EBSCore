USE [EBS]
GO

/****** Template reference: ProcessSP was provided as a pattern for CRUD operations. ******/

IF OBJECT_ID('[dbo].[ManagementReviewReport]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ManagementReviewReport]
    (
        [ReportID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [UnitID] BIGINT NOT NULL,
        [ReportTitle] NVARCHAR(250) NOT NULL,
        [MeetingDate] DATETIME NULL,
        [Summary] NVARCHAR(MAX) NULL,
        [Decisions] NVARCHAR(MAX) NULL,
        [FollowUpActions] NVARCHAR(MAX) NULL,
        [NextReviewDate] DATETIME NULL,
        [Status] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [CreatedBy] INT NULL,
        [ModifiedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL CONSTRAINT DF_ManagementReviewReport_CreatedAt DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME NOT NULL CONSTRAINT DF_ManagementReviewReport_UpdatedAt DEFAULT (GETUTCDATE())
    );

    CREATE NONCLUSTERED INDEX IX_ManagementReviewReport_CompanyUnit
        ON [dbo].[ManagementReviewReport] ([CompanyID], [UnitID]);
END
GO

IF OBJECT_ID('[dbo].[ManagementReviewReportSP]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[ManagementReviewReportSP];
GO

CREATE PROCEDURE [dbo].[ManagementReviewReportSP]
    @Operation           NVARCHAR(100) = NULL,
    @UserID              BIGINT = NULL,
    @CompanyID           INT = NULL,
    @UnitID              BIGINT = NULL,
    @ReportID            INT = NULL,
    @ReportTitle         NVARCHAR(250) = NULL,
    @MeetingDate         DATETIME = NULL,
    @Summary             NVARCHAR(MAX) = NULL,
    @Decisions           NVARCHAR(MAX) = NULL,
    @FollowUpActions     NVARCHAR(MAX) = NULL,
    @NextReviewDate      DATETIME = NULL,
    @Status              NVARCHAR(50) = NULL,
    @Notes               NVARCHAR(MAX) = NULL,
    @CreatedBy           INT = NULL,
    @ModifiedBy          INT = NULL,
    @CreatedAt           DATETIME = NULL,
    @UpdatedAt           DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveManagementReviewReport'
    BEGIN
        IF EXISTS(SELECT 1 FROM ManagementReviewReport WHERE ReportID = @ReportID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ManagementReviewReport', @ReportID,
                    (SELECT * FROM ManagementReviewReport WHERE ReportID = @ReportID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[ManagementReviewReport]
            SET [UnitID] = @UnitID,
                [ReportTitle] = @ReportTitle,
                [MeetingDate] = @MeetingDate,
                [Summary] = @Summary,
                [Decisions] = @Decisions,
                [FollowUpActions] = @FollowUpActions,
                [NextReviewDate] = @NextReviewDate,
                [Status] = @Status,
                [Notes] = @Notes,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE ReportID = @ReportID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[ManagementReviewReport]
                ([CompanyID], [UnitID], [ReportTitle], [MeetingDate], [Summary], [Decisions], [FollowUpActions],
                 [NextReviewDate], [Status], [Notes], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @ReportTitle, @MeetingDate, @Summary, @Decisions, @FollowUpActions,
                 @NextReviewDate, @Status, @Notes, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewReportID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ManagementReviewReport', @NewReportID,
                    (SELECT * FROM ManagementReviewReport WHERE ReportID = @NewReportID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvManagementReviewReports'
    BEGIN
        SELECT ReportID, CompanyID, UnitID, ReportTitle, MeetingDate, Summary, Decisions, FollowUpActions,
               NextReviewDate, Status, Notes, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [ManagementReviewReport]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ManagementReviewReport', NULL,
                (SELECT * FROM ManagementReviewReport FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvManagementReviewReportsByUnit'
    BEGIN
        SELECT ReportID, CompanyID, UnitID, ReportTitle, MeetingDate, Summary, Decisions, FollowUpActions,
               NextReviewDate, Status, Notes, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [ManagementReviewReport]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ManagementReviewReport', NULL,
                (SELECT * FROM ManagementReviewReport FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvManagementReviewReport'
    BEGIN
        SELECT ReportID, CompanyID, UnitID, ReportTitle, MeetingDate, Summary, Decisions, FollowUpActions,
               NextReviewDate, Status, Notes, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [ManagementReviewReport]
        WHERE ReportID = @ReportID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ManagementReviewReport', @ReportID,
                (SELECT * FROM ManagementReviewReport WHERE ReportID = @ReportID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteManagementReviewReport'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ManagementReviewReport', @ReportID,
                (SELECT * FROM ManagementReviewReport WHERE ReportID = @ReportID FOR XML AUTO), GETDATE());

        DELETE FROM [ManagementReviewReport]
        WHERE ReportID = @ReportID
          AND CompanyID = @CompanyID;
    END
END
GO
