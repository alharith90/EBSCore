USE [EBS]
GO

/****** Template Reference: see ProcessSP above for permission and history pattern ******/

IF OBJECT_ID('dbo.RecoveryStrategyStep', 'U') IS NOT NULL
    DROP TABLE dbo.RecoveryStrategyStep;
GO

IF OBJECT_ID('dbo.RecoveryStrategy', 'U') IS NOT NULL
    DROP TABLE dbo.RecoveryStrategy;
GO

CREATE TABLE [dbo].[RecoveryStrategy]
(
    [RecoveryStrategyID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [UnitID] BIGINT NOT NULL,
    [FailureScenario] NVARCHAR(500) NOT NULL,
    [Strategy] NVARCHAR(MAX) NOT NULL,
    [CostImpact] DECIMAL(18,2) NULL,
    [ConfidenceLevel] NVARCHAR(100) NULL,
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RecoveryStrategy_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RecoveryStrategy_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE TABLE [dbo].[RecoveryStrategyStep]
(
    [StepID] INT IDENTITY(1,1) PRIMARY KEY,
    [RecoveryStrategyID] INT NOT NULL,
    [StepOrder] INT NOT NULL,
    [StepDescription] NVARCHAR(MAX) NOT NULL,
    [ValidationCheck] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RecoveryStrategyStep_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RecoveryStrategyStep_UpdatedAt DEFAULT (GETUTCDATE()),
    CONSTRAINT FK_RecoveryStrategyStep_Strategy FOREIGN KEY ([RecoveryStrategyID]) REFERENCES [dbo].[RecoveryStrategy]([RecoveryStrategyID]) ON DELETE CASCADE
);
GO

CREATE OR ALTER PROCEDURE [dbo].[RecoveryStrategySP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @RecoveryStrategyID INT = NULL,
    @FailureScenario NVARCHAR(500) = NULL,
    @Strategy NVARCHAR(MAX) = NULL,
    @CostImpact DECIMAL(18,2) = NULL,
    @ConfidenceLevel NVARCHAR(100) = NULL,
    @StepsJson NVARCHAR(MAX) = NULL,
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

    IF @Operation = 'SaveRecoveryStrategy'
    BEGIN
        IF EXISTS (SELECT 1 FROM RecoveryStrategy WHERE RecoveryStrategyID = @RecoveryStrategyID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'RecoveryStrategy', @RecoveryStrategyID,
                    (SELECT * FROM RecoveryStrategy WHERE RecoveryStrategyID = @RecoveryStrategyID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[RecoveryStrategy]
            SET [UnitID] = @UnitID,
                [FailureScenario] = @FailureScenario,
                [Strategy] = @Strategy,
                [CostImpact] = @CostImpact,
                [ConfidenceLevel] = @ConfidenceLevel,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE RecoveryStrategyID = @RecoveryStrategyID
              AND CompanyID = @CompanyID;

            DELETE FROM [dbo].[RecoveryStrategyStep]
            WHERE RecoveryStrategyID = @RecoveryStrategyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[RecoveryStrategy]
                ([CompanyID], [UnitID], [FailureScenario], [Strategy], [CostImpact], [ConfidenceLevel], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @FailureScenario, @Strategy, @CostImpact, @ConfidenceLevel, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @RecoveryStrategyID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'RecoveryStrategy', @RecoveryStrategyID,
                    (SELECT * FROM RecoveryStrategy WHERE RecoveryStrategyID = @RecoveryStrategyID FOR XML AUTO), GETDATE());
        END

        IF @StepsJson IS NOT NULL AND ISJSON(@StepsJson) = 1
        BEGIN
            ;WITH StepData AS (
                SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn,
                       StepOrder,
                       StepDescription,
                       ValidationCheck
                FROM OPENJSON(@StepsJson)
                     WITH (
                         StepOrder INT '$.StepOrder',
                         StepDescription NVARCHAR(MAX) '$.StepDescription',
                         ValidationCheck NVARCHAR(MAX) '$.ValidationCheck'
                     ) sd
            )
            INSERT INTO [dbo].[RecoveryStrategyStep]
                ([RecoveryStrategyID], [StepOrder], [StepDescription], [ValidationCheck], [CreatedAt], [UpdatedAt])
            SELECT @RecoveryStrategyID,
                   COALESCE(StepOrder, rn),
                   StepDescription,
                   ValidationCheck,
                   GETUTCDATE(),
                   GETUTCDATE()
            FROM StepData;
        END
    END
    ELSE IF @Operation = 'rtvRecoveryStrategies'
    BEGIN
        SELECT RecoveryStrategyID, CompanyID, UnitID, FailureScenario, Strategy, CostImpact, ConfidenceLevel,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[RecoveryStrategy]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RecoveryStrategy', NULL,
                (SELECT * FROM RecoveryStrategy WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvRecoveryStrategiesByUnit'
    BEGIN
        SELECT RecoveryStrategyID, CompanyID, UnitID, FailureScenario, Strategy, CostImpact, ConfidenceLevel,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[RecoveryStrategy]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RecoveryStrategy', NULL,
                (SELECT * FROM RecoveryStrategy WHERE CompanyID = @CompanyID AND UnitID = @UnitID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvRecoveryStrategy'
    BEGIN
        SELECT RecoveryStrategyID, CompanyID, UnitID, FailureScenario, Strategy, CostImpact, ConfidenceLevel,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[RecoveryStrategy]
        WHERE RecoveryStrategyID = @RecoveryStrategyID
          AND CompanyID = @CompanyID;

        SELECT StepID, RecoveryStrategyID, StepOrder, StepDescription, ValidationCheck, CreatedAt, UpdatedAt
        FROM [dbo].[RecoveryStrategyStep]
        WHERE RecoveryStrategyID = @RecoveryStrategyID
        ORDER BY StepOrder;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'RecoveryStrategy', @RecoveryStrategyID,
                (SELECT * FROM RecoveryStrategy WHERE RecoveryStrategyID = @RecoveryStrategyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteRecoveryStrategy'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'RecoveryStrategy', @RecoveryStrategyID,
                (SELECT * FROM RecoveryStrategy WHERE RecoveryStrategyID = @RecoveryStrategyID FOR XML AUTO), GETDATE());

        DELETE FROM [dbo].[RecoveryStrategy]
        WHERE RecoveryStrategyID = @RecoveryStrategyID
          AND CompanyID = @CompanyID;
    END
END
GO
