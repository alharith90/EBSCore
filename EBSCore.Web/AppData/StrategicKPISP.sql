USE [EBS]
GO

/****** Template reference: see ProcessSP for permission and history pattern ******/

IF OBJECT_ID('dbo.StrategicKPI', 'U') IS NOT NULL
    DROP TABLE dbo.StrategicKPI;
GO

CREATE TABLE [dbo].[StrategicKPI]
(
    [KPIID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [UnitID] BIGINT NOT NULL,
    [ObjectiveID] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(250) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [TargetValue] DECIMAL(18,2) NULL,
    [CurrentValue] DECIMAL(18,2) NULL,
    [Unit] NVARCHAR(50) NULL,
    [Frequency] NVARCHAR(50) NULL,
    [Status] NVARCHAR(50) NULL,
    [Owner] NVARCHAR(150) NULL,
    [EscalationPlan] NVARCHAR(MAX) NULL,
    [ActivationCriteria] NVARCHAR(MAX) NULL,
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_StrategicKPI_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_StrategicKPI_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[StrategicKPISP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @KPIID INT = NULL,
    @ObjectiveID NVARCHAR(50) = NULL,
    @Title NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @TargetValue DECIMAL(18,2) = NULL,
    @CurrentValue DECIMAL(18,2) = NULL,
    @Unit NVARCHAR(50) = NULL,
    @Frequency NVARCHAR(50) = NULL,
    @Status NVARCHAR(50) = NULL,
    @Owner NVARCHAR(150) = NULL,
    @EscalationPlan NVARCHAR(MAX) = NULL,
    @ActivationCriteria NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveStrategicKPI'
    BEGIN
        IF EXISTS (SELECT 1 FROM StrategicKPI WHERE KPIID = @KPIID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'StrategicKPI', @KPIID,
                    (SELECT * FROM StrategicKPI WHERE KPIID = @KPIID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[StrategicKPI]
            SET [UnitID] = @UnitID,
                [ObjectiveID] = @ObjectiveID,
                [Title] = @Title,
                [Description] = @Description,
                [TargetValue] = @TargetValue,
                [CurrentValue] = @CurrentValue,
                [Unit] = @Unit,
                [Frequency] = @Frequency,
                [Status] = @Status,
                [Owner] = @Owner,
                [EscalationPlan] = @EscalationPlan,
                [ActivationCriteria] = @ActivationCriteria,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE KPIID = @KPIID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[StrategicKPI]
                ([CompanyID], [UnitID], [ObjectiveID], [Title], [Description], [TargetValue], [CurrentValue],
                 [Unit], [Frequency], [Status], [Owner], [EscalationPlan], [ActivationCriteria], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @ObjectiveID, @Title, @Description, @TargetValue, @CurrentValue,
                 @Unit, @Frequency, @Status, @Owner, @EscalationPlan, @ActivationCriteria, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @KPIID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'StrategicKPI', @KPIID,
                    (SELECT * FROM StrategicKPI WHERE KPIID = @KPIID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvStrategicKPIs'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, ObjectiveID, Title, Description, TargetValue, CurrentValue,
               Unit, Frequency, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicKPI]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicKPI', NULL,
                (SELECT * FROM StrategicKPI WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicKPIsByUnit'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, ObjectiveID, Title, Description, TargetValue, CurrentValue,
               Unit, Frequency, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicKPI]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicKPI', NULL,
                (SELECT * FROM StrategicKPI WHERE CompanyID = @CompanyID AND UnitID = @UnitID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicKPI'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, ObjectiveID, Title, Description, TargetValue, CurrentValue,
               Unit, Frequency, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicKPI]
        WHERE KPIID = @KPIID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'StrategicKPI', @KPIID,
                (SELECT * FROM StrategicKPI WHERE KPIID = @KPIID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteStrategicKPI'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'StrategicKPI', @KPIID,
                (SELECT * FROM StrategicKPI WHERE KPIID = @KPIID FOR XML AUTO), GETDATE());

        DELETE FROM [StrategicKPI]
        WHERE KPIID = @KPIID
          AND CompanyID = @CompanyID;
    END
END
GO
