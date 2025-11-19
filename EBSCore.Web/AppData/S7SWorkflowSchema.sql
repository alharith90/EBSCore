-- Workflow Engine schema aligned with ADO-style stored procedures
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'S7SWorkflow')
BEGIN
    CREATE TABLE S7SWorkflow
    (
        WorkflowID            INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID             INT                  NOT NULL,
        UnitID                BIGINT               NULL,
        WorkflowCode          NVARCHAR(50)         NOT NULL,
        Name                  NVARCHAR(200)        NOT NULL,
        Description           NVARCHAR(1000)       NULL,
        Status                NVARCHAR(50)         NULL,
        Priority              NVARCHAR(50)         NULL,
        Frequency             NVARCHAR(50)         NULL,
        Notes                 NVARCHAR(MAX)        NULL,
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
        CompanyID             INT                  NOT NULL,
        Name                  NVARCHAR(200)        NOT NULL,
        CredentialType        NVARCHAR(100)        NOT NULL,
        DataJson              NVARCHAR(MAX)        NOT NULL,
        Notes                 NVARCHAR(MAX)        NULL,
        IsActive              BIT                  NOT NULL DEFAULT (1),
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
        CompanyID             INT                  NOT NULL,
        UnitID                BIGINT               NULL,
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
        SelectedOutputKey     NVARCHAR(100)        NULL,
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
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SWorkflow_Company' AND object_id = OBJECT_ID('S7SWorkflow'))
    CREATE INDEX IX_S7SWorkflow_Company ON S7SWorkflow(CompanyID, IsDeleted, IsActive);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SNode_Workflow' AND object_id = OBJECT_ID('S7SNode'))
    CREATE INDEX IX_S7SNode_Workflow ON S7SNode(WorkflowID) INCLUDE (NodeType);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SExecution_Workflow' AND object_id = OBJECT_ID('S7SExecution'))
    CREATE INDEX IX_S7SExecution_Workflow ON S7SExecution(WorkflowID, Status);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_S7SExecutionStep_Execution' AND object_id = OBJECT_ID('S7SExecutionStep'))
    CREATE INDEX IX_S7SExecutionStep_Execution ON S7SExecutionStep(ExecutionID, NodeID);
GO

CREATE OR ALTER PROCEDURE dbo.S7SWorkflowSP
    @Operation          NVARCHAR(50),
    @UserID             BIGINT = NULL,
    @CompanyID          INT = NULL,
    @UnitID             BIGINT = NULL,
    @WorkflowID         INT = NULL,
    @WorkflowCode       NVARCHAR(50) = NULL,
    @WorkflowName       NVARCHAR(200) = NULL,
    @WorkflowDescription NVARCHAR(1000) = NULL,
    @Status             NVARCHAR(50) = NULL,
    @Priority           NVARCHAR(50) = NULL,
    @Frequency          NVARCHAR(50) = NULL,
    @Notes              NVARCHAR(MAX) = NULL,
    @IsActive           BIT = 1,
    @NodesJson          NVARCHAR(MAX) = NULL,
    @ConnectionsJson    NVARCHAR(MAX) = NULL,
    @TriggersJson       NVARCHAR(MAX) = NULL,
    @PageNumber         INT = 1,
    @PageSize           INT = 50,
    @SearchQuery        NVARCHAR(200) = NULL,
    @SortColumn         NVARCHAR(50) = 'WorkflowID',
    @SortDirection      NVARCHAR(4) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentUserType INT = (SELECT TOP 1 UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID INT = (SELECT TOP 1 CompanyID FROM [User] WHERE UserID = @UserID);
    DECLARE @ActingCompanyID INT = ISNULL(@CompanyID, @CurrentCompanyID);

    IF @Operation IN ('SaveWorkflow', 'DeleteWorkflow') AND ISNULL(@CurrentUserType, 0) <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveWorkflow'
    BEGIN
        DECLARE @Now DATETIME2 = SYSUTCDATETIME();

        IF EXISTS(SELECT 1 FROM S7SWorkflow WHERE WorkflowID = @WorkflowID AND CompanyID = @ActingCompanyID AND IsDeleted = 0)
        BEGIN
            UPDATE S7SWorkflow
            SET WorkflowCode = @WorkflowCode,
                Name = @WorkflowName,
                Description = @WorkflowDescription,
                Status = @Status,
                Priority = @Priority,
                Frequency = @Frequency,
                Notes = @Notes,
                UnitID = @UnitID,
                IsActive = @IsActive,
                UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
                UpdatedAt = @Now
            WHERE WorkflowID = @WorkflowID AND CompanyID = @ActingCompanyID;

            DELETE FROM S7SNodeConnection WHERE WorkflowID = @WorkflowID;
            DELETE FROM S7SWorkflowTrigger WHERE WorkflowID = @WorkflowID;
            DELETE FROM S7SNode WHERE WorkflowID = @WorkflowID;
        END
        ELSE
        BEGIN
            INSERT INTO S7SWorkflow (CompanyID, UnitID, WorkflowCode, Name, Description, Status, Priority, Frequency, Notes, IsActive, CreatedBy, CreatedAt)
            VALUES (@ActingCompanyID, @UnitID, @WorkflowCode, @WorkflowName, @WorkflowDescription, @Status, @Priority, @Frequency, @Notes, @IsActive, CAST(@UserID AS NVARCHAR(100)), @Now);
            SET @WorkflowID = SCOPE_IDENTITY();
        END

        DECLARE @NodeMap TABLE(NodeKey NVARCHAR(200), NodeID INT);
        DECLARE @NodePayload TABLE
        (
            NodeKey NVARCHAR(200),
            Name NVARCHAR(200),
            NodeType NVARCHAR(100),
            ConfigJson NVARCHAR(MAX),
            PositionX DECIMAL(18,4),
            PositionY DECIMAL(18,4),
            CredentialID INT,
            RetryCount INT
        );
        IF ISNULL(@NodesJson, '') <> ''
        BEGIN
            INSERT INTO @NodePayload(NodeKey, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount)
            SELECT NodeKey,
                   Name,
                   NodeType,
                   ConfigJson,
                   PositionX,
                   PositionY,
                   CredentialID,
                   RetryCount
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

            INSERT INTO S7SNode (WorkflowID, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount, CreatedBy, CreatedAt)
            OUTPUT payload.NodeKey, inserted.NodeID INTO @NodeMap(NodeKey, NodeID)
            SELECT @WorkflowID,
                   payload.Name,
                   payload.NodeType,
                   payload.ConfigJson,
                   payload.PositionX,
                   payload.PositionY,
                   payload.CredentialID,
                   ISNULL(payload.RetryCount, 0),
                   CAST(@UserID AS NVARCHAR(100)),
                   @Now
            FROM @NodePayload payload;
        END

        IF ISNULL(@ConnectionsJson, '') <> ''
        BEGIN
            INSERT INTO S7SNodeConnection (WorkflowID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey, CreatedBy, CreatedAt)
            SELECT @WorkflowID,
                   SourceMap.NodeID,
                   ConnectionPayload.SourceOutputKey,
                   TargetMap.NodeID,
                   ConnectionPayload.TargetInputKey,
                   CAST(@UserID AS NVARCHAR(100)),
                   @Now
            FROM OPENJSON(@ConnectionsJson)
                 WITH (
                    SourceNodeKey NVARCHAR(200) '$.SourceNodeKey',
                    TargetNodeKey NVARCHAR(200) '$.TargetNodeKey',
                    SourceOutputKey NVARCHAR(100) '$.SourceOutputKey',
                    TargetInputKey NVARCHAR(100) '$.TargetInputKey'
                 ) AS ConnectionPayload
                 INNER JOIN @NodeMap SourceMap ON SourceMap.NodeKey = ConnectionPayload.SourceNodeKey
                 INNER JOIN @NodeMap TargetMap ON TargetMap.NodeKey = ConnectionPayload.TargetNodeKey;
        END

        IF ISNULL(@TriggersJson, '') <> ''
        BEGIN
            INSERT INTO S7SWorkflowTrigger (WorkflowID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, CreatedBy, CreatedAt)
            SELECT @WorkflowID,
                   TriggerPayload.TriggerType,
                   TriggerPayload.TriggerNodeID,
                   TriggerPayload.Secret,
                   TriggerPayload.CronExpression,
                   TriggerPayload.ConfigurationJson,
                   CAST(@UserID AS NVARCHAR(100)),
                   @Now
            FROM OPENJSON(@TriggersJson)
                 WITH (
                    TriggerType NVARCHAR(50) '$.TriggerType',
                    TriggerNodeID INT '$.TriggerNodeID',
                    Secret NVARCHAR(200) '$.Secret',
                    CronExpression NVARCHAR(100) '$.CronExpression',
                    ConfigurationJson NVARCHAR(MAX) '$.ConfigurationJson'
                 ) AS TriggerPayload;
        END

        SELECT WorkflowID FROM S7SWorkflow WHERE WorkflowID = @WorkflowID;
        RETURN;
    END
    ELSE IF @Operation = 'rtvWorkflow'
    BEGIN
        SELECT * FROM S7SWorkflow WHERE WorkflowID = @WorkflowID AND CompanyID = @ActingCompanyID AND IsDeleted = 0;
        SELECT * FROM S7SNode WHERE WorkflowID = @WorkflowID AND IsDeleted = 0 ORDER BY NodeID;
        SELECT * FROM S7SNodeConnection WHERE WorkflowID = @WorkflowID AND IsDeleted = 0 ORDER BY NodeConnectionID;
        SELECT * FROM S7SWorkflowTrigger WHERE WorkflowID = @WorkflowID AND IsDeleted = 0 ORDER BY WorkflowTriggerID;
        RETURN;
    END
    ELSE IF @Operation = 'rtvWorkflows'
    BEGIN
        WITH OrderedWorkflows AS
        (
            SELECT ROW_NUMBER() OVER (ORDER BY
                    CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'ASC' THEN Name END ASC,
                    CASE WHEN @SortColumn = 'Name' AND @SortDirection = 'DESC' THEN Name END DESC,
                    CASE WHEN @SortColumn = 'WorkflowCode' AND @SortDirection = 'ASC' THEN WorkflowCode END ASC,
                    CASE WHEN @SortColumn = 'WorkflowCode' AND @SortDirection = 'DESC' THEN WorkflowCode END DESC,
                    WorkflowID DESC
                ) AS RowNum,
                *
            FROM S7SWorkflow
            WHERE CompanyID = @ActingCompanyID
              AND IsDeleted = 0
              AND (@SearchQuery IS NULL OR Name LIKE '%' + @SearchQuery + '%' OR WorkflowCode LIKE '%' + @SearchQuery + '%')
        )
        SELECT * FROM OrderedWorkflows
        WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
        ORDER BY RowNum;

        SELECT COUNT(1) AS TotalCount
        FROM S7SWorkflow
        WHERE CompanyID = @ActingCompanyID AND IsDeleted = 0
          AND (@SearchQuery IS NULL OR Name LIKE '%' + @SearchQuery + '%' OR WorkflowCode LIKE '%' + @SearchQuery + '%');
        RETURN;
    END
    ELSE IF @Operation = 'DeleteWorkflow'
    BEGIN
        UPDATE S7SWorkflow
        SET IsDeleted = 1,
            UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
            UpdatedAt = SYSUTCDATETIME()
        WHERE WorkflowID = @WorkflowID AND CompanyID = @ActingCompanyID;
        RETURN;
    END
END;
GO

CREATE OR ALTER PROCEDURE dbo.S7SCredentialSP
    @Operation          NVARCHAR(50),
    @UserID             BIGINT = NULL,
    @CompanyID          INT = NULL,
    @CredentialID       INT = NULL,
    @CredentialName     NVARCHAR(200) = NULL,
    @CredentialType     NVARCHAR(100) = NULL,
    @DataJson           NVARCHAR(MAX) = NULL,
    @Notes              NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentCompanyID INT = (SELECT TOP 1 CompanyID FROM [User] WHERE UserID = @UserID);
    DECLARE @ActingCompanyID INT = ISNULL(@CompanyID, @CurrentCompanyID);

    IF @Operation = 'SaveCredential'
    BEGIN
        DECLARE @Now DATETIME2 = SYSUTCDATETIME();
        IF EXISTS(SELECT 1 FROM S7SCredential WHERE CredentialID = @CredentialID AND CompanyID = @ActingCompanyID AND IsDeleted = 0)
        BEGIN
            UPDATE S7SCredential
            SET Name = @CredentialName,
                CredentialType = @CredentialType,
                DataJson = @DataJson,
                Notes = @Notes,
                UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
                UpdatedAt = @Now
            WHERE CredentialID = @CredentialID AND CompanyID = @ActingCompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO S7SCredential (CompanyID, Name, CredentialType, DataJson, Notes, CreatedBy, CreatedAt)
            VALUES (@ActingCompanyID, @CredentialName, @CredentialType, @DataJson, @Notes, CAST(@UserID AS NVARCHAR(100)), @Now);
            SET @CredentialID = SCOPE_IDENTITY();
        END
        SELECT @CredentialID AS CredentialID;
        RETURN;
    END
    ELSE IF @Operation = 'rtvCredentials'
    BEGIN
        SELECT CredentialID, Name, CredentialType, DataJson, Notes, IsActive
        FROM S7SCredential
        WHERE CompanyID = @ActingCompanyID AND IsDeleted = 0
        ORDER BY CredentialID DESC;
        RETURN;
    END
    ELSE IF @Operation = 'DeleteCredential'
    BEGIN
        UPDATE S7SCredential
        SET IsDeleted = 1,
            UpdatedBy = CAST(@UserID AS NVARCHAR(100)),
            UpdatedAt = SYSUTCDATETIME()
        WHERE CredentialID = @CredentialID AND CompanyID = @ActingCompanyID;
        RETURN;
    END
END;
GO

CREATE OR ALTER PROCEDURE dbo.S7SWorkflowExecutionSP
    @Operation          NVARCHAR(50),
    @UserID             BIGINT = NULL,
    @WorkflowID         INT = NULL,
    @ExecutionID        BIGINT = NULL,
    @PayloadJson        NVARCHAR(MAX) = NULL,
    @WebhookSecret      NVARCHAR(200) = NULL,
    @RequestJson        NVARCHAR(MAX) = NULL,
    @Status             NVARCHAR(50) = NULL,
    @ErrorMessage       NVARCHAR(MAX) = NULL,
    @NodeID             INT = NULL,
    @OutputJson         NVARCHAR(MAX) = NULL,
    @OutputKey          NVARCHAR(100) = NULL,
    @PageNumber         INT = 1,
    @PageSize           INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();
    DECLARE @UserName NVARCHAR(100) = (SELECT TOP 1 UserName FROM [User] WHERE UserID = @UserID);

    IF @Operation = 'StartManualExecution'
    BEGIN
        DECLARE @WFCompany INT = (SELECT CompanyID FROM S7SWorkflow WHERE WorkflowID = @WorkflowID AND IsDeleted = 0);
        DECLARE @WFUnit BIGINT = (SELECT UnitID FROM S7SWorkflow WHERE WorkflowID = @WorkflowID);
        IF @WFCompany IS NULL
            THROW 51001, 'Workflow not found', 1;

        INSERT INTO S7SExecution (WorkflowID, CompanyID, UnitID, Status, TriggerType, TriggerDataJson, CreatedBy, CreatedAt)
        VALUES (@WorkflowID, @WFCompany, @WFUnit, 'Pending', 'Manual', @PayloadJson, ISNULL(@UserName, 'manual'), @Now);
        SELECT SCOPE_IDENTITY() AS ExecutionID;
        RETURN;
    END
    ELSE IF @Operation = 'StartWebhookExecution'
    BEGIN
        DECLARE @TriggerWorkflow INT = (SELECT WorkflowID FROM S7SWorkflowTrigger WHERE WorkflowID = @WorkflowID AND Secret = @WebhookSecret AND IsDeleted = 0);
        IF @TriggerWorkflow IS NULL
            THROW 51002, 'Invalid webhook secret', 1;

        DECLARE @Company INT = (SELECT CompanyID FROM S7SWorkflow WHERE WorkflowID = @WorkflowID);
        DECLARE @Unit BIGINT = (SELECT UnitID FROM S7SWorkflow WHERE WorkflowID = @WorkflowID);

        INSERT INTO S7SExecution (WorkflowID, CompanyID, UnitID, Status, TriggerType, TriggerDataJson, WebhookRequestJson, CreatedBy, CreatedAt)
        VALUES (@WorkflowID, @Company, @Unit, 'Pending', 'Webhook', NULL, @RequestJson, 'webhook', @Now);
        SELECT SCOPE_IDENTITY() AS ExecutionID;
        RETURN;
    END
    ELSE IF @Operation = 'rtvExecutions'
    BEGIN
        SELECT *
        FROM (
            SELECT ROW_NUMBER() OVER (ORDER BY ExecutionID DESC) AS RowNum, *
            FROM S7SExecution
            WHERE WorkflowID = @WorkflowID AND IsDeleted = 0
        ) e
        WHERE RowNum BETWEEN ((@PageNumber - 1) * @PageSize) + 1 AND (@PageNumber * @PageSize)
        ORDER BY RowNum;

        SELECT COUNT(1) AS TotalCount
        FROM S7SExecution WHERE WorkflowID = @WorkflowID AND IsDeleted = 0;
        RETURN;
    END
    ELSE IF @Operation = 'rtvExecution'
    BEGIN
        SELECT * FROM S7SExecution WHERE ExecutionID = @ExecutionID;
        SELECT * FROM S7SExecutionStep WHERE ExecutionID = @ExecutionID ORDER BY ExecutionStepID;
        RETURN;
    END
    ELSE IF @Operation = 'SaveExecutionStep'
    BEGIN
        INSERT INTO S7SExecutionStep (ExecutionID, NodeID, Status, SelectedOutputKey, OutputJson, ErrorMessage, CreatedBy)
        VALUES (@ExecutionID, @NodeID, @Status, @OutputKey, @OutputJson, @ErrorMessage, ISNULL(@UserName, 'workflow-daemon'));
        RETURN;
    END
    ELSE IF @Operation = 'MarkExecutionStatus'
    BEGIN
        UPDATE S7SExecution
        SET Status = @Status,
            ErrorMessage = @ErrorMessage,
            CompletedAt = CASE WHEN @Status IN ('Succeeded','Failed','Canceled') THEN @Now ELSE CompletedAt END,
            UpdatedAt = @Now,
            UpdatedBy = ISNULL(@UserName, 'workflow-daemon')
        WHERE ExecutionID = @ExecutionID;
        RETURN;
    END
    ELSE IF @Operation = 'DequeueExecution'
    BEGIN
        DECLARE @Execution TABLE (ExecutionID BIGINT, WorkflowID INT);

        UPDATE S7SExecution
        SET Status = 'Running',
            StartedAt = CASE WHEN StartedAt IS NULL THEN @Now ELSE StartedAt END,
            UpdatedAt = @Now,
            UpdatedBy = 'workflow-daemon'
        OUTPUT inserted.ExecutionID, inserted.WorkflowID INTO @Execution
        WHERE ExecutionID = (
            SELECT TOP 1 ExecutionID
            FROM S7SExecution WITH (READPAST, UPDLOCK)
            WHERE Status = 'Pending' AND IsDeleted = 0
            ORDER BY ExecutionID
        );

        IF EXISTS(SELECT 1 FROM @Execution)
        BEGIN
            SELECT exec.ExecutionID,
                   exec.WorkflowID,
                   wf.Name AS WorkflowName,
                   exec.TriggerType,
                   exec.TriggerDataJson,
                   exec.WebhookRequestJson
            FROM @Execution e
            INNER JOIN S7SExecution exec ON exec.ExecutionID = e.ExecutionID
            INNER JOIN S7SWorkflow wf ON wf.WorkflowID = e.WorkflowID;

            SELECT n.*
            FROM S7SNode n
            INNER JOIN @Execution e ON e.WorkflowID = n.WorkflowID
            WHERE n.IsDeleted = 0
            ORDER BY n.NodeID;

            SELECT c.*
            FROM S7SNodeConnection c
            INNER JOIN @Execution e ON e.WorkflowID = c.WorkflowID
            WHERE c.IsDeleted = 0
            ORDER BY c.NodeConnectionID;

            SELECT t.*
            FROM S7SWorkflowTrigger t
            INNER JOIN @Execution e ON e.WorkflowID = t.WorkflowID
            WHERE t.IsDeleted = 0;
        END
        ELSE
        BEGIN
            SELECT CAST(NULL AS BIGINT) AS ExecutionID WHERE 1 = 0;
            SELECT CAST(NULL AS INT) AS NodeID WHERE 1 = 0;
            SELECT CAST(NULL AS INT) AS NodeConnectionID WHERE 1 = 0;
            SELECT CAST(NULL AS INT) AS WorkflowTriggerID WHERE 1 = 0;
        END
        RETURN;
    END
END;
GO
