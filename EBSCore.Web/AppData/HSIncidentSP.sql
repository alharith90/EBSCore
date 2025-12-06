/****** Template reference: mirrors ProcessSP permission/history flow ******/
IF OBJECT_ID('dbo.HSIncident', 'U') IS NOT NULL
    DROP TABLE dbo.HSIncident;
GO

CREATE TABLE [dbo].[HSIncident]
(
    [IncidentID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [IncidentTitle] NVARCHAR(300) NULL,
    [IncidentDateTime] DATETIME NOT NULL,
    [Location] NVARCHAR(250) NULL,
    [IncidentType] NVARCHAR(150) NULL,
    [PersonsInvolved] NVARCHAR(500) NULL,
    [InjurySeverity] NVARCHAR(100) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ImmediateActions] NVARCHAR(MAX) NULL,
    [RootCause] NVARCHAR(MAX) NULL,
    [RelatedHazard] NVARCHAR(250) NULL,
    [RelatedActivity] NVARCHAR(250) NULL,
    [RelatedRegulation] NVARCHAR(250) NULL,
    [CorrectiveActions] NVARCHAR(MAX) NULL,
    [IncidentStatus] NVARCHAR(100) NULL,
    [ReportedBy] NVARCHAR(200) NULL,
    [Reportable] BIT NULL,
    [DateClosed] DATETIME NULL,
    [CreatedBy] BIGINT NULL,
    [ModifiedBy] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_HSIncident_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_HSIncident_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[HSIncidentSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @IncidentID INT = NULL,
    @IncidentTitle NVARCHAR(300) = NULL,
    @IncidentDateTime DATETIME = NULL,
    @Location NVARCHAR(250) = NULL,
    @IncidentType NVARCHAR(150) = NULL,
    @PersonsInvolved NVARCHAR(500) = NULL,
    @InjurySeverity NVARCHAR(100) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @ImmediateActions NVARCHAR(MAX) = NULL,
    @RootCause NVARCHAR(MAX) = NULL,
    @RelatedHazard NVARCHAR(250) = NULL,
    @RelatedActivity NVARCHAR(250) = NULL,
    @RelatedRegulation NVARCHAR(250) = NULL,
    @CorrectiveActions NVARCHAR(MAX) = NULL,
    @IncidentStatus NVARCHAR(100) = NULL,
    @ReportedBy NVARCHAR(200) = NULL,
    @Reportable BIT = NULL,
    @DateClosed DATETIME = NULL,
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

    IF @Operation = 'SaveHSIncident'
    BEGIN
        IF EXISTS(SELECT 1 FROM HSIncident WHERE IncidentID = @IncidentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'HSIncident', @IncidentID,
                    (SELECT * FROM HSIncident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[HSIncident]
            SET [IncidentTitle] = @IncidentTitle,
                [IncidentDateTime] = @IncidentDateTime,
                [Location] = @Location,
                [IncidentType] = @IncidentType,
                [PersonsInvolved] = @PersonsInvolved,
                [InjurySeverity] = @InjurySeverity,
                [Description] = @Description,
                [ImmediateActions] = @ImmediateActions,
                [RootCause] = @RootCause,
                [RelatedHazard] = @RelatedHazard,
                [RelatedActivity] = @RelatedActivity,
                [RelatedRegulation] = @RelatedRegulation,
                [CorrectiveActions] = @CorrectiveActions,
                [IncidentStatus] = @IncidentStatus,
                [ReportedBy] = @ReportedBy,
                [Reportable] = @Reportable,
                [DateClosed] = @DateClosed,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE IncidentID = @IncidentID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[HSIncident]
                ([CompanyID], [IncidentTitle], [IncidentDateTime], [Location], [IncidentType], [PersonsInvolved], [InjurySeverity], [Description], [ImmediateActions], [RootCause], [RelatedHazard], [RelatedActivity], [RelatedRegulation], [CorrectiveActions], [IncidentStatus], [ReportedBy], [Reportable], [DateClosed], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @IncidentTitle, @IncidentDateTime, @Location, @IncidentType, @PersonsInvolved, @InjurySeverity, @Description, @ImmediateActions, @RootCause, @RelatedHazard, @RelatedActivity, @RelatedRegulation, @CorrectiveActions, @IncidentStatus, @ReportedBy, @Reportable, @DateClosed, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewIncidentID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'HSIncident', @NewIncidentID,
                    (SELECT * FROM HSIncident WHERE IncidentID = @NewIncidentID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvHSIncidents'
    BEGIN
        SELECT
            IncidentID,
            CompanyID,
            IncidentTitle,
            IncidentDateTime,
            Location,
            IncidentType,
            PersonsInvolved,
            InjurySeverity,
            Description,
            ImmediateActions,
            RootCause,
            RelatedHazard,
            RelatedActivity,
            RelatedRegulation,
            CorrectiveActions,
            IncidentStatus,
            ReportedBy,
            Reportable,
            DateClosed,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [HSIncident]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'HSIncident', NULL,
                (SELECT * FROM HSIncident WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvHSIncident'
    BEGIN
        SELECT
            IncidentID,
            CompanyID,
            IncidentTitle,
            IncidentDateTime,
            Location,
            IncidentType,
            PersonsInvolved,
            InjurySeverity,
            Description,
            ImmediateActions,
            RootCause,
            RelatedHazard,
            RelatedActivity,
            RelatedRegulation,
            CorrectiveActions,
            IncidentStatus,
            ReportedBy,
            Reportable,
            DateClosed,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [HSIncident]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'HSIncident', @IncidentID,
                (SELECT * FROM HSIncident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteHSIncident'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'HSIncident', @IncidentID,
                (SELECT * FROM HSIncident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

        DELETE FROM [HSIncident]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;
    END
END
GO
