USE [EBS]
GO

-- Workflow tables
IF OBJECT_ID('[dbo].[S7SWorkflow]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7SWorkflow](
        [WorkflowID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [WorkflowCode] NVARCHAR(50) NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [Status] NVARCHAR(50) NULL,
        [Priority] NVARCHAR(50) NULL,
        [Frequency] NVARCHAR(50) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT DF_S7SWorkflow_IsActive DEFAULT(1),
        [CompanyID] INT NULL,
        [UnitID] BIGINT NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SWorkflow_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7SWorkflow_IsDeleted DEFAULT(0)
    );
END
GO

IF OBJECT_ID('[dbo].[S7SNode]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7SNode](
        [NodeID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [WorkflowID] INT NOT NULL,
        [NodeKey] NVARCHAR(200) NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [NodeType] NVARCHAR(100) NOT NULL,
        [ConfigJson] NVARCHAR(MAX) NULL,
        [PositionX] DECIMAL(18,4) NOT NULL,
        [PositionY] DECIMAL(18,4) NOT NULL,
        [CredentialID] INT NULL,
        [RetryCount] INT NOT NULL CONSTRAINT DF_S7SNode_RetryCount DEFAULT(0),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SNode_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7SNode_IsDeleted DEFAULT(0)
    );
END
GO

IF OBJECT_ID('[dbo].[S7SConnection]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7SConnection](
        [ConnectionID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [WorkflowID] INT NOT NULL,
        [SourceNodeID] INT NOT NULL,
        [SourceOutputKey] NVARCHAR(100) NULL,
        [TargetNodeID] INT NOT NULL,
        [TargetInputKey] NVARCHAR(100) NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SConnection_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7SConnection_IsDeleted DEFAULT(0)
    );
END
GO

IF OBJECT_ID('[dbo].[S7STrigger]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7STrigger](
        [TriggerID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [WorkflowID] INT NOT NULL,
        [TriggerType] NVARCHAR(50) NOT NULL,
        [TriggerNodeID] INT NULL,
        [Secret] NVARCHAR(200) NULL,
        [CronExpression] NVARCHAR(100) NULL,
        [ConfigurationJson] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT DF_S7STrigger_IsActive DEFAULT(1),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7STrigger_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7STrigger_IsDeleted DEFAULT(0)
    );
END
GO

IF OBJECT_ID('[dbo].[S7SExecution]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7SExecution](
        [ExecutionID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [WorkflowID] INT NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [TriggerType] NVARCHAR(50) NOT NULL,
        [TriggerDataJson] NVARCHAR(MAX) NULL,
        [WebhookRequestJson] NVARCHAR(MAX) NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [StartedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SExecution_StartedAt DEFAULT(sysutcdatetime()),
        [CompletedAt] DATETIME2 NULL,
        [RetryCount] INT NOT NULL CONSTRAINT DF_S7SExecution_RetryCount DEFAULT(0),
        [CompanyID] INT NULL,
        [UnitID] BIGINT NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SExecution_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7SExecution_IsDeleted DEFAULT(0)
    );
END
GO

IF OBJECT_ID('[dbo].[S7SStep]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[S7SStep](
        [StepID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ExecutionID] BIGINT NOT NULL,
        [NodeID] INT NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [SelectedOutputKey] NVARCHAR(100) NULL,
        [OutputJson] NVARCHAR(MAX) NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [StartedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SStep_StartedAt DEFAULT(sysutcdatetime()),
        [CompletedAt] DATETIME2 NULL,
        [RetryCount] INT NOT NULL CONSTRAINT DF_S7SStep_RetryCount DEFAULT(0),
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_S7SStep_CreatedAt DEFAULT(sysutcdatetime()),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2 NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT DF_S7SStep_IsDeleted DEFAULT(0)
    );
END
GO

-- Workflow stored procedure
IF OBJECT_ID('dbo.S7SWorkflowSP', 'P') IS NOT NULL
    DROP PROCEDURE dbo.S7SWorkflowSP;
GO
CREATE PROCEDURE [dbo].[S7SWorkflowSP]
    @Operation NVARCHAR(50),
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @WorkflowID INT = NULL,
    @WorkflowCode NVARCHAR(50) = NULL,
    @WorkflowName NVARCHAR(200) = NULL,
    @WorkflowDescription NVARCHAR(1000) = NULL,
    @Status NVARCHAR(50) = NULL,
    @Priority NVARCHAR(50) = NULL,
    @Frequency NVARCHAR(50) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @IsActive BIT = NULL,
    @NodesJson NVARCHAR(MAX) = NULL,
    @ConnectionsJson NVARCHAR(MAX) = NULL,
    @TriggersJson NVARCHAR(MAX) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @SearchQuery NVARCHAR(200) = NULL,
    @SortColumn NVARCHAR(50) = NULL,
    @SortDirection NVARCHAR(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'Save'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.S7SWorkflow WHERE WorkflowID = @WorkflowID AND IsDeleted = 0)
        BEGIN
            UPDATE dbo.S7SWorkflow
            SET WorkflowCode = @WorkflowCode,
                Name = @WorkflowName,
                Description = @WorkflowDescription,
                Status = @Status,
                Priority = @Priority,
                Frequency = @Frequency,
                Notes = @Notes,
                IsActive = ISNULL(@IsActive, IsActive),
                CompanyID = @CompanyID,
                UnitID = @UnitID,
                UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
                UpdatedAt = sysutcdatetime()
            WHERE WorkflowID = @WorkflowID;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.S7SWorkflow
            (WorkflowCode, Name, Description, Status, Priority, Frequency, Notes, IsActive, CompanyID, UnitID, CreatedBy)
            VALUES (@WorkflowCode, @WorkflowName, @WorkflowDescription, @Status, @Priority, @Frequency, @Notes, ISNULL(@IsActive, 1), @CompanyID, @UnitID, CAST(@UserID AS NVARCHAR(100)));
            SET @WorkflowID = SCOPE_IDENTITY();
        END;

        DELETE FROM dbo.S7SNode WHERE WorkflowID = @WorkflowID;
        DELETE FROM dbo.S7SConnection WHERE WorkflowID = @WorkflowID;
        DELETE FROM dbo.S7STrigger WHERE WorkflowID = @WorkflowID;

        IF (@NodesJson IS NOT NULL AND ISJSON(@NodesJson) = 1)
        BEGIN
            INSERT INTO dbo.S7SNode (WorkflowID, NodeKey, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount, CreatedBy)
            SELECT @WorkflowID, NodeKey, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount, CAST(@UserID AS NVARCHAR(100))
            FROM OPENJSON(@NodesJson)
            WITH (
                NodeKey NVARCHAR(200) '$.NodeKey',
                Name NVARCHAR(200) '$.Name',
                NodeType NVARCHAR(100) '$.NodeType',
                ConfigJson NVARCHAR(MAX) '$.ConfigJson',
                PositionX DECIMAL(18,4) '$.PositionX',
                PositionY DECIMAL(18,4) '$.PositionY',
                CredentialID INT '$.CredentialID',
                RetryCount INT '$.RetryCount'
            );
        END

        IF (@ConnectionsJson IS NOT NULL AND ISJSON(@ConnectionsJson) = 1)
        BEGIN
            INSERT INTO dbo.S7SConnection (WorkflowID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey, CreatedBy)
            SELECT @WorkflowID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey, CAST(@UserID AS NVARCHAR(100))
            FROM OPENJSON(@ConnectionsJson)
            WITH (
                SourceNodeID INT '$.SourceNodeID',
                SourceOutputKey NVARCHAR(100) '$.SourceOutputKey',
                TargetNodeID INT '$.TargetNodeID',
                TargetInputKey NVARCHAR(100) '$.TargetInputKey'
            );
        END

        IF (@TriggersJson IS NOT NULL AND ISJSON(@TriggersJson) = 1)
        BEGIN
            INSERT INTO dbo.S7STrigger (WorkflowID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, IsActive, CreatedBy)
            SELECT @WorkflowID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, ISNULL(IsActive, 1), CAST(@UserID AS NVARCHAR(100))
            FROM OPENJSON(@TriggersJson)
            WITH (
                TriggerType NVARCHAR(50) '$.TriggerType',
                TriggerNodeID INT '$.TriggerNodeID',
                Secret NVARCHAR(200) '$.Secret',
                CronExpression NVARCHAR(100) '$.CronExpression',
                ConfigurationJson NVARCHAR(MAX) '$.ConfigurationJson',
                IsActive BIT '$.IsActive'
            );
        END

        SELECT @WorkflowID AS WorkflowID;
        RETURN;
    END

    IF @Operation = 'Delete'
    BEGIN
        UPDATE dbo.S7SWorkflow SET IsDeleted = 1, UpdatedAt = sysutcdatetime(), UpdatedBy = CAST(@UserID AS NVARCHAR(100)) WHERE WorkflowID = @WorkflowID;
        RETURN;
    END

    IF @Operation = 'Retrieve'
    BEGIN
        SELECT WorkflowID, WorkflowCode, Name, Status, Priority, Frequency, IsActive
        FROM dbo.S7SWorkflow
        WHERE IsDeleted = 0
        ORDER BY WorkflowID DESC
        OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount FROM dbo.S7SWorkflow WHERE IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'RetrieveDetails'
    BEGIN
        SELECT WorkflowID, CompanyID, UnitID, WorkflowCode, Name, Description, Status, Priority, Frequency, Notes, IsActive
        FROM dbo.S7SWorkflow WHERE WorkflowID = @WorkflowID AND IsDeleted = 0;

        SELECT NodeID, NodeKey, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount
        FROM dbo.S7SNode WHERE WorkflowID = @WorkflowID AND IsDeleted = 0;

        SELECT ConnectionID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey
        FROM dbo.S7SConnection WHERE WorkflowID = @WorkflowID AND IsDeleted = 0;

        SELECT TriggerID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, IsActive
        FROM dbo.S7STrigger WHERE WorkflowID = @WorkflowID AND IsDeleted = 0;
        RETURN;
    END
END
GO

-- Execution stored procedure
IF OBJECT_ID('dbo.S7SWorkflowExecutionSP', 'P') IS NOT NULL
    DROP PROCEDURE dbo.S7SWorkflowExecutionSP;
GO
CREATE PROCEDURE [dbo].[S7SWorkflowExecutionSP]
    @Operation NVARCHAR(50),
    @UserID BIGINT = NULL,
    @WorkflowID INT = NULL,
    @ExecutionID BIGINT = NULL,
    @PayloadJson NVARCHAR(MAX) = NULL,
    @WebhookSecret NVARCHAR(200) = NULL,
    @RequestJson NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(50) = NULL,
    @ErrorMessage NVARCHAR(MAX) = NULL,
    @NodeID INT = NULL,
    @OutputJson NVARCHAR(MAX) = NULL,
    @OutputKey NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'ExecutionStartManual'
    BEGIN
        INSERT INTO dbo.S7SExecution (WorkflowID, Status, TriggerType, TriggerDataJson, CreatedBy)
        VALUES (@WorkflowID, 'Queued', 'Manual', @PayloadJson, CAST(@UserID AS NVARCHAR(100)));
        SELECT SCOPE_IDENTITY() AS ExecutionID;
        RETURN;
    END

    IF @Operation = 'ExecutionStartWebhook'
    BEGIN
        INSERT INTO dbo.S7SExecution (WorkflowID, Status, TriggerType, TriggerDataJson, WebhookRequestJson, CreatedBy)
        VALUES (@WorkflowID, 'Queued', 'Webhook', @PayloadJson, @RequestJson, CAST(@UserID AS NVARCHAR(100)));
        SELECT SCOPE_IDENTITY() AS ExecutionID;
        RETURN;
    END

    IF @Operation = 'ExecutionDequeue'
    BEGIN
        SELECT TOP 1 ExecutionID, WorkflowID, Status, TriggerType, TriggerDataJson, WebhookRequestJson
        FROM dbo.S7SExecution WITH (READPAST, UPDLOCK)
        WHERE Status = 'Queued' AND IsDeleted = 0
        ORDER BY ExecutionID;
        RETURN;
    END

    IF @Operation = 'ExecutionSaveStep'
    BEGIN
        INSERT INTO dbo.S7SStep (ExecutionID, NodeID, Status, SelectedOutputKey, OutputJson, ErrorMessage, CreatedBy)
        VALUES (@ExecutionID, @NodeID, ISNULL(@Status, 'InProgress'), @OutputKey, @OutputJson, @ErrorMessage, CAST(@UserID AS NVARCHAR(100)));
        RETURN;
    END

    IF @Operation = 'ExecutionMarkStatus'
    BEGIN
        UPDATE dbo.S7SExecution
        SET Status = @Status,
            ErrorMessage = @ErrorMessage,
            UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
            UpdatedAt = sysutcdatetime()
        WHERE ExecutionID = @ExecutionID;
        RETURN;
    END

    IF @Operation = 'Retrieve'
    BEGIN
        SELECT ExecutionID, WorkflowID, Status, TriggerType, ErrorMessage
        FROM dbo.S7SExecution
        WHERE IsDeleted = 0
        ORDER BY ExecutionID DESC
        OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount FROM dbo.S7SExecution WHERE IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'RetrieveDetails'
    BEGIN
        SELECT ExecutionID, WorkflowID, Status, TriggerType, TriggerDataJson, WebhookRequestJson, ErrorMessage
        FROM dbo.S7SExecution WHERE ExecutionID = @ExecutionID;

        SELECT StepID, NodeID, Status, SelectedOutputKey, OutputJson, ErrorMessage
        FROM dbo.S7SStep WHERE ExecutionID = @ExecutionID AND IsDeleted = 0;
        RETURN;
    END
END
GO
