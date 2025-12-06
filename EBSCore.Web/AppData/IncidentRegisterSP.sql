/****** Template reference: mirrors ProcessSP permission/history flow ******/
IF OBJECT_ID('dbo.IncidentRegister', 'U') IS NOT NULL
    DROP TABLE dbo.IncidentRegister;
GO

CREATE TABLE [dbo].[IncidentRegister]
(
    [IncidentID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [IncidentDescription] NVARCHAR(MAX) NOT NULL,
    [IncidentDate] DATETIME NOT NULL,
    [ImpactedArea] NVARCHAR(250) NULL,
    [Severity] NVARCHAR(100) NULL,
    [RootCause] NVARCHAR(MAX) NULL,
    [ActionsTaken] NVARCHAR(MAX) NULL,
    [IncidentOwner] NVARCHAR(200) NULL,
    [Status] NVARCHAR(100) NULL,
    [CreatedBy] BIGINT NULL,
    [ModifiedBy] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_IncidentRegister_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_IncidentRegister_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[IncidentRegisterSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @IncidentID INT = NULL,
    @IncidentDescription NVARCHAR(MAX) = NULL,
    @IncidentDate DATETIME = NULL,
    @ImpactedArea NVARCHAR(250) = NULL,
    @Severity NVARCHAR(100) = NULL,
    @RootCause NVARCHAR(MAX) = NULL,
    @ActionsTaken NVARCHAR(MAX) = NULL,
    @IncidentOwner NVARCHAR(200) = NULL,
    @Status NVARCHAR(100) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveIncident'
    BEGIN
        IF EXISTS(SELECT 1 FROM IncidentRegister WHERE IncidentID = @IncidentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'IncidentRegister', @IncidentID,
                    (SELECT * FROM IncidentRegister WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[IncidentRegister]
            SET [IncidentDescription] = @IncidentDescription,
                [IncidentDate] = @IncidentDate,
                [ImpactedArea] = @ImpactedArea,
                [Severity] = @Severity,
                [RootCause] = @RootCause,
                [ActionsTaken] = @ActionsTaken,
                [IncidentOwner] = @IncidentOwner,
                [Status] = @Status,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE IncidentID = @IncidentID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[IncidentRegister]
                ([CompanyID], [IncidentDescription], [IncidentDate], [ImpactedArea], [Severity], [RootCause], [ActionsTaken], [IncidentOwner], [Status], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @IncidentDescription, @IncidentDate, @ImpactedArea, @Severity, @RootCause, @ActionsTaken, @IncidentOwner, @Status, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewIncidentID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'IncidentRegister', @NewIncidentID,
                    (SELECT * FROM IncidentRegister WHERE IncidentID = @NewIncidentID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvIncidents'
    BEGIN
        SELECT
            IncidentID,
            CompanyID,
            IncidentDescription,
            IncidentDate,
            ImpactedArea,
            Severity,
            RootCause,
            ActionsTaken,
            IncidentOwner,
            Status,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [IncidentRegister]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'IncidentRegister', NULL,
                (SELECT * FROM IncidentRegister WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvIncident'
    BEGIN
        SELECT
            IncidentID,
            CompanyID,
            IncidentDescription,
            IncidentDate,
            ImpactedArea,
            Severity,
            RootCause,
            ActionsTaken,
            IncidentOwner,
            Status,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [IncidentRegister]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'IncidentRegister', @IncidentID,
                (SELECT * FROM IncidentRegister WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteIncident'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'IncidentRegister', @IncidentID,
                (SELECT * FROM IncidentRegister WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

        DELETE FROM [IncidentRegister]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;
    END
END
GO
