USE [EBS]
GO

-- Template reference: see ProcessSP for role/permission pattern
-- Incident tables and stored procedure built to support BCM escalation and activation tracking (ISO/NCEMA 7000 aligned)

IF OBJECT_ID('dbo.Incident', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Incident]
    (
        [IncidentID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [UnitID] BIGINT NULL,
        [Title] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [IncidentDate] DATETIME NOT NULL,
        [ReportedBy] NVARCHAR(255) NULL,
        [AffectedAssets] NVARCHAR(MAX) NULL,
        [RelatedRiskIDs] NVARCHAR(MAX) NULL,
        [ImpactedActivities] NVARCHAR(MAX) NULL,
        [EscalationLevel] NVARCHAR(50) NULL,
        [EscalationNotes] NVARCHAR(MAX) NULL,
        [EscalatedToBC] BIT NOT NULL CONSTRAINT DF_Incident_EscalatedToBC DEFAULT(0),
        [BCPActivated] BIT NOT NULL CONSTRAINT DF_Incident_BCPActivated DEFAULT(0),
        [ActivationReason] NVARCHAR(MAX) NULL,
        [ActivationTime] DATETIME NULL,
        [RecoveryStartTime] DATETIME NULL,
        [Status] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [CreatedBy] INT NULL,
        [UpdatedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL CONSTRAINT DF_Incident_CreatedAt DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME NOT NULL CONSTRAINT DF_Incident_UpdatedAt DEFAULT (GETUTCDATE())
    );
END
GO

IF OBJECT_ID('dbo.IncidentSP', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[IncidentSP];
GO

CREATE PROCEDURE [dbo].[IncidentSP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @UnitID                 BIGINT = NULL,
    @IncidentID             INT = NULL,
    @Title                  NVARCHAR(255) = NULL,
    @Description            NVARCHAR(MAX) = NULL,
    @IncidentDate           DATETIME = NULL,
    @ReportedBy             NVARCHAR(255) = NULL,
    @AffectedAssets         NVARCHAR(MAX) = NULL,
    @RelatedRiskIDs         NVARCHAR(MAX) = NULL,
    @ImpactedActivities     NVARCHAR(MAX) = NULL,
    @EscalationLevel        NVARCHAR(50) = NULL,
    @EscalationNotes        NVARCHAR(MAX) = NULL,
    @EscalatedToBC          BIT = NULL,
    @BCPActivated           BIT = NULL,
    @ActivationReason       NVARCHAR(MAX) = NULL,
    @ActivationTime         DATETIME = NULL,
    @RecoveryStartTime      DATETIME = NULL,
    @Status                 NVARCHAR(50) = NULL,
    @Notes                  NVARCHAR(MAX) = NULL,
    @CreatedBy              INT = NULL,
    @UpdatedBy              INT = NULL,
    @CreatedAt              DATETIME = NULL,
    @UpdatedAt              DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);
    DECLARE @ResolvedActivationTime AS DATETIME = CASE
                                                    WHEN ISNULL(@BCPActivated, 0) = 1 AND @ActivationTime IS NULL THEN GETUTCDATE()
                                                    ELSE @ActivationTime
                                                  END;

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveIncident'
    BEGIN
        IF EXISTS(SELECT 1 FROM Incident WHERE IncidentID = @IncidentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'Incident', @IncidentID,
                    (SELECT * FROM Incident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[Incident]
            SET [UnitID] = @UnitID,
                [Title] = @Title,
                [Description] = @Description,
                [IncidentDate] = @IncidentDate,
                [ReportedBy] = @ReportedBy,
                [AffectedAssets] = @AffectedAssets,
                [RelatedRiskIDs] = @RelatedRiskIDs,
                [ImpactedActivities] = @ImpactedActivities,
                [EscalationLevel] = @EscalationLevel,
                [EscalationNotes] = @EscalationNotes,
                [EscalatedToBC] = ISNULL(@EscalatedToBC, 0),
                [BCPActivated] = ISNULL(@BCPActivated, 0),
                [ActivationReason] = @ActivationReason,
                [ActivationTime] = @ResolvedActivationTime,
                [RecoveryStartTime] = @RecoveryStartTime,
                [Status] = @Status,
                [Notes] = @Notes,
                [UpdatedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE IncidentID = @IncidentID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[Incident]
                ([CompanyID], [UnitID], [Title], [Description], [IncidentDate], [ReportedBy], [AffectedAssets], [RelatedRiskIDs],
                 [ImpactedActivities], [EscalationLevel], [EscalationNotes], [EscalatedToBC], [BCPActivated], [ActivationReason],
                 [ActivationTime], [RecoveryStartTime], [Status], [Notes], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @Title, @Description, @IncidentDate, @ReportedBy, @AffectedAssets, @RelatedRiskIDs,
                 @ImpactedActivities, @EscalationLevel, @EscalationNotes, ISNULL(@EscalatedToBC, 0), ISNULL(@BCPActivated, 0),
                 @ActivationReason, @ResolvedActivationTime, @RecoveryStartTime, @Status, @Notes, @UserID, @UserID,
                 GETUTCDATE(), GETUTCDATE());

            DECLARE @NewIncidentID INT = SCOPE_IDENTITY();
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'Incident', @NewIncidentID,
                    (SELECT * FROM Incident WHERE IncidentID = @NewIncidentID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvIncidents'
    BEGIN
        SELECT *
        FROM [Incident]
        WHERE CompanyID = @CompanyID
        ORDER BY IncidentDate DESC, IncidentID DESC;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Incident', NULL, (SELECT * FROM Incident FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvIncidentsByUnit'
    BEGIN
        SELECT *
        FROM [Incident]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID
        ORDER BY IncidentDate DESC, IncidentID DESC;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Incident', NULL, (SELECT * FROM Incident FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvIncident'
    BEGIN
        SELECT *
        FROM [Incident]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'Incident', @IncidentID,
                (SELECT * FROM Incident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteIncident'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'Incident', @IncidentID,
                (SELECT * FROM Incident WHERE IncidentID = @IncidentID FOR XML AUTO), GETDATE());

        DELETE FROM [Incident]
        WHERE IncidentID = @IncidentID
          AND CompanyID = @CompanyID;
    END
END
GO
