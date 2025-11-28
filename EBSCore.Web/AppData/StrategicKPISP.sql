USE [EBS]
GO

/****** Template reference: see ProcessSP for permission and history pattern ******/

IF OBJECT_ID('dbo.KPILink', 'U') IS NOT NULL
    DROP TABLE dbo.KPILink;
GO

IF OBJECT_ID('dbo.KPIResult', 'U') IS NOT NULL
    DROP TABLE dbo.KPIResult;
GO

IF OBJECT_ID('dbo.StrategicKPI', 'U') IS NOT NULL
    DROP TABLE dbo.StrategicKPI;
GO

CREATE TABLE [dbo].[StrategicKPI]
(
    [KPIID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [UnitID] BIGINT NOT NULL,
    [Category] NVARCHAR(100) NULL,
    [Type] NVARCHAR(100) NULL,
    [ObjectiveID] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(250) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [TargetValue] DECIMAL(18,2) NULL,
    [ThresholdGreen] DECIMAL(18,2) NULL,
    [ThresholdRed] DECIMAL(18,2) NULL,
    [ThresholdMethod] NVARCHAR(50) NULL,
    [Unit] NVARCHAR(50) NULL,
    [Frequency] NVARCHAR(50) NULL,
    [CalculationMethod] NVARCHAR(250) NULL,
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

CREATE TABLE [dbo].[KPIResult]
(
    [ResultID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [KPIID] INT NOT NULL,
    [Period] NVARCHAR(50) NULL,
    [Value] DECIMAL(18,2) NULL,
    [Status] NVARCHAR(50) NULL,
    [Comments] NVARCHAR(MAX) NULL,
    [EvidenceFile] NVARCHAR(MAX) NULL,
    [CreatedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KPIResult_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KPIResult_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE TABLE [dbo].[KPILink]
(
    [LinkID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [KPIID] INT NOT NULL,
    [LinkedType] NVARCHAR(50) NULL,
    [LinkedID] NVARCHAR(50) NULL,
    [CreatedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KPILink_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KPILink_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

ALTER TABLE [dbo].[KPIResult]
ADD CONSTRAINT [FK_KPIResult_StrategicKPI] FOREIGN KEY ([KPIID], [CompanyID]) REFERENCES [dbo].[StrategicKPI]([KPIID], [CompanyID]);

ALTER TABLE [dbo].[KPILink]
ADD CONSTRAINT [FK_KPILink_StrategicKPI] FOREIGN KEY ([KPIID], [CompanyID]) REFERENCES [dbo].[StrategicKPI]([KPIID], [CompanyID]);
GO

CREATE OR ALTER PROCEDURE [dbo].[StrategicKPISP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @KPIID INT = NULL,
    @Category NVARCHAR(100) = NULL,
    @Type NVARCHAR(100) = NULL,
    @ObjectiveID NVARCHAR(50) = NULL,
    @Title NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @TargetValue DECIMAL(18,2) = NULL,
    @ThresholdGreen DECIMAL(18,2) = NULL,
    @ThresholdRed DECIMAL(18,2) = NULL,
    @ThresholdMethod NVARCHAR(50) = NULL,
    @Unit NVARCHAR(50) = NULL,
    @Frequency NVARCHAR(50) = NULL,
    @CalculationMethod NVARCHAR(250) = NULL,
    @Status NVARCHAR(50) = NULL,
    @Owner NVARCHAR(150) = NULL,
    @EscalationPlan NVARCHAR(MAX) = NULL,
    @ActivationCriteria NVARCHAR(MAX) = NULL,
    @ResultID INT = NULL,
    @Period NVARCHAR(50) = NULL,
    @Value DECIMAL(18,2) = NULL,
    @Comments NVARCHAR(MAX) = NULL,
    @EvidenceFile NVARCHAR(MAX) = NULL,
    @LinkID INT = NULL,
    @LinkedType NVARCHAR(50) = NULL,
    @LinkedID NVARCHAR(50) = NULL,
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
                [Category] = @Category,
                [Type] = @Type,
                [ObjectiveID] = @ObjectiveID,
                [Title] = @Title,
                [Description] = @Description,
                [TargetValue] = @TargetValue,
                [ThresholdGreen] = @ThresholdGreen,
                [ThresholdRed] = @ThresholdRed,
                [ThresholdMethod] = @ThresholdMethod,
                [Unit] = @Unit,
                [Frequency] = @Frequency,
                [CalculationMethod] = @CalculationMethod,
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
                ([CompanyID], [UnitID], [Category], [Type], [ObjectiveID], [Title], [Description], [TargetValue], [ThresholdGreen], [ThresholdRed],
                 [ThresholdMethod], [Unit], [Frequency], [CalculationMethod], [Status], [Owner], [EscalationPlan], [ActivationCriteria], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @Category, @Type, @ObjectiveID, @Title, @Description, @TargetValue, @ThresholdGreen, @ThresholdRed,
                 @ThresholdMethod, @Unit, @Frequency, @CalculationMethod, @Status, @Owner, @EscalationPlan, @ActivationCriteria, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @KPIID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'StrategicKPI', @KPIID,
                    (SELECT * FROM StrategicKPI WHERE KPIID = @KPIID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvStrategicKPIs'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, Category, Type, ObjectiveID, Title, Description, TargetValue, ThresholdGreen, ThresholdRed,
               ThresholdMethod, Unit, Frequency, CalculationMethod, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicKPI]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicKPI', NULL,
                (SELECT * FROM StrategicKPI WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicKPIsByUnit'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, Category, Type, ObjectiveID, Title, Description, TargetValue, ThresholdGreen, ThresholdRed,
               ThresholdMethod, Unit, Frequency, CalculationMethod, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicKPI]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicKPI', NULL,
                (SELECT * FROM StrategicKPI WHERE CompanyID = @CompanyID AND UnitID = @UnitID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicKPI'
    BEGIN
        SELECT KPIID, CompanyID, UnitID, Category, Type, ObjectiveID, Title, Description, TargetValue, ThresholdGreen, ThresholdRed,
               ThresholdMethod, Unit, Frequency, CalculationMethod, Status, Owner, EscalationPlan, ActivationCriteria, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
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
    ELSE IF @Operation = 'SaveKPIResult'
    BEGIN
        IF EXISTS (SELECT 1 FROM KPIResult WHERE ResultID = @ResultID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'KPIResult', @ResultID,
                    (SELECT * FROM KPIResult WHERE ResultID = @ResultID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[KPIResult]
            SET [KPIID] = @KPIID,
                [Period] = @Period,
                [Value] = @Value,
                [Status] = @Status,
                [Comments] = @Comments,
                [EvidenceFile] = @EvidenceFile,
                [UpdatedAt] = GETUTCDATE()
            WHERE ResultID = @ResultID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[KPIResult]
                ([CompanyID], [KPIID], [Period], [Value], [Status], [Comments], [EvidenceFile], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @KPIID, @Period, @Value, @Status, @Comments, @EvidenceFile, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @ResultID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'KPIResult', @ResultID,
                    (SELECT * FROM KPIResult WHERE ResultID = @ResultID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvKPIResults'
    BEGIN
        SELECT ResultID, CompanyID, KPIID, Period, Value, Status, Comments, EvidenceFile, CreatedBy, CreatedAt, UpdatedAt
        FROM [KPIResult]
        WHERE CompanyID = @CompanyID
          AND (@KPIID IS NULL OR KPIID = @KPIID);

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'KPIResult', NULL,
                (SELECT * FROM KPIResult WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteKPIResult'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'KPIResult', @ResultID,
                (SELECT * FROM KPIResult WHERE ResultID = @ResultID FOR XML AUTO), GETDATE());

        DELETE FROM [KPIResult]
        WHERE ResultID = @ResultID
          AND CompanyID = @CompanyID;
    END
    ELSE IF @Operation = 'SaveKPILink'
    BEGIN
        IF EXISTS (SELECT 1 FROM KPILink WHERE LinkID = @LinkID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'KPILink', @LinkID,
                    (SELECT * FROM KPILink WHERE LinkID = @LinkID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[KPILink]
            SET [KPIID] = @KPIID,
                [LinkedType] = @LinkedType,
                [LinkedID] = @LinkedID,
                [UpdatedAt] = GETUTCDATE()
            WHERE LinkID = @LinkID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[KPILink]
                ([CompanyID], [KPIID], [LinkedType], [LinkedID], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @KPIID, @LinkedType, @LinkedID, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @LinkID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'KPILink', @LinkID,
                    (SELECT * FROM KPILink WHERE LinkID = @LinkID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvKPILinks'
    BEGIN
        SELECT LinkID, CompanyID, KPIID, LinkedType, LinkedID, CreatedBy, CreatedAt, UpdatedAt
        FROM [KPILink]
        WHERE CompanyID = @CompanyID
          AND (@KPIID IS NULL OR KPIID = @KPIID);

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'KPILink', NULL,
                (SELECT * FROM KPILink WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteKPILink'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'KPILink', @LinkID,
                (SELECT * FROM KPILink WHERE LinkID = @LinkID FOR XML AUTO), GETDATE());

        DELETE FROM [KPILink]
        WHERE LinkID = @LinkID
          AND CompanyID = @CompanyID;
    END
END
GO
