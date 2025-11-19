-- Workflow Engine schema for SQL Server
-- All database objects follow the S7S prefix requirement

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SWorkflow')
BEGIN
    CREATE TABLE S7SWorkflow
    (
        WorkflowID            INT IDENTITY(1,1) PRIMARY KEY,
        Name                  NVARCHAR(200)        NOT NULL,
        Description           NVARCHAR(1000)       NULL,
        IsActive              BIT                  NOT NULL DEFAULT (1),
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SNode')
BEGIN
    CREATE TABLE S7SNode
    (
        NodeID                INT IDENTITY(1,1) PRIMARY KEY,
        WorkflowID            INT                  NOT NULL,
        Name                  NVARCHAR(200)        NOT NULL,
        NodeType              NVARCHAR(100)        NOT NULL,
        ConfigJson            NVARCHAR(MAX)        NULL,
        PositionX             DECIMAL(18,4)        NOT NULL DEFAULT (0),
        PositionY             DECIMAL(18,4)        NOT NULL DEFAULT (0),
        CredentialID          INT                  NULL,
        RetryCount            INT                  NOT NULL DEFAULT (0),
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0),
        CONSTRAINT FK_S7SNode_Workflow FOREIGN KEY (WorkflowID) REFERENCES S7SWorkflow(WorkflowID)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SNodeConnection')
BEGIN
    CREATE TABLE S7SNodeConnection
    (
        NodeConnectionID      INT IDENTITY(1,1) PRIMARY KEY,
        WorkflowID            INT                  NOT NULL,
        SourceNodeID          INT                  NOT NULL,
        SourceOutputKey       NVARCHAR(100)        NULL,
        TargetNodeID          INT                  NOT NULL,
        TargetInputKey        NVARCHAR(100)        NULL,
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0),
        CONSTRAINT FK_S7SNodeConnection_Workflow FOREIGN KEY (WorkflowID) REFERENCES S7SWorkflow(WorkflowID),
        CONSTRAINT FK_S7SNodeConnection_SourceNode FOREIGN KEY (SourceNodeID) REFERENCES S7SNode(NodeID),
        CONSTRAINT FK_S7SNodeConnection_TargetNode FOREIGN KEY (TargetNodeID) REFERENCES S7SNode(NodeID)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SWorkflowTrigger')
BEGIN
    CREATE TABLE S7SWorkflowTrigger
    (
        WorkflowTriggerID     INT IDENTITY(1,1) PRIMARY KEY,
        WorkflowID            INT                  NOT NULL,
        TriggerType           NVARCHAR(50)         NOT NULL,
        TriggerNodeID         INT                  NULL,
        Secret                NVARCHAR(200)        NULL,
        CronExpression        NVARCHAR(100)        NULL,
        ConfigurationJson     NVARCHAR(MAX)        NULL,
        IsActive              BIT                  NOT NULL DEFAULT (1),
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0),
        CONSTRAINT FK_S7SWorkflowTrigger_Workflow FOREIGN KEY (WorkflowID) REFERENCES S7SWorkflow(WorkflowID),
        CONSTRAINT FK_S7SWorkflowTrigger_Node FOREIGN KEY (TriggerNodeID) REFERENCES S7SNode(NodeID)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SCredential')
BEGIN
    CREATE TABLE S7SCredential
    (
        CredentialID          INT IDENTITY(1,1) PRIMARY KEY,
        Name                  NVARCHAR(200)        NOT NULL,
        CredentialType        NVARCHAR(100)        NOT NULL,
        DataJson              NVARCHAR(MAX)        NOT NULL,
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SExecution')
BEGIN
    CREATE TABLE S7SExecution
    (
        ExecutionID           BIGINT IDENTITY(1,1) PRIMARY KEY,
        WorkflowID            INT                  NOT NULL,
        Status                NVARCHAR(50)         NOT NULL,
        TriggerType           NVARCHAR(50)         NOT NULL,
        TriggerDataJson       NVARCHAR(MAX)        NULL,
        WebhookRequestJson    NVARCHAR(MAX)        NULL,
        ErrorMessage          NVARCHAR(MAX)        NULL,
        StartedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        CompletedAt           DATETIME2            NULL,
        RetryCount            INT                  NOT NULL DEFAULT (0),
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0),
        CONSTRAINT FK_S7SExecution_Workflow FOREIGN KEY (WorkflowID) REFERENCES S7SWorkflow(WorkflowID)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SExecutionStep')
BEGIN
    CREATE TABLE S7SExecutionStep
    (
        ExecutionStepID       BIGINT IDENTITY(1,1) PRIMARY KEY,
        ExecutionID           BIGINT               NOT NULL,
        NodeID                INT                  NOT NULL,
        Status                NVARCHAR(50)         NOT NULL,
        StartedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        CompletedAt           DATETIME2            NULL,
        RetryCount            INT                  NOT NULL DEFAULT (0),
        OutputJson            NVARCHAR(MAX)        NULL,
        ErrorMessage          NVARCHAR(MAX)        NULL,
        CreatedBy             NVARCHAR(100)        NOT NULL,
        CreatedAt             DATETIME2            NOT NULL DEFAULT (SYSUTCDATETIME()),
        UpdatedBy             NVARCHAR(100)        NULL,
        UpdatedAt             DATETIME2            NULL,
        IsDeleted             BIT                  NOT NULL DEFAULT (0),
        CONSTRAINT FK_S7SExecutionStep_Execution FOREIGN KEY (ExecutionID) REFERENCES S7SExecution(ExecutionID),
        CONSTRAINT FK_S7SExecutionStep_Node FOREIGN KEY (NodeID) REFERENCES S7SNode(NodeID)
    );
END;

-- Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SWorkflow_IsDeleted' AND object_id = OBJECT_ID('S7SWorkflow'))
    CREATE INDEX IX_S7SWorkflow_IsDeleted ON S7SWorkflow(IsDeleted, IsActive);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SNode_Workflow' AND object_id = OBJECT_ID('S7SNode'))
    CREATE INDEX IX_S7SNode_Workflow ON S7SNode(WorkflowID) INCLUDE (NodeType);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SExecution_Workflow' AND object_id = OBJECT_ID('S7SExecution'))
    CREATE INDEX IX_S7SExecution_Workflow ON S7SExecution(WorkflowID, Status);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SExecutionStep_Execution' AND object_id = OBJECT_ID('S7SExecutionStep'))
    CREATE INDEX IX_S7SExecutionStep_Execution ON S7SExecutionStep(ExecutionID, NodeID);

GO

-- Sample stored procedure for upserting a workflow definition with its nodes and connections
IF OBJECT_ID('dbo.S7SWorkflowSP', 'P') IS NOT NULL
    DROP PROCEDURE dbo.S7SWorkflowSP;
GO
CREATE PROCEDURE dbo.S7SWorkflowSP
    @WorkflowID        INT OUTPUT,
    @Name              NVARCHAR(200),
    @Description       NVARCHAR(1000),
    @IsActive          BIT,
    @UserName          NVARCHAR(100),
    @NodesJson         NVARCHAR(MAX),
    @ConnectionsJson   NVARCHAR(MAX),
    @TriggersJson      NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    IF @WorkflowID IS NULL OR @WorkflowID = 0
    BEGIN
        INSERT INTO S7SWorkflow (Name, Description, IsActive, CreatedBy, CreatedAt)
        VALUES (@Name, @Description, @IsActive, @UserName, @Now);
        SET @WorkflowID = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE S7SWorkflow
        SET Name = @Name,
            Description = @Description,
            IsActive = @IsActive,
            UpdatedBy = @UserName,
            UpdatedAt = @Now
        WHERE WorkflowID = @WorkflowID;

        UPDATE S7SNode SET IsDeleted = 1 WHERE WorkflowID = @WorkflowID;
        UPDATE S7SNodeConnection SET IsDeleted = 1 WHERE WorkflowID = @WorkflowID;
        UPDATE S7SWorkflowTrigger SET IsDeleted = 1 WHERE WorkflowID = @WorkflowID;
    END;

    INSERT INTO S7SNode (WorkflowID, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, CreatedBy, CreatedAt)
    SELECT @WorkflowID,
           NodeData.Name,
           NodeData.NodeType,
           NodeData.ConfigJson,
           NodeData.PositionX,
           NodeData.PositionY,
           NodeData.CredentialID,
           @UserName,
           @Now
    FROM OPENJSON(@NodesJson)
         WITH (
             Name NVARCHAR(200),
             NodeType NVARCHAR(100),
             ConfigJson NVARCHAR(MAX),
             PositionX DECIMAL(18,4),
             PositionY DECIMAL(18,4),
             CredentialID INT
         ) AS NodeData;

    INSERT INTO S7SNodeConnection (WorkflowID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey, CreatedBy, CreatedAt)
    SELECT @WorkflowID,
           ConnectionData.SourceNodeID,
           ConnectionData.SourceOutputKey,
           ConnectionData.TargetNodeID,
           ConnectionData.TargetInputKey,
           @UserName,
           @Now
    FROM OPENJSON(@ConnectionsJson)
         WITH (
             SourceNodeID INT,
             SourceOutputKey NVARCHAR(100),
             TargetNodeID INT,
             TargetInputKey NVARCHAR(100)
         ) AS ConnectionData;

    INSERT INTO S7SWorkflowTrigger (WorkflowID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, CreatedBy, CreatedAt)
    SELECT @WorkflowID,
           TriggerData.TriggerType,
           TriggerData.TriggerNodeID,
           TriggerData.Secret,
           TriggerData.CronExpression,
           TriggerData.ConfigurationJson,
           @UserName,
           @Now
    FROM OPENJSON(@TriggersJson)
         WITH (
             TriggerType NVARCHAR(50),
             TriggerNodeID INT,
             Secret NVARCHAR(200),
             CronExpression NVARCHAR(100),
             ConfigurationJson NVARCHAR(MAX)
         ) AS TriggerData;
END;
GO

-- Sample stored procedure for listing executions per workflow
IF OBJECT_ID('dbo.S7SExecutionListSP', 'P') IS NOT NULL
    DROP PROCEDURE dbo.S7SExecutionListSP;
GO
CREATE PROCEDURE dbo.S7SExecutionListSP
    @WorkflowID INT,
    @Skip       INT = 0,
    @Take       INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ExecutionID,
           WorkflowID,
           Status,
           TriggerType,
           StartedAt,
           CompletedAt,
           ErrorMessage
    FROM S7SExecution
    WHERE WorkflowID = @WorkflowID
      AND IsDeleted = 0
    ORDER BY ExecutionID DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
END;
GO
