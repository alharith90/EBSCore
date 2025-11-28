USE [EBS]
GO

IF OBJECT_ID('[dbo].[ProcessControl]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProcessControl]
    (
        [ProcessControlID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [ProcessID] INT NOT NULL,
        [StepID] INT NOT NULL,
        [Title] NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Type] NVARCHAR(100) NULL,
        [EvidenceRequired] BIT NOT NULL DEFAULT 0,
        [RelatedStandards] NVARCHAR(500) NULL,
        [CreatedBy] INT NULL,
        [UpdatedBy] INT NULL,
        [CreatedAt] DATETIME NULL,
        [UpdatedAt] DATETIME NULL
    );

    ALTER TABLE [dbo].[ProcessControl]
    ADD CONSTRAINT FK_ProcessControl_Process FOREIGN KEY ([ProcessID]) REFERENCES [dbo].[Process] ([ProcessID]);

    ALTER TABLE [dbo].[ProcessControl]
    ADD CONSTRAINT FK_ProcessControl_ProcessStep FOREIGN KEY ([StepID]) REFERENCES [dbo].[ProcessStep] ([StepID]);

    CREATE INDEX IX_ProcessControl_ProcessId ON [dbo].[ProcessControl] ([ProcessID]);
    CREATE INDEX IX_ProcessControl_StepId ON [dbo].[ProcessControl] ([StepID]);
END
GO

IF OBJECT_ID('[dbo].[ProcessControlSP]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[ProcessControlSP];
GO

CREATE PROCEDURE [dbo].[ProcessControlSP]
    @Operation            NVARCHAR(100) = NULL,
    @UserID               BIGINT = NULL,
    @CompanyID            INT = NULL,
    @ProcessID            INT = NULL,
    @StepID               INT = NULL,
    @ProcessControlID     INT = NULL,
    @Title                NVARCHAR(250) = NULL,
    @Description          NVARCHAR(MAX) = NULL,
    @Type                 NVARCHAR(100) = NULL,
    @EvidenceRequired     BIT = NULL,
    @RelatedStandards     NVARCHAR(500) = NULL,
    @ControlsJson         NVARCHAR(MAX) = NULL,
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

    IF @Operation = 'SaveProcessControls'
    BEGIN
        DECLARE @Controls TABLE
        (
            ProcessControlID INT NULL,
            ProcessID INT NOT NULL,
            StepID INT NOT NULL,
            Title NVARCHAR(250) NOT NULL,
            Description NVARCHAR(MAX) NULL,
            [Type] NVARCHAR(100) NULL,
            EvidenceRequired BIT NOT NULL DEFAULT 0,
            RelatedStandards NVARCHAR(500) NULL
        );

        INSERT INTO @Controls (ProcessControlID, ProcessID, StepID, Title, Description, [Type], EvidenceRequired, RelatedStandards)
        SELECT
            c.ProcessControlID,
            ISNULL(c.ProcessID, @ProcessID),
            c.StepID,
            c.Title,
            c.Description,
            c.[Type],
            ISNULL(c.EvidenceRequired, 0),
            c.RelatedStandards
        FROM OPENJSON(@ControlsJson)
        WITH
        (
            ProcessControlID INT '$.ProcessControlID',
            ProcessID INT '$.ProcessID',
            StepID INT '$.StepID',
            Title NVARCHAR(250) '$.Title',
            Description NVARCHAR(MAX) '$.Description',
            [Type] NVARCHAR(100) '$.Type',
            EvidenceRequired BIT '$.EvidenceRequired',
            RelatedStandards NVARCHAR(500) '$.RelatedStandards'
        ) c;

        MERGE INTO [dbo].[ProcessControl] AS target
        USING @Controls AS src
            ON target.ProcessControlID = src.ProcessControlID
            AND target.CompanyID = @CompanyID
        WHEN MATCHED THEN
            UPDATE SET
                ProcessID = ISNULL(src.ProcessID, @ProcessID),
                StepID = src.StepID,
                Title = src.Title,
                [Description] = src.Description,
                [Type] = src.[Type],
                EvidenceRequired = src.EvidenceRequired,
                RelatedStandards = src.RelatedStandards,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT
                (
                    CompanyID,
                    ProcessID,
                    StepID,
                    Title,
                    [Description],
                    [Type],
                    EvidenceRequired,
                    RelatedStandards,
                    CreatedBy,
                    UpdatedBy,
                    CreatedAt,
                    UpdatedAt
                )
            VALUES
                (
                    @CompanyID,
                    ISNULL(src.ProcessID, @ProcessID),
                    src.StepID,
                    src.Title,
                    src.Description,
                    src.[Type],
                    src.EvidenceRequired,
                    src.RelatedStandards,
                    @UserID,
                    @UserID,
                    GETUTCDATE(),
                    GETUTCDATE()
                );

        DELETE FROM [dbo].[ProcessControl]
        WHERE ProcessID = @ProcessID
            AND CompanyID = @CompanyID
            AND ProcessControlID NOT IN (SELECT ProcessControlID FROM @Controls WHERE ProcessControlID IS NOT NULL);

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'UPSERT',
            'ProcessControl',
            @ProcessID,
            @ControlsJson,
            GETDATE()
        );
    END
    ELSE IF @Operation = 'rtvProcessControls'
    BEGIN
        SELECT
            ProcessControlID,
            CompanyID,
            ProcessID,
            StepID,
            Title,
            [Description],
            [Type],
            EvidenceRequired,
            RelatedStandards,
            CreatedBy,
            UpdatedBy,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[ProcessControl]
        WHERE CompanyID = @CompanyID
            AND ProcessID = @ProcessID
        ORDER BY ProcessControlID ASC;

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'Select All',
            'ProcessControl',
            @ProcessID,
            (SELECT * FROM [ProcessControl] WHERE ProcessID = @ProcessID FOR XML AUTO),
            GETDATE()
        );
    END
    ELSE IF @Operation = 'rtvProcessControlsByStep'
    BEGIN
        SELECT
            ProcessControlID,
            CompanyID,
            ProcessID,
            StepID,
            Title,
            [Description],
            [Type],
            EvidenceRequired,
            RelatedStandards,
            CreatedBy,
            UpdatedBy,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[ProcessControl]
        WHERE CompanyID = @CompanyID
            AND StepID = @StepID
        ORDER BY ProcessControlID ASC;

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'Select All',
            'ProcessControl',
            @StepID,
            (SELECT * FROM [ProcessControl] WHERE StepID = @StepID FOR XML AUTO),
            GETDATE()
        );
    END
    ELSE IF @Operation = 'rtvProcessControl'
    BEGIN
        SELECT
            ProcessControlID,
            CompanyID,
            ProcessID,
            StepID,
            Title,
            [Description],
            [Type],
            EvidenceRequired,
            RelatedStandards,
            CreatedBy,
            UpdatedBy,
            CreatedAt,
            UpdatedAt
        FROM [dbo].[ProcessControl]
        WHERE CompanyID = @CompanyID
            AND ProcessControlID = @ProcessControlID;

        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'Select One',
            'ProcessControl',
            @ProcessControlID,
            (SELECT * FROM [ProcessControl] WHERE ProcessControlID = @ProcessControlID FOR XML AUTO),
            GETDATE()
        );
    END
    ELSE IF @Operation = 'DeleteProcessControl'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES
        (
            @UserID,
            'DELETE',
            'ProcessControl',
            @ProcessControlID,
            (SELECT * FROM [ProcessControl] WHERE ProcessControlID = @ProcessControlID FOR XML AUTO),
            GETDATE()
        );

        DELETE FROM [dbo].[ProcessControl]
        WHERE CompanyID = @CompanyID
            AND ProcessControlID = @ProcessControlID;
    END
END
GO
