USE [EBS]
GO

IF OBJECT_ID('[dbo].[ProcessStep]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProcessStep]
    (
        [StepID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [ProcessID] INT NOT NULL,
        [Title] NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [StepOrder] INT NOT NULL,
        [RoleID] BIGINT NULL,
        [ExpectedOutput] NVARCHAR(MAX) NULL,
        [EscalationMinutes] INT NULL,
        [ActivationCriteria] NVARCHAR(500) NULL,
        [CreatedBy] INT NULL,
        [UpdatedBy] INT NULL,
        [CreatedAt] DATETIME NULL,
        [UpdatedAt] DATETIME NULL
    );

    ALTER TABLE [dbo].[ProcessStep]
    ADD CONSTRAINT FK_ProcessStep_Process FOREIGN KEY ([ProcessID]) REFERENCES [dbo].[Process] ([ProcessID]);

    CREATE INDEX IX_ProcessStep_ProcessId ON [dbo].[ProcessStep] ([ProcessID]);
END
GO

IF OBJECT_ID('[dbo].[ProcessStepSP]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[ProcessStepSP];
GO

CREATE PROCEDURE [dbo].[ProcessStepSP]
    @Operation            NVARCHAR(100) = NULL,
    @UserID               BIGINT = NULL,
    @CompanyID            INT = NULL,
    @ProcessID            INT = NULL,
    @StepID               INT = NULL,
    @Title                NVARCHAR(250) = NULL,
    @Description          NVARCHAR(MAX) = NULL,
    @StepOrder            INT = NULL,
    @RoleID               BIGINT = NULL,
    @ExpectedOutput       NVARCHAR(MAX) = NULL,
    @EscalationMinutes    INT = NULL,
    @ActivationCriteria   NVARCHAR(500) = NULL,
    @StepsJson            NVARCHAR(MAX) = NULL,
    @CreatedBy            INT = NULL,
    @UpdatedBy            INT = NULL,
    @CreatedAt            DATETIME = NULL,
    @UpdatedAt            DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveProcessSteps'
    BEGIN
        DECLARE @Steps TABLE
        (
            StepID INT NULL,
            ProcessID INT NOT NULL,
            Title NVARCHAR(250) NOT NULL,
            Description NVARCHAR(MAX) NULL,
            StepOrder INT NOT NULL,
            RoleID BIGINT NULL,
            ExpectedOutput NVARCHAR(MAX) NULL,
            EscalationMinutes INT NULL,
            ActivationCriteria NVARCHAR(500) NULL
        );

        INSERT INTO @Steps (StepID, ProcessID, Title, Description, StepOrder, RoleID, ExpectedOutput, EscalationMinutes, ActivationCriteria)
        SELECT
            s.StepID,
            ISNULL(s.ProcessID, @ProcessID),
            s.Title,
            s.Description,
            ISNULL(s.StepOrder, ROW_NUMBER() OVER(ORDER BY (SELECT NULL))),
            s.RoleID,
            s.ExpectedOutput,
            s.EscalationMinutes,
            s.ActivationCriteria
        FROM OPENJSON(@StepsJson)
        WITH
        (
            StepID INT '$.StepID',
            ProcessID INT '$.ProcessID',
            Title NVARCHAR(250) '$.Title',
            Description NVARCHAR(MAX) '$.Description',
            StepOrder INT '$.StepOrder',
            RoleID BIGINT '$.RoleID',
            ExpectedOutput NVARCHAR(MAX) '$.ExpectedOutput',
            EscalationMinutes INT '$.EscalationMinutes',
            ActivationCriteria NVARCHAR(500) '$.ActivationCriteria'
        ) s;

        MERGE INTO [dbo].[ProcessStep] AS target
        USING @Steps AS src
            ON target.StepID = src.StepID
            AND target.ProcessID = @ProcessID
            AND target.CompanyID = @CompanyID
        WHEN MATCHED THEN
            UPDATE SET
                Title = src.Title,
                [Description] = src.Description,
                StepOrder = src.StepOrder,
                RoleID = src.RoleID,
                ExpectedOutput = src.ExpectedOutput,
                EscalationMinutes = src.EscalationMinutes,
                ActivationCriteria = src.ActivationCriteria,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT
                (
                    CompanyID,
                    ProcessID,
                    Title,
                    [Description],
                    StepOrder,
                    RoleID,
                    ExpectedOutput,
                    EscalationMinutes,
                    ActivationCriteria,
                    CreatedBy,
                    UpdatedBy,
                    CreatedAt,
                    UpdatedAt
                )
            VALUES
                (
                    @CompanyID,
                    @ProcessID,
                    src.Title,
                    src.Description,
                    src.StepOrder,
                    src.RoleID,
                    src.ExpectedOutput,
                    src.EscalationMinutes,
                    src.ActivationCriteria,
                    @UserID,
                    @UserID,
                    GETUTCDATE(),
                    GETUTCDATE()
                );

        DELETE FROM [dbo].[ProcessStep]
        WHERE ProcessID = @ProcessID
            AND CompanyID = @CompanyID
            AND StepID NOT IN (SELECT StepID FROM @Steps WHERE StepID IS NOT NULL);

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'UPSERT',
            'ProcessStep',
            @ProcessID,
            @StepsJson,
            GETDATE()
        );
    END
    ELSE IF @Operation = 'rtvProcessSteps'
    BEGIN
        SELECT
            StepID,
            CompanyID,
            ProcessID,
            Title,
            [Description],
            StepOrder,
            RoleID,
            ExpectedOutput,
            EscalationMinutes,
            ActivationCriteria,
            CreatedBy,
            UpdatedBy,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[ProcessStep]
        WHERE CompanyID = @CompanyID
            AND ProcessID = @ProcessID
        ORDER BY StepOrder ASC;

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'Select All',
            'ProcessStep',
            @ProcessID,
            (SELECT * FROM [ProcessStep] WHERE ProcessID = @ProcessID FOR XML AUTO),
            GETDATE()
        );
    END
    ELSE IF @Operation = 'rtvProcessStep'
    BEGIN
        SELECT
            StepID,
            CompanyID,
            ProcessID,
            Title,
            [Description],
            StepOrder,
            RoleID,
            ExpectedOutput,
            EscalationMinutes,
            ActivationCriteria,
            CreatedBy,
            UpdatedBy,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[ProcessStep]
        WHERE CompanyID = @CompanyID
            AND StepID = @StepID;

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'Select One',
            'ProcessStep',
            @StepID,
            (SELECT * FROM [ProcessStep] WHERE StepID = @StepID FOR XML AUTO),
            GETDATE()
        );
    END
    ELSE IF @Operation = 'DeleteProcessStep'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'DELETE',
            'ProcessStep',
            @StepID,
            (SELECT * FROM [ProcessStep] WHERE StepID = @StepID FOR XML AUTO),
            GETDATE()
        );

        DELETE FROM [dbo].[ProcessStep]
        WHERE CompanyID = @CompanyID
            AND StepID = @StepID;
    END
END
GO
