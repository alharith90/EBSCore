USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
  Enterprise Operations and Service Delivery schema extensions.
  This script keeps master data isolated from transactional data and analytics projections.
*/

IF OBJECT_ID('dbo.OperationsServiceCategory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsServiceCategory
    (
        ServiceCategoryID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        CategoryCode NVARCHAR(50) NOT NULL,
        CategoryName NVARCHAR(200) NOT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CreatedBy BIGINT NULL,
        UpdatedAt DATETIME2 NULL,
        UpdatedBy BIGINT NULL
    );
END
GO

IF OBJECT_ID('dbo.OperationsAssignmentGroup', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsAssignmentGroup
    (
        AssignmentGroupID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        GroupCode NVARCHAR(50) NOT NULL,
        GroupName NVARCHAR(200) NOT NULL,
        OrganisationUnitID INT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.OperationsSLAProfile', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsSLAProfile
    (
        SLAProfileID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ProfileCode NVARCHAR(50) NOT NULL,
        ProfileName NVARCHAR(200) NOT NULL,
        PriorityCode NVARCHAR(20) NOT NULL,
        FirstResponseMinutes INT NOT NULL,
        ResolutionMinutes INT NOT NULL,
        CalendarCode NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.OperationsServiceRequest', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsServiceRequest
    (
        ServiceRequestID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        RequestNumber NVARCHAR(30) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        RequesterUserID BIGINT NULL,
        RequestChannelCode NVARCHAR(30) NULL,
        ServiceCategoryID INT NULL,
        ServiceTypeID INT NULL,
        PriorityCode NVARCHAR(20) NULL,
        ImpactCode NVARCHAR(20) NULL,
        UrgencyCode NVARCHAR(20) NULL,
        StatusCode NVARCHAR(30) NOT NULL,
        AssignmentGroupID INT NULL,
        AssignedUserID BIGINT NULL,
        OrganisationUnitID INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        DueAt DATETIME2 NULL,
        FirstRespondedAt DATETIME2 NULL,
        ResolvedAt DATETIME2 NULL,
        ClosedAt DATETIME2 NULL,
        ResolutionCode NVARCHAR(50) NULL,
        ClosureReasonCode NVARCHAR(50) NULL,
        IsReopened BIT NOT NULL DEFAULT(0)
    );
END
GO

IF OBJECT_ID('dbo.OperationsIncident', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsIncident
    (
        IncidentID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        IncidentNumber NVARCHAR(30) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        SeverityCode NVARCHAR(20) NULL,
        ImpactCode NVARCHAR(20) NULL,
        UrgencyCode NVARCHAR(20) NULL,
        PriorityCode NVARCHAR(20) NULL,
        StatusCode NVARCHAR(30) NOT NULL,
        ServiceCategoryID INT NULL,
        AssignmentGroupID INT NULL,
        AssignedUserID BIGINT NULL,
        OrganisationUnitID INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        FirstRespondedAt DATETIME2 NULL,
        ResolvedAt DATETIME2 NULL,
        ClosedAt DATETIME2 NULL,
        ResponseTargetAt DATETIME2 NULL,
        ResolutionTargetAt DATETIME2 NULL,
        RootCauseNotes NVARCHAR(MAX) NULL
    );
END
GO

IF OBJECT_ID('dbo.OperationsWorkOrder', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsWorkOrder
    (
        WorkOrderID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        WorkOrderNumber NVARCHAR(30) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        StatusCode NVARCHAR(30) NOT NULL,
        AssignmentGroupID INT NULL,
        AssignedUserID BIGINT NULL,
        OrganisationUnitID INT NULL,
        DueAt DATETIME2 NULL,
        CompletedAt DATETIME2 NULL,
        ProgressPercent DECIMAL(5,2) NULL,
        EvidenceUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.OperationsSLAEvent', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsSLAEvent
    (
        SLAEventID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        EntityType NVARCHAR(30) NOT NULL,
        EntityID BIGINT NOT NULL,
        SLAProfileID INT NULL,
        EventType NVARCHAR(30) NOT NULL,
        EventAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        TargetAt DATETIME2 NULL,
        IsBreached BIT NOT NULL DEFAULT(0),
        Notes NVARCHAR(MAX) NULL
    );
END
GO

IF OBJECT_ID('dbo.OperationsActivityLog', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsActivityLog
    (
        ActivityLogID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        EntityType NVARCHAR(30) NOT NULL,
        EntityID BIGINT NOT NULL,
        ActivityType NVARCHAR(50) NOT NULL,
        ActivityNotes NVARCHAR(MAX) NULL,
        PreviousValue NVARCHAR(MAX) NULL,
        NewValue NVARCHAR(MAX) NULL,
        PerformedBy BIGINT NULL,
        PerformedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.OperationsEscalationRule', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsEscalationRule
    (
        EscalationRuleID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        RuleCode NVARCHAR(50) NOT NULL,
        RuleName NVARCHAR(200) NOT NULL,
        TriggerType NVARCHAR(50) NOT NULL,
        PriorityCode NVARCHAR(20) NULL,
        ThresholdMinutes INT NOT NULL,
        EscalateToGroupID INT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.OperationsServiceDashboardConfig', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationsServiceDashboardConfig
    (
        DashboardConfigID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        DashboardCode NVARCHAR(50) NOT NULL,
        DashboardName NVARCHAR(200) NOT NULL,
        ConfigJson NVARCHAR(MAX) NOT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.vwOperationsQueueBacklog', 'V') IS NOT NULL
    DROP VIEW dbo.vwOperationsQueueBacklog
GO

CREATE VIEW dbo.vwOperationsQueueBacklog AS
SELECT
    sr.CompanyID,
    ISNULL(sr.AssignmentGroupID, 0) AS AssignmentGroupID,
    ISNULL(g.GroupName, 'Unassigned') AS QueueName,
    COUNT(1) AS OpenItems,
    SUM(CASE WHEN sr.DueAt < SYSUTCDATETIME() THEN 1 ELSE 0 END) AS OverdueItems,
    SUM(CASE WHEN EXISTS (
        SELECT 1 FROM dbo.OperationsSLAEvent ev
        WHERE ev.EntityType = 'ServiceRequest'
          AND ev.EntityID = sr.ServiceRequestID
          AND ev.IsBreached = 1
    ) THEN 1 ELSE 0 END) AS BreachedItems,
    CAST(AVG(DATEDIFF(MINUTE, sr.CreatedAt, SYSUTCDATETIME()) / 60.0) AS DECIMAL(18,2)) AS AvgAgeHours
FROM dbo.OperationsServiceRequest sr
LEFT JOIN dbo.OperationsAssignmentGroup g
    ON g.AssignmentGroupID = sr.AssignmentGroupID
WHERE sr.StatusCode NOT IN ('Closed', 'Cancelled')
GROUP BY sr.CompanyID, ISNULL(sr.AssignmentGroupID, 0), ISNULL(g.GroupName, 'Unassigned')
GO

CREATE OR ALTER PROCEDURE dbo.OperationsServiceDeliverySP
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @OrganisationUnitID INT = NULL,
    @IncludeChildren BIT = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @AssignmentGroupID INT = NULL,
    @AssignedUserID BIGINT = NULL,
    @PriorityCode NVARCHAR(20) = NULL,
    @StatusCode NVARCHAR(30) = NULL,
    @SerializedObject NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'rtvServiceRequests'
    BEGIN
        SELECT *
        FROM dbo.OperationsServiceRequest
        WHERE CompanyID = @CompanyID
        ORDER BY CreatedAt DESC;
        RETURN;
    END

    IF @Operation = 'SaveServiceRequest'
    BEGIN
        DECLARE @RequestNumber NVARCHAR(30) = JSON_VALUE(@SerializedObject, '$.RequestNumber');
        DECLARE @Title NVARCHAR(250) = JSON_VALUE(@SerializedObject, '$.Title');
        DECLARE @Description NVARCHAR(MAX) = JSON_VALUE(@SerializedObject, '$.Description');
        DECLARE @Priority NVARCHAR(20) = JSON_VALUE(@SerializedObject, '$.PriorityCode');
        DECLARE @Status NVARCHAR(30) = COALESCE(JSON_VALUE(@SerializedObject, '$.StatusCode'), 'New');
        DECLARE @DueAt DATETIME2 = TRY_CAST(JSON_VALUE(@SerializedObject, '$.DueAt') AS DATETIME2);

        INSERT INTO dbo.OperationsServiceRequest
        (
            CompanyID, RequestNumber, Title, [Description], PriorityCode, StatusCode, DueAt, CreatedAt
        )
        VALUES
        (
            @CompanyID,
            COALESCE(@RequestNumber, CONCAT('SR-', FORMAT(SYSUTCDATETIME(), 'yyyyMMddHHmmss'))),
            @Title,
            @Description,
            @Priority,
            @Status,
            @DueAt,
            SYSUTCDATETIME()
        );

        SELECT TOP 1 *
        FROM dbo.OperationsServiceRequest
        WHERE CompanyID = @CompanyID
        ORDER BY ServiceRequestID DESC;

        RETURN;
    END

    IF @Operation = 'rtvOperationsIncidents'
    BEGIN
        SELECT i.*,
               CASE
                 WHEN i.ResolutionTargetAt IS NOT NULL AND i.ResolutionTargetAt < SYSUTCDATETIME() AND i.StatusCode NOT IN ('Resolved', 'Closed') THEN 'Breached'
                 WHEN i.ResolutionTargetAt IS NOT NULL AND DATEDIFF(MINUTE, SYSUTCDATETIME(), i.ResolutionTargetAt) <= 60 THEN 'AtRisk'
                 ELSE 'Met'
               END AS SLAStatus
        FROM dbo.OperationsIncident i
        WHERE i.CompanyID = @CompanyID
        ORDER BY i.CreatedAt DESC;
        RETURN;
    END

    IF @Operation = 'rtvOperationsWorkOrders'
    BEGIN
        SELECT *
        FROM dbo.OperationsWorkOrder
        WHERE CompanyID = @CompanyID
        ORDER BY CreatedAt DESC;
        RETURN;
    END

    IF @Operation = 'rtvOperationsDashboard'
    BEGIN
        DECLARE @FromDate DATETIME2 = COALESCE(CAST(@StartDate AS DATETIME2), DATEADD(DAY, -30, SYSUTCDATETIME()));
        DECLARE @ToDate DATETIME2 = COALESCE(DATEADD(DAY, 1, CAST(@EndDate AS DATETIME2)), DATEADD(DAY, 1, SYSUTCDATETIME()));

        ;WITH RequestsInScope AS
        (
            SELECT *
            FROM dbo.OperationsServiceRequest sr
            WHERE sr.CompanyID = @CompanyID
              AND sr.CreatedAt >= @FromDate
              AND sr.CreatedAt < @ToDate
              AND (@OrganisationUnitID IS NULL OR sr.OrganisationUnitID = @OrganisationUnitID)
              AND (@AssignmentGroupID IS NULL OR sr.AssignmentGroupID = @AssignmentGroupID)
              AND (@AssignedUserID IS NULL OR sr.AssignedUserID = @AssignedUserID)
              AND (@PriorityCode IS NULL OR sr.PriorityCode = @PriorityCode)
              AND (@StatusCode IS NULL OR sr.StatusCode = @StatusCode)
        ),
        IncidentsInScope AS
        (
            SELECT *
            FROM dbo.OperationsIncident i
            WHERE i.CompanyID = @CompanyID
              AND i.CreatedAt >= @FromDate
              AND i.CreatedAt < @ToDate
              AND (@OrganisationUnitID IS NULL OR i.OrganisationUnitID = @OrganisationUnitID)
        ),
        WorkOrdersInScope AS
        (
            SELECT *
            FROM dbo.OperationsWorkOrder w
            WHERE w.CompanyID = @CompanyID
              AND w.CreatedAt >= @FromDate
              AND w.CreatedAt < @ToDate
              AND (@OrganisationUnitID IS NULL OR w.OrganisationUnitID = @OrganisationUnitID)
        )
        SELECT 'OpenItems' AS Code, 'Open Items' AS Label,
               CAST((SELECT COUNT(1) FROM RequestsInScope WHERE StatusCode NOT IN ('Closed','Cancelled'))
                    + (SELECT COUNT(1) FROM IncidentsInScope WHERE StatusCode NOT IN ('Closed','Resolved'))
                    + (SELECT COUNT(1) FROM WorkOrdersInScope WHERE StatusCode NOT IN ('Closed','Completed')) AS DECIMAL(18,2)) AS Value,
               'count' AS Unit,
               NULL AS PreviousValue,
               'stable' AS Trend
        UNION ALL
        SELECT 'OverdueItems', 'Overdue Items',
               CAST((SELECT COUNT(1) FROM RequestsInScope WHERE DueAt < SYSUTCDATETIME() AND StatusCode NOT IN ('Closed','Cancelled'))
                    + (SELECT COUNT(1) FROM WorkOrdersInScope WHERE DueAt < SYSUTCDATETIME() AND StatusCode NOT IN ('Closed','Completed')) AS DECIMAL(18,2)),
               'count', NULL, 'unfavorable'
        UNION ALL
        SELECT 'SLACompliance', 'SLA Compliance',
               CAST(CASE WHEN (SELECT COUNT(1) FROM dbo.OperationsSLAEvent ev WHERE ev.CompanyID = @CompanyID AND ev.EventAt >= @FromDate AND ev.EventAt < @ToDate) = 0
                         THEN 100
                         ELSE 100.0 * (SELECT COUNT(1) FROM dbo.OperationsSLAEvent ev WHERE ev.CompanyID = @CompanyID AND ev.EventAt >= @FromDate AND ev.EventAt < @ToDate AND ev.IsBreached = 0)
                                   / NULLIF((SELECT COUNT(1) FROM dbo.OperationsSLAEvent ev WHERE ev.CompanyID = @CompanyID AND ev.EventAt >= @FromDate AND ev.EventAt < @ToDate),0)
                    END AS DECIMAL(18,2)),
               '%', NULL, 'favorable'
        UNION ALL
        SELECT 'AvgResolutionHours', 'Avg Resolution Time',
               CAST(ISNULL((SELECT AVG(DATEDIFF(MINUTE, CreatedAt, ISNULL(ResolvedAt, SYSUTCDATETIME())) / 60.0) FROM RequestsInScope), 0) AS DECIMAL(18,2)),
               'hours', NULL, 'stable';

        SELECT TOP 15 AssignmentGroupID, QueueName, OpenItems, OverdueItems, BreachedItems, AvgAgeHours
        FROM dbo.vwOperationsQueueBacklog
        WHERE CompanyID = @CompanyID
        ORDER BY OpenItems DESC;

        SELECT TOP 10 sr.*, CASE
            WHEN EXISTS (SELECT 1 FROM dbo.OperationsSLAEvent ev WHERE ev.EntityType = 'ServiceRequest' AND ev.EntityID = sr.ServiceRequestID AND ev.IsBreached = 1) THEN 'Breached'
            WHEN sr.DueAt IS NOT NULL AND DATEDIFF(MINUTE, SYSUTCDATETIME(), sr.DueAt) <= 60 THEN 'AtRisk'
            ELSE 'Met'
        END AS SLAStatus
        FROM RequestsInScope sr
        WHERE sr.StatusCode NOT IN ('Closed', 'Cancelled')
        ORDER BY sr.CreatedAt;

        SELECT TOP 10 i.*, CASE
            WHEN i.ResolutionTargetAt IS NOT NULL AND i.ResolutionTargetAt < SYSUTCDATETIME() AND i.StatusCode NOT IN ('Closed','Resolved') THEN 'Breached'
            WHEN i.ResolutionTargetAt IS NOT NULL AND DATEDIFF(MINUTE, SYSUTCDATETIME(), i.ResolutionTargetAt) <= 60 THEN 'AtRisk'
            ELSE 'Met'
        END AS SLAStatus
        FROM IncidentsInScope i
        WHERE i.SeverityCode IN ('Critical', 'Sev1')
          AND i.StatusCode NOT IN ('Closed', 'Resolved')
        ORDER BY i.CreatedAt;

        SELECT TOP 10 *
        FROM WorkOrdersInScope
        WHERE DueAt < SYSUTCDATETIME()
          AND StatusCode NOT IN ('Closed', 'Completed')
        ORDER BY DueAt;

        RETURN;
    END
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.OperationsServiceCategory WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.OperationsServiceCategory (CompanyID, CategoryCode, CategoryName, CreatedBy)
    VALUES
    (1, 'IT', 'IT Services', 1),
    (1, 'FAC', 'Facilities', 1),
    (1, 'HR', 'HR Shared Services', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.OperationsAssignmentGroup WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.OperationsAssignmentGroup (CompanyID, GroupCode, GroupName)
    VALUES
    (1, 'NOC', 'Network Operations Center'),
    (1, 'SD', 'Service Desk'),
    (1, 'FACOPS', 'Facilities Operations');
END
GO
