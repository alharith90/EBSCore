USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
  Supply Chain and Inventory Performance module
  - Master data separated from transactions and analytics snapshots
  - Dashboard and rollups powered by live transaction/balance data
  - Future extensibility for lot/serial, transport, planning, and ERP integrations
*/

IF OBJECT_ID('dbo.SCItemCategory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCItemCategory
    (
        ItemCategoryID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        CategoryCode NVARCHAR(50) NOT NULL,
        CategoryName NVARCHAR(200) NOT NULL,
        ParentCategoryID INT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedBy BIGINT NULL,
        ModifiedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.SCUnitOfMeasure', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCUnitOfMeasure
    (
        UnitOfMeasureID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        UOMCode NVARCHAR(20) NOT NULL,
        UOMName NVARCHAR(100) NOT NULL,
        UOMType NVARCHAR(30) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCWarehouse', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCWarehouse
    (
        WarehouseID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        WarehouseCode NVARCHAR(50) NOT NULL,
        WarehouseName NVARCHAR(200) NOT NULL,
        OrganisationUnitID INT NULL,
        WarehouseTypeCode NVARCHAR(30) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.SCStorageLocation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCStorageLocation
    (
        StorageLocationID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        WarehouseID INT NOT NULL,
        LocationCode NVARCHAR(50) NOT NULL,
        LocationName NVARCHAR(200) NOT NULL,
        LocationTypeCode NVARCHAR(30) NULL,
        IsPickLocation BIT NOT NULL DEFAULT(0),
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCLeadTimeProfile', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCLeadTimeProfile
    (
        LeadTimeProfileID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ProfileCode NVARCHAR(50) NOT NULL,
        ProfileName NVARCHAR(200) NOT NULL,
        ProcurementLeadDays INT NULL,
        InternalTransferLeadDays INT NULL,
        ReplenishmentCycleDays INT NULL,
        TargetServiceLevelPct DECIMAL(5,2) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCStockStatus', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCStockStatus
    (
        StockStatusID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        StockStatusCode NVARCHAR(30) NOT NULL,
        StockStatusName NVARCHAR(100) NOT NULL,
        IsAvailable BIT NOT NULL DEFAULT(1),
        IsSaleable BIT NOT NULL DEFAULT(1),
        IsActive BIT NOT NULL DEFAULT(1)
    );
END
GO

IF OBJECT_ID('dbo.SCItem', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCItem
    (
        ItemID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ItemCode NVARCHAR(60) NOT NULL,
        ItemName NVARCHAR(250) NOT NULL,
        ItemDescription NVARCHAR(500) NULL,
        ItemCategoryID INT NULL,
        UnitOfMeasureID INT NULL,
        StockTypeCode NVARCHAR(30) NULL,
        ItemStatusCode NVARCHAR(30) NOT NULL DEFAULT('ACTIVE'),
        ReplenishmentMethodCode NVARCHAR(30) NULL,
        PreferredSupplierID INT NULL,
        LeadTimeProfileID INT NULL,
        SafetyStockQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        MinStockQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        MaxStockQty DECIMAL(18,4) NULL,
        ReorderPointQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        ReorderQty DECIMAL(18,4) NULL,
        ABCClassCode NVARCHAR(5) NULL,
        IsNegativeStockAllowed BIT NOT NULL DEFAULT(0),
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedBy BIGINT NULL,
        ModifiedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.SCReplenishmentRule', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCReplenishmentRule
    (
        ReplenishmentRuleID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NOT NULL,
        StorageLocationID INT NULL,
        RuleCode NVARCHAR(50) NOT NULL,
        RuleName NVARCHAR(200) NOT NULL,
        ReplenishmentMethodCode NVARCHAR(30) NOT NULL,
        MinStockQty DECIMAL(18,4) NOT NULL,
        MaxStockQty DECIMAL(18,4) NULL,
        ReorderPointQty DECIMAL(18,4) NOT NULL,
        ReorderQty DECIMAL(18,4) NULL,
        SafetyStockQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        ReviewCycleDays INT NULL,
        PreferredSupplierID INT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedBy BIGINT NULL,
        ModifiedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.SCInventoryBalance', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCInventoryBalance
    (
        InventoryBalanceID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NOT NULL,
        StorageLocationID INT NULL,
        StockStatusCode NVARCHAR(30) NOT NULL DEFAULT('AVAILABLE'),
        OnHandQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        ReservedQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        InTransitQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        CommittedQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        AverageUnitCost DECIMAL(18,4) NOT NULL DEFAULT(0),
        LastMovementAt DATETIME2 NULL,
        UpdatedBy BIGINT NULL,
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCInventoryTransaction', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCInventoryTransaction
    (
        InventoryTransactionID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        TransactionDate DATETIME2 NOT NULL,
        TransactionTypeCode NVARCHAR(30) NOT NULL,
        MovementTypeCode NVARCHAR(30) NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NULL,
        StorageLocationID INT NULL,
        ToWarehouseID INT NULL,
        ToStorageLocationID INT NULL,
        Quantity DECIMAL(18,4) NOT NULL,
        UnitCost DECIMAL(18,4) NOT NULL DEFAULT(0),
        TotalCost AS (Quantity * UnitCost) PERSISTED,
        StockStatusCode NVARCHAR(30) NOT NULL DEFAULT('AVAILABLE'),
        ReasonCode NVARCHAR(50) NULL,
        RelatedDocType NVARCHAR(50) NULL,
        RelatedDocNo NVARCHAR(100) NULL,
        RelatedDocID BIGINT NULL,
        OrganisationUnitID INT NULL,
        ApprovalStatusCode NVARCHAR(30) NOT NULL DEFAULT('APPROVED'),
        WorkflowStatusCode NVARCHAR(30) NOT NULL DEFAULT('APPROVED'),
        CreatedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCDemandRecord', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCDemandRecord
    (
        DemandRecordID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        DemandDate DATE NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NULL,
        OrganisationUnitID INT NULL,
        DemandSourceCode NVARCHAR(30) NOT NULL,
        DemandDocumentType NVARCHAR(50) NULL,
        DemandDocumentNo NVARCHAR(100) NULL,
        Quantity DECIMAL(18,4) NOT NULL,
        FulfilledQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        DemandStatusCode NVARCHAR(30) NOT NULL DEFAULT('OPEN'),
        RequiredByDate DATE NULL,
        CreatedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCSupplyRecord', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCSupplyRecord
    (
        SupplyRecordID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        SupplyDate DATE NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NULL,
        OrganisationUnitID INT NULL,
        SupplierID INT NULL,
        SupplySourceCode NVARCHAR(30) NOT NULL,
        SupplyDocumentType NVARCHAR(50) NULL,
        SupplyDocumentNo NVARCHAR(100) NULL,
        ExpectedDate DATE NULL,
        ReceivedDate DATE NULL,
        Quantity DECIMAL(18,4) NOT NULL,
        ReceivedQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        SupplyStatusCode NVARCHAR(30) NOT NULL DEFAULT('PLANNED'),
        CreatedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCFulfillmentRecord', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCFulfillmentRecord
    (
        FulfillmentRecordID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NULL,
        OrganisationUnitID INT NULL,
        OrderTypeCode NVARCHAR(30) NOT NULL,
        OrderNo NVARCHAR(100) NOT NULL,
        OrderDate DATE NOT NULL,
        PromiseDate DATE NULL,
        ActualShipDate DATE NULL,
        OrderedQty DECIMAL(18,4) NOT NULL,
        FulfilledQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        BackOrderQty DECIMAL(18,4) NOT NULL DEFAULT(0),
        FillRatePct DECIMAL(9,4) NULL,
        OnTimeFlag BIT NULL,
        StatusCode NVARCHAR(30) NOT NULL DEFAULT('OPEN'),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCInventoryCount', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCInventoryCount
    (
        InventoryCountID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        CountNo NVARCHAR(50) NOT NULL,
        CountDate DATE NOT NULL,
        WarehouseID INT NOT NULL,
        StorageLocationID INT NULL,
        ItemID INT NOT NULL,
        SystemQty DECIMAL(18,4) NOT NULL,
        CountedQty DECIMAL(18,4) NOT NULL,
        VarianceQty AS (CountedQty - SystemQty) PERSISTED,
        VarianceValue DECIMAL(18,4) NULL,
        ApprovalStatusCode NVARCHAR(30) NOT NULL DEFAULT('DRAFT'),
        ApprovedBy BIGINT NULL,
        ApprovedAt DATETIME2 NULL,
        CreatedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCInventorySnapshot', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCInventorySnapshot
    (
        InventorySnapshotID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        SnapshotDate DATE NOT NULL,
        ItemID INT NOT NULL,
        WarehouseID INT NOT NULL,
        OrganisationUnitID INT NULL,
        OnHandQty DECIMAL(18,4) NOT NULL,
        AvailableQty DECIMAL(18,4) NOT NULL,
        StockValue DECIMAL(18,4) NOT NULL,
        AgingDays INT NULL,
        IsLowStock BIT NOT NULL DEFAULT(0),
        IsStockOut BIT NOT NULL DEFAULT(0),
        IsOverStock BIT NOT NULL DEFAULT(0),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.SCDashboardConfig', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCDashboardConfig
    (
        DashboardConfigID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        DashboardCode NVARCHAR(50) NOT NULL,
        DashboardName NVARCHAR(150) NOT NULL,
        ConfigJson NVARCHAR(MAX) NOT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedBy BIGINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.SCAlert', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SCAlert
    (
        AlertID BIGINT IDENTITY(1,1) PRIMARY KEY,
        CompanyID INT NOT NULL,
        AlertCode NVARCHAR(50) NOT NULL,
        AlertSeverityCode NVARCHAR(20) NOT NULL,
        AlertTitle NVARCHAR(250) NOT NULL,
        AlertMessage NVARCHAR(1000) NOT NULL,
        ItemID INT NULL,
        WarehouseID INT NULL,
        OrganisationUnitID INT NULL,
        TriggeredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        IsAcknowledged BIT NOT NULL DEFAULT(0),
        AcknowledgedBy BIGINT NULL,
        AcknowledgedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.vwSCInventoryBalance', 'V') IS NOT NULL
    DROP VIEW dbo.vwSCInventoryBalance
GO

CREATE VIEW dbo.vwSCInventoryBalance AS
SELECT
    b.CompanyID,
    b.ItemID,
    i.ItemCode,
    i.ItemName,
    b.WarehouseID,
    w.WarehouseCode,
    w.WarehouseName,
    b.StorageLocationID,
    sl.LocationCode AS StorageLocationCode,
    b.StockStatusCode,
    b.OnHandQty,
    b.ReservedQty,
    b.InTransitQty,
    b.CommittedQty,
    (b.OnHandQty - b.ReservedQty - b.CommittedQty) AS AvailableQty,
    (b.OnHandQty * b.AverageUnitCost) AS StockValue,
    b.UpdatedAt,
    w.OrganisationUnitID
FROM dbo.SCInventoryBalance b
JOIN dbo.SCItem i ON i.ItemID = b.ItemID
JOIN dbo.SCWarehouse w ON w.WarehouseID = b.WarehouseID
LEFT JOIN dbo.SCStorageLocation sl ON sl.StorageLocationID = b.StorageLocationID
GO

IF OBJECT_ID('dbo.vwSCSupplyDemandGap', 'V') IS NOT NULL
    DROP VIEW dbo.vwSCSupplyDemandGap
GO

CREATE VIEW dbo.vwSCSupplyDemandGap AS
SELECT
    d.CompanyID,
    d.ItemID,
    i.ItemCode,
    i.ItemName,
    SUM(d.Quantity - d.FulfilledQty) AS DemandQty,
    ISNULL((SELECT SUM(s.Quantity - s.ReceivedQty)
            FROM dbo.SCSupplyRecord s
            WHERE s.CompanyID = d.CompanyID
              AND s.ItemID = d.ItemID
              AND s.SupplyStatusCode IN ('PLANNED','INTRANSIT')), 0) AS SupplyQty
FROM dbo.SCDemandRecord d
JOIN dbo.SCItem i ON i.ItemID = d.ItemID
WHERE d.DemandStatusCode IN ('OPEN','PARTIAL')
GROUP BY d.CompanyID, d.ItemID, i.ItemCode, i.ItemName
GO

CREATE OR ALTER PROCEDURE dbo.SupplyChainInventorySP
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @OrganisationUnitID INT = NULL,
    @IncludeChildren BIT = NULL,
    @WarehouseID INT = NULL,
    @StorageLocationID INT = NULL,
    @ItemID INT = NULL,
    @SupplierID INT = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @PeriodCode NVARCHAR(20) = NULL,
    @StockStatusCode NVARCHAR(30) = NULL,
    @TransactionTypeCode NVARCHAR(30) = NULL,
    @DemandSourceCode NVARCHAR(30) = NULL,
    @SerializedObject NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FromDate DATE = COALESCE(@StartDate, DATEADD(DAY, -30, CAST(SYSUTCDATETIME() AS DATE)));
    DECLARE @ToDate DATE = COALESCE(@EndDate, CAST(SYSUTCDATETIME() AS DATE));

    IF @PeriodCode = 'MTD'
    BEGIN
        SET @FromDate = DATEFROMPARTS(YEAR(SYSUTCDATETIME()), MONTH(SYSUTCDATETIME()), 1);
        SET @ToDate = CAST(SYSUTCDATETIME() AS DATE);
    END

    IF @PeriodCode = 'QTD'
    BEGIN
        SET @FromDate = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, SYSUTCDATETIME()), 0);
        SET @ToDate = CAST(SYSUTCDATETIME() AS DATE);
    END

    IF @PeriodCode = 'YTD'
    BEGIN
        SET @FromDate = DATEFROMPARTS(YEAR(SYSUTCDATETIME()), 1, 1);
        SET @ToDate = CAST(SYSUTCDATETIME() AS DATE);
    END

    IF @Operation = 'rtvItems'
    BEGIN
        SELECT i.ItemID, i.CompanyID, i.ItemCode, i.ItemName,
               i.ItemCategoryID, c.CategoryCode AS ItemCategoryCode, c.CategoryName AS ItemCategoryName,
               u.UOMCode AS UnitOfMeasureCode, i.StockTypeCode, i.ItemStatusCode,
               i.ReplenishmentMethodCode, i.PreferredSupplierID, i.LeadTimeProfileID,
               i.SafetyStockQty, i.MinStockQty, i.MaxStockQty, i.ReorderPointQty, i.ReorderQty,
               i.ABCClassCode, i.IsActive
        FROM dbo.SCItem i
        LEFT JOIN dbo.SCItemCategory c ON c.ItemCategoryID = i.ItemCategoryID
        LEFT JOIN dbo.SCUnitOfMeasure u ON u.UnitOfMeasureID = i.UnitOfMeasureID
        WHERE i.CompanyID = @CompanyID
        ORDER BY i.ItemCode;
        RETURN;
    END

    IF @Operation = 'rtvInventoryTransactions'
    BEGIN
        SELECT TOP 500 t.InventoryTransactionID, t.TransactionDate, t.TransactionTypeCode, t.MovementTypeCode,
               t.ItemID, i.ItemCode, i.ItemName,
               t.WarehouseID, t.StorageLocationID, t.ToWarehouseID, t.ToStorageLocationID,
               t.Quantity, t.UnitCost, t.TotalCost, t.RelatedDocType, t.RelatedDocNo,
               t.ApprovalStatusCode, t.ReasonCode
        FROM dbo.SCInventoryTransaction t
        JOIN dbo.SCItem i ON i.ItemID = t.ItemID
        WHERE t.CompanyID = @CompanyID
          AND (@ItemID IS NULL OR t.ItemID = @ItemID)
          AND (@WarehouseID IS NULL OR t.WarehouseID = @WarehouseID OR t.ToWarehouseID = @WarehouseID)
        ORDER BY t.TransactionDate DESC;
        RETURN;
    END

    IF @Operation = 'SaveItem'
    BEGIN
        DECLARE @pItemID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT);
        DECLARE @pItemCode NVARCHAR(60) = JSON_VALUE(@SerializedObject, '$.ItemCode');
        DECLARE @pItemName NVARCHAR(250) = JSON_VALUE(@SerializedObject, '$.ItemName');
        DECLARE @pItemCategoryID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemCategoryID') AS INT);
        DECLARE @pUnitOfMeasureID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.UnitOfMeasureID') AS INT);
        DECLARE @pStockTypeCode NVARCHAR(30) = JSON_VALUE(@SerializedObject, '$.StockTypeCode');
        DECLARE @pItemStatusCode NVARCHAR(30) = COALESCE(JSON_VALUE(@SerializedObject, '$.ItemStatusCode'), 'ACTIVE');

        IF @pItemID IS NULL
        BEGIN
            INSERT INTO dbo.SCItem
            (
                CompanyID, ItemCode, ItemName, ItemCategoryID, UnitOfMeasureID, StockTypeCode,
                ItemStatusCode, ReplenishmentMethodCode, PreferredSupplierID, LeadTimeProfileID,
                SafetyStockQty, MinStockQty, MaxStockQty, ReorderPointQty, ReorderQty,
                ABCClassCode, CreatedBy
            )
            VALUES
            (
                @CompanyID, @pItemCode, @pItemName, @pItemCategoryID, @pUnitOfMeasureID, @pStockTypeCode,
                @pItemStatusCode, JSON_VALUE(@SerializedObject, '$.ReplenishmentMethodCode'),
                TRY_CAST(JSON_VALUE(@SerializedObject, '$.PreferredSupplierID') AS INT),
                TRY_CAST(JSON_VALUE(@SerializedObject, '$.LeadTimeProfileID') AS INT),
                COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.SafetyStockQty') AS DECIMAL(18,4)), 0),
                COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.MinStockQty') AS DECIMAL(18,4)), 0),
                TRY_CAST(JSON_VALUE(@SerializedObject, '$.MaxStockQty') AS DECIMAL(18,4)),
                COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderPointQty') AS DECIMAL(18,4)), 0),
                TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderQty') AS DECIMAL(18,4)),
                JSON_VALUE(@SerializedObject, '$.ABCClassCode'),
                @UserID
            );
        END
        ELSE
        BEGIN
            UPDATE dbo.SCItem
               SET ItemCode = @pItemCode,
                   ItemName = @pItemName,
                   ItemCategoryID = @pItemCategoryID,
                   UnitOfMeasureID = @pUnitOfMeasureID,
                   StockTypeCode = @pStockTypeCode,
                   ItemStatusCode = @pItemStatusCode,
                   ReplenishmentMethodCode = JSON_VALUE(@SerializedObject, '$.ReplenishmentMethodCode'),
                   PreferredSupplierID = TRY_CAST(JSON_VALUE(@SerializedObject, '$.PreferredSupplierID') AS INT),
                   LeadTimeProfileID = TRY_CAST(JSON_VALUE(@SerializedObject, '$.LeadTimeProfileID') AS INT),
                   SafetyStockQty = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.SafetyStockQty') AS DECIMAL(18,4)), 0),
                   MinStockQty = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.MinStockQty') AS DECIMAL(18,4)), 0),
                   MaxStockQty = TRY_CAST(JSON_VALUE(@SerializedObject, '$.MaxStockQty') AS DECIMAL(18,4)),
                   ReorderPointQty = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderPointQty') AS DECIMAL(18,4)), 0),
                   ReorderQty = TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderQty') AS DECIMAL(18,4)),
                   ABCClassCode = JSON_VALUE(@SerializedObject, '$.ABCClassCode'),
                   ModifiedBy = @UserID,
                   UpdatedAt = SYSUTCDATETIME()
             WHERE ItemID = @pItemID
               AND CompanyID = @CompanyID;
        END

        SELECT TOP 1 *
        FROM dbo.SCItem
        WHERE CompanyID = @CompanyID
        ORDER BY ItemID DESC;
        RETURN;
    END

    IF @Operation = 'SaveInventoryTransaction'
    BEGIN
        DECLARE @tItemID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT);
        DECLARE @tWarehouseID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.WarehouseID') AS INT);
        DECLARE @tStorageLocationID INT = TRY_CAST(JSON_VALUE(@SerializedObject, '$.StorageLocationID') AS INT);
        DECLARE @tQty DECIMAL(18,4) = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.Quantity') AS DECIMAL(18,4)), 0);
        DECLARE @tType NVARCHAR(30) = COALESCE(JSON_VALUE(@SerializedObject, '$.TransactionTypeCode'), 'ADJUSTMENT');
        DECLARE @tMovement NVARCHAR(30) = COALESCE(JSON_VALUE(@SerializedObject, '$.MovementTypeCode'), 'ADJUSTMENT');
        DECLARE @tUnitCost DECIMAL(18,4) = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.UnitCost') AS DECIMAL(18,4)), 0);
        DECLARE @tDate DATETIME2 = COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.TransactionDate') AS DATETIME2), SYSUTCDATETIME());
        DECLARE @tDocType NVARCHAR(50) = JSON_VALUE(@SerializedObject, '$.RelatedDocType');
        DECLARE @tDocNo NVARCHAR(100) = JSON_VALUE(@SerializedObject, '$.RelatedDocNo');

        IF EXISTS (SELECT 1 FROM dbo.SCItem WHERE ItemID = @tItemID AND CompanyID = @CompanyID AND ItemStatusCode IN ('BLOCKED','INACTIVE','OBSOLETE'))
        BEGIN
            RAISERROR('Item status does not allow new transactions.', 16, 1);
            RETURN;
        END

        IF @tQty = 0
        BEGIN
            RAISERROR('Transaction quantity cannot be zero.', 16, 1);
            RETURN;
        END

        INSERT INTO dbo.SCInventoryTransaction
        (
            CompanyID, TransactionDate, TransactionTypeCode, MovementTypeCode, ItemID, WarehouseID,
            StorageLocationID, ToWarehouseID, ToStorageLocationID, Quantity, UnitCost,
            StockStatusCode, ReasonCode, RelatedDocType, RelatedDocNo, OrganisationUnitID,
            ApprovalStatusCode, WorkflowStatusCode, CreatedBy
        )
        VALUES
        (
            @CompanyID,
            @tDate,
            @tType,
            @tMovement,
            @tItemID,
            @tWarehouseID,
            @tStorageLocationID,
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ToWarehouseID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ToStorageLocationID') AS INT),
            @tQty,
            @tUnitCost,
            COALESCE(JSON_VALUE(@SerializedObject, '$.StockStatusCode'), 'AVAILABLE'),
            JSON_VALUE(@SerializedObject, '$.ReasonCode'),
            @tDocType,
            @tDocNo,
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.OrganisationUnitID') AS INT),
            COALESCE(JSON_VALUE(@SerializedObject, '$.ApprovalStatusCode'), 'APPROVED'),
            COALESCE(JSON_VALUE(@SerializedObject, '$.WorkflowStatusCode'), 'APPROVED'),
            @UserID
        );

        MERGE dbo.SCInventoryBalance AS target
        USING
        (
            SELECT @CompanyID AS CompanyID,
                   @tItemID AS ItemID,
                   @tWarehouseID AS WarehouseID,
                   @tStorageLocationID AS StorageLocationID
        ) AS src
        ON target.CompanyID = src.CompanyID
           AND target.ItemID = src.ItemID
           AND target.WarehouseID = src.WarehouseID
           AND ISNULL(target.StorageLocationID, 0) = ISNULL(src.StorageLocationID, 0)
        WHEN MATCHED THEN
            UPDATE SET OnHandQty = target.OnHandQty + @tQty,
                       LastMovementAt = @tDate,
                       AverageUnitCost = CASE WHEN @tUnitCost > 0 THEN @tUnitCost ELSE target.AverageUnitCost END,
                       UpdatedBy = @UserID,
                       UpdatedAt = SYSUTCDATETIME()
        WHEN NOT MATCHED THEN
            INSERT (CompanyID, ItemID, WarehouseID, StorageLocationID, StockStatusCode, OnHandQty, AverageUnitCost, LastMovementAt, UpdatedBy)
            VALUES (@CompanyID, @tItemID, @tWarehouseID, @tStorageLocationID, 'AVAILABLE', @tQty, @tUnitCost, @tDate, @UserID);

        IF EXISTS
        (
            SELECT 1
            FROM dbo.SCInventoryBalance b
            JOIN dbo.SCItem i ON i.ItemID = b.ItemID
            WHERE b.CompanyID = @CompanyID
              AND b.ItemID = @tItemID
              AND b.WarehouseID = @tWarehouseID
              AND ISNULL(b.StorageLocationID,0) = ISNULL(@tStorageLocationID,0)
              AND b.OnHandQty < 0
              AND i.IsNegativeStockAllowed = 0
        )
        BEGIN
            RAISERROR('Negative stock is not allowed for this item.', 16, 1);
            RETURN;
        END

        SELECT TOP 1 *
        FROM dbo.SCInventoryTransaction
        WHERE CompanyID = @CompanyID
        ORDER BY InventoryTransactionID DESC;
        RETURN;
    END

    IF @Operation = 'SaveReplenishmentRule'
    BEGIN
        INSERT INTO dbo.SCReplenishmentRule
        (
            CompanyID, ItemID, WarehouseID, StorageLocationID, RuleCode, RuleName,
            ReplenishmentMethodCode, MinStockQty, MaxStockQty, ReorderPointQty, ReorderQty,
            SafetyStockQty, ReviewCycleDays, PreferredSupplierID, CreatedBy
        )
        VALUES
        (
            @CompanyID,
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.WarehouseID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.StorageLocationID') AS INT),
            COALESCE(JSON_VALUE(@SerializedObject, '$.RuleCode'), CONCAT('RPL-', FORMAT(SYSUTCDATETIME(),'yyyyMMddHHmmss'))),
            COALESCE(JSON_VALUE(@SerializedObject, '$.RuleName'), 'Auto Rule'),
            COALESCE(JSON_VALUE(@SerializedObject, '$.ReplenishmentMethodCode'), 'MINMAX'),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.MinStockQty') AS DECIMAL(18,4)),0),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.MaxStockQty') AS DECIMAL(18,4)),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderPointQty') AS DECIMAL(18,4)),0),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReorderQty') AS DECIMAL(18,4)),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.SafetyStockQty') AS DECIMAL(18,4)),0),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReviewCycleDays') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.PreferredSupplierID') AS INT),
            @UserID
        );

        SELECT TOP 1 * FROM dbo.SCReplenishmentRule WHERE CompanyID = @CompanyID ORDER BY ReplenishmentRuleID DESC;
        RETURN;
    END

    IF @Operation = 'SaveDemandRecord'
    BEGIN
        INSERT INTO dbo.SCDemandRecord
        (
            CompanyID, DemandDate, ItemID, WarehouseID, OrganisationUnitID, DemandSourceCode,
            DemandDocumentType, DemandDocumentNo, Quantity, FulfilledQty, DemandStatusCode,
            RequiredByDate, CreatedBy
        )
        VALUES
        (
            @CompanyID,
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.DemandDate') AS DATE), CAST(SYSUTCDATETIME() AS DATE)),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.WarehouseID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.OrganisationUnitID') AS INT),
            COALESCE(JSON_VALUE(@SerializedObject, '$.DemandSourceCode'), 'INTERNAL'),
            JSON_VALUE(@SerializedObject, '$.DemandDocumentType'),
            JSON_VALUE(@SerializedObject, '$.DemandDocumentNo'),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.Quantity') AS DECIMAL(18,4)), 0),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.FulfilledQty') AS DECIMAL(18,4)), 0),
            COALESCE(JSON_VALUE(@SerializedObject, '$.DemandStatusCode'), 'OPEN'),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.RequiredByDate') AS DATE),
            @UserID
        );

        SELECT TOP 1 * FROM dbo.SCDemandRecord WHERE CompanyID = @CompanyID ORDER BY DemandRecordID DESC;
        RETURN;
    END

    IF @Operation = 'SaveSupplyRecord'
    BEGIN
        INSERT INTO dbo.SCSupplyRecord
        (
            CompanyID, SupplyDate, ItemID, WarehouseID, OrganisationUnitID, SupplierID, SupplySourceCode,
            SupplyDocumentType, SupplyDocumentNo, ExpectedDate, ReceivedDate, Quantity, ReceivedQty,
            SupplyStatusCode, CreatedBy
        )
        VALUES
        (
            @CompanyID,
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.SupplyDate') AS DATE), CAST(SYSUTCDATETIME() AS DATE)),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.WarehouseID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.OrganisationUnitID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.SupplierID') AS INT),
            COALESCE(JSON_VALUE(@SerializedObject, '$.SupplySourceCode'), 'PROCUREMENT'),
            JSON_VALUE(@SerializedObject, '$.SupplyDocumentType'),
            JSON_VALUE(@SerializedObject, '$.SupplyDocumentNo'),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ExpectedDate') AS DATE),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReceivedDate') AS DATE),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.Quantity') AS DECIMAL(18,4)), 0),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.ReceivedQty') AS DECIMAL(18,4)), 0),
            COALESCE(JSON_VALUE(@SerializedObject, '$.SupplyStatusCode'), 'PLANNED'),
            @UserID
        );

        SELECT TOP 1 * FROM dbo.SCSupplyRecord WHERE CompanyID = @CompanyID ORDER BY SupplyRecordID DESC;
        RETURN;
    END

    IF @Operation = 'SaveInventoryCount'
    BEGIN
        INSERT INTO dbo.SCInventoryCount
        (
            CompanyID, CountNo, CountDate, WarehouseID, StorageLocationID, ItemID, SystemQty,
            CountedQty, VarianceValue, ApprovalStatusCode, CreatedBy
        )
        VALUES
        (
            @CompanyID,
            COALESCE(JSON_VALUE(@SerializedObject, '$.CountNo'), CONCAT('CNT-', FORMAT(SYSUTCDATETIME(),'yyyyMMddHHmmss'))),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.CountDate') AS DATE), CAST(SYSUTCDATETIME() AS DATE)),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.WarehouseID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.StorageLocationID') AS INT),
            TRY_CAST(JSON_VALUE(@SerializedObject, '$.ItemID') AS INT),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.SystemQty') AS DECIMAL(18,4)),0),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.CountedQty') AS DECIMAL(18,4)),0),
            COALESCE(TRY_CAST(JSON_VALUE(@SerializedObject, '$.VarianceValue') AS DECIMAL(18,4)),0),
            COALESCE(JSON_VALUE(@SerializedObject, '$.ApprovalStatusCode'),'DRAFT'),
            @UserID
        );

        SELECT TOP 1 * FROM dbo.SCInventoryCount WHERE CompanyID = @CompanyID ORDER BY InventoryCountID DESC;
        RETURN;
    END

    IF @Operation = 'rtvSupplyChainDashboard'
    BEGIN
        ;WITH BalanceScope AS
        (
            SELECT *
            FROM dbo.vwSCInventoryBalance b
            WHERE b.CompanyID = @CompanyID
              AND (@WarehouseID IS NULL OR b.WarehouseID = @WarehouseID)
              AND (@StorageLocationID IS NULL OR b.StorageLocationID = @StorageLocationID)
              AND (@ItemID IS NULL OR b.ItemID = @ItemID)
              AND (@StockStatusCode IS NULL OR b.StockStatusCode = @StockStatusCode)
              AND (@OrganisationUnitID IS NULL OR b.OrganisationUnitID = @OrganisationUnitID)
        ),
        GapScope AS
        (
            SELECT g.CompanyID, g.ItemID, g.ItemCode, g.ItemName, g.DemandQty, g.SupplyQty,
                   (g.SupplyQty - g.DemandQty) AS GapQty,
                   CASE WHEN (g.SupplyQty - g.DemandQty) < 0 THEN 'Shortage'
                        WHEN (g.SupplyQty - g.DemandQty) = 0 THEN 'Balanced'
                        ELSE 'Excess' END AS GapStatus
            FROM dbo.vwSCSupplyDemandGap g
            WHERE g.CompanyID = @CompanyID
              AND (@ItemID IS NULL OR g.ItemID = @ItemID)
        )
        SELECT 'TotalStockOnHand' AS Code, 'Total Stock On Hand' AS Label,
               CAST(ISNULL((SELECT SUM(OnHandQty) FROM BalanceScope),0) AS DECIMAL(18,2)) AS Value,
               'qty' AS Unit, NULL AS PreviousValue,
               'stable' AS Trend
        UNION ALL
        SELECT 'TotalStockValue', 'Total Stock Value',
               CAST(ISNULL((SELECT SUM(StockValue) FROM BalanceScope),0) AS DECIMAL(18,2)),
               'value', NULL, 'stable'
        UNION ALL
        SELECT 'LowStockItems', 'Low Stock Items',
               CAST(ISNULL((SELECT COUNT(DISTINCT b.ItemID)
                            FROM BalanceScope b
                            JOIN dbo.SCItem i ON i.ItemID = b.ItemID
                            WHERE b.AvailableQty <= i.ReorderPointQty),0) AS DECIMAL(18,2)),
               'count', NULL, 'unfavorable'
        UNION ALL
        SELECT 'OutOfStockItems', 'Out Of Stock Items',
               CAST(ISNULL((SELECT COUNT(DISTINCT b.ItemID)
                            FROM BalanceScope b
                            WHERE b.OnHandQty <= 0),0) AS DECIMAL(18,2)),
               'count', NULL, 'unfavorable'
        UNION ALL
        SELECT 'ReorderDue', 'Reorder Due',
               CAST(ISNULL((SELECT COUNT(1)
                            FROM dbo.SCReplenishmentRule r
                            JOIN BalanceScope b ON b.ItemID = r.ItemID AND b.WarehouseID = r.WarehouseID
                            WHERE r.CompanyID = @CompanyID
                              AND r.IsActive = 1
                              AND b.AvailableQty <= r.ReorderPointQty),0) AS DECIMAL(18,2)),
               'count', NULL, 'unfavorable'
        UNION ALL
        SELECT 'FillRate', 'Fill Rate',
               CAST(ISNULL((SELECT CASE WHEN SUM(OrderedQty) = 0 THEN 100 ELSE 100.0 * SUM(FulfilledQty) / NULLIF(SUM(OrderedQty),0) END
                            FROM dbo.SCFulfillmentRecord f
                            WHERE f.CompanyID = @CompanyID
                              AND f.OrderDate BETWEEN @FromDate AND @ToDate),100) AS DECIMAL(18,2)),
               '%', NULL, 'favorable'
        UNION ALL
        SELECT 'StockAccuracy', 'Stock Accuracy',
               CAST(ISNULL((SELECT CASE WHEN COUNT(1) = 0 THEN 100
                                        ELSE 100.0 * SUM(CASE WHEN ABS(VarianceQty) <= 0.01 THEN 1 ELSE 0 END) / NULLIF(COUNT(1),0) END
                            FROM dbo.SCInventoryCount c
                            WHERE c.CompanyID = @CompanyID
                              AND c.CountDate BETWEEN @FromDate AND @ToDate),100) AS DECIMAL(18,2)),
               '%', NULL, 'favorable'
        UNION ALL
        SELECT 'DelayedReplenishment', 'Delayed Replenishment',
               CAST(ISNULL((SELECT COUNT(1)
                            FROM dbo.SCSupplyRecord s
                            WHERE s.CompanyID = @CompanyID
                              AND s.ExpectedDate IS NOT NULL
                              AND s.ExpectedDate < CAST(SYSUTCDATETIME() AS DATE)
                              AND s.SupplyStatusCode IN ('PLANNED','INTRANSIT')),0) AS DECIMAL(18,2)),
               'count', NULL, 'unfavorable';

        SELECT TOP 25 *
        FROM BalanceScope
        ORDER BY AvailableQty ASC, ItemCode;

        SELECT TOP 25
               r.ReplenishmentRuleID, r.ItemID, i.ItemCode, i.ItemName,
               r.WarehouseID, w.WarehouseName,
               CAST(b.AvailableQty AS DECIMAL(18,4)) AS AvailableQty,
               r.MinStockQty, r.ReorderPointQty,
               CAST(CASE WHEN r.MaxStockQty IS NOT NULL
                         THEN (r.MaxStockQty - b.AvailableQty)
                         ELSE ISNULL(r.ReorderQty, r.ReorderPointQty - b.AvailableQty)
                    END AS DECIMAL(18,4)) AS SuggestedReplenishmentQty,
               DATEADD(DAY, ISNULL(r.ReviewCycleDays, 0), CAST(SYSUTCDATETIME() AS DATE)) AS NextDueDate,
               CAST(NULL AS NVARCHAR(200)) AS SupplierName,
               ISNULL(DATEDIFF(DAY, s.ExpectedDate, CAST(SYSUTCDATETIME() AS DATE)), 0) AS DelayDays
        FROM dbo.SCReplenishmentRule r
        JOIN dbo.SCItem i ON i.ItemID = r.ItemID
        JOIN dbo.SCWarehouse w ON w.WarehouseID = r.WarehouseID
        LEFT JOIN BalanceScope b ON b.ItemID = r.ItemID AND b.WarehouseID = r.WarehouseID
        LEFT JOIN dbo.SCSupplyRecord s ON s.CompanyID = r.CompanyID AND s.ItemID = r.ItemID AND s.WarehouseID = r.WarehouseID AND s.SupplyStatusCode IN ('PLANNED','INTRANSIT')
        WHERE r.CompanyID = @CompanyID
          AND r.IsActive = 1
          AND ISNULL(b.AvailableQty, 0) <= r.ReorderPointQty
        ORDER BY SuggestedReplenishmentQty DESC;

        SELECT TOP 25 ItemID, ItemCode, ItemName, DemandQty, SupplyQty, GapQty, GapStatus
        FROM GapScope
        ORDER BY ABS(GapQty) DESC;

        SELECT TOP 25
            b.ItemID, b.ItemCode, b.ItemName,
            b.WarehouseID, b.WarehouseName,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) <= 30 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age0To30Qty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) BETWEEN 31 AND 60 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age31To60Qty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) BETWEEN 61 AND 90 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age61To90Qty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) BETWEEN 91 AND 180 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age91To180Qty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) BETWEEN 181 AND 365 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age181To365Qty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) > 365 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS Age365PlusQty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) > 180 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS SlowMovingQty,
            CAST(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) > 365 THEN b.OnHandQty ELSE 0 END AS DECIMAL(18,4)) AS ObsoleteQty
        FROM BalanceScope b
        ORDER BY Age365PlusQty DESC, Age181To365Qty DESC;

        SELECT
            ou.UnitID AS OrganisationUnitID,
            ou.UnitCode,
            ou.UnitName,
            COUNT(DISTINCT b.ItemID) AS ItemCount,
            CAST(ISNULL(SUM(b.OnHandQty), 0) AS DECIMAL(18,4)) AS OnHandQty,
            SUM(CASE WHEN b.AvailableQty <= i.ReorderPointQty THEN 1 ELSE 0 END) AS LowStockCount,
            SUM(CASE WHEN b.OnHandQty <= 0 THEN 1 ELSE 0 END) AS StockOutCount,
            SUM(CASE WHEN b.AvailableQty <= i.ReorderPointQty THEN 1 ELSE 0 END) AS ReorderDueCount,
            SUM(CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.SCSupplyRecord s
                    WHERE s.CompanyID = b.CompanyID
                      AND s.ItemID = b.ItemID
                      AND s.WarehouseID = b.WarehouseID
                      AND s.ExpectedDate < CAST(SYSUTCDATETIME() AS DATE)
                      AND s.SupplyStatusCode IN ('PLANNED','INTRANSIT')
                ) THEN 1 ELSE 0 END) AS DelayedReplenishmentCount,
            CAST(100.0 * ISNULL((SELECT SUM(FulfilledQty) FROM dbo.SCFulfillmentRecord f WHERE f.CompanyID = @CompanyID AND f.OrganisationUnitID = ou.UnitID),0)
                         / NULLIF((SELECT SUM(OrderedQty) FROM dbo.SCFulfillmentRecord f WHERE f.CompanyID = @CompanyID AND f.OrganisationUnitID = ou.UnitID),0) AS DECIMAL(9,2)) AS FillRatePct,
            CAST(ISNULL((SELECT SUM(Quantity) FROM dbo.SCInventoryTransaction t WHERE t.CompanyID = @CompanyID AND t.OrganisationUnitID = ou.UnitID AND t.TransactionTypeCode = 'ISSUE'),0)
                / NULLIF(AVG(NULLIF(b.OnHandQty,0)),0) AS DECIMAL(9,2)) AS InventoryTurnover,
            CAST(100.0 * SUM(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) > 180 THEN 1 ELSE 0 END)
                / NULLIF(COUNT(1),0) AS DECIMAL(9,2)) AS AgingExposurePct,
            CAST(100
                 - (10 * SUM(CASE WHEN b.AvailableQty <= i.ReorderPointQty THEN 1 ELSE 0 END))
                 - (15 * SUM(CASE WHEN b.OnHandQty <= 0 THEN 1 ELSE 0 END))
                 - (5 * SUM(CASE WHEN DATEDIFF(DAY, ISNULL(b.LastMovementAt, DATEADD(DAY,-400,SYSUTCDATETIME())), SYSUTCDATETIME()) > 180 THEN 1 ELSE 0 END))
                AS DECIMAL(9,2)) AS SupplyHealthScore
        FROM dbo.OrganisationUnit ou
        LEFT JOIN BalanceScope b ON b.OrganisationUnitID = ou.UnitID
        LEFT JOIN dbo.SCItem i ON i.ItemID = b.ItemID
        WHERE ou.CompanyID = @CompanyID
          AND (@OrganisationUnitID IS NULL OR ou.UnitID = @OrganisationUnitID OR (@IncludeChildren = 1 AND ou.ParentUnitID = @OrganisationUnitID))
        GROUP BY ou.UnitID, ou.UnitCode, ou.UnitName
        ORDER BY SupplyHealthScore ASC, ou.UnitName;

        SELECT TOP 50 AlertID, AlertCode, AlertSeverityCode, AlertTitle, AlertMessage,
               ItemID, WarehouseID, OrganisationUnitID, TriggeredAt, IsAcknowledged
        FROM dbo.SCAlert
        WHERE CompanyID = @CompanyID
          AND IsAcknowledged = 0
        ORDER BY TriggeredAt DESC;

        SELECT TOP 30
            t.InventoryTransactionID,
            t.TransactionDate,
            t.TransactionTypeCode,
            t.MovementTypeCode,
            t.ItemID,
            i.ItemCode,
            i.ItemName,
            t.WarehouseID,
            t.StorageLocationID,
            t.ToWarehouseID,
            t.ToStorageLocationID,
            t.Quantity,
            t.UnitCost,
            t.TotalCost,
            t.RelatedDocType,
            t.RelatedDocNo,
            t.ApprovalStatusCode,
            t.ReasonCode
        FROM dbo.SCInventoryTransaction t
        JOIN dbo.SCItem i ON i.ItemID = t.ItemID
        WHERE t.CompanyID = @CompanyID
          AND t.TransactionDate >= @FromDate
          AND t.TransactionDate < DATEADD(DAY, 1, @ToDate)
        ORDER BY t.TransactionDate DESC;

        RETURN;
    END
END
GO

/* Demo seed data for enterprise supply chain scenarios */
IF NOT EXISTS (SELECT 1 FROM dbo.SCItemCategory WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCItemCategory (CompanyID, CategoryCode, CategoryName, CreatedBy)
    VALUES
    (1, 'RAW', 'Raw Material', 1),
    (1, 'FG', 'Finished Goods', 1),
    (1, 'MRO', 'MRO Supplies', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCUnitOfMeasure WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCUnitOfMeasure (CompanyID, UOMCode, UOMName, UOMType)
    VALUES
    (1, 'EA', 'Each', 'COUNT'),
    (1, 'BOX', 'Box', 'PACK'),
    (1, 'KG', 'Kilogram', 'WEIGHT');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCLeadTimeProfile WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCLeadTimeProfile (CompanyID, ProfileCode, ProfileName, ProcurementLeadDays, InternalTransferLeadDays, ReplenishmentCycleDays, TargetServiceLevelPct)
    VALUES
    (1, 'STD', 'Standard Lead Time', 14, 3, 7, 95),
    (1, 'FAST', 'Fast Track', 5, 1, 3, 98);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCStockStatus WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCStockStatus (CompanyID, StockStatusCode, StockStatusName, IsAvailable, IsSaleable)
    VALUES
    (1, 'AVAILABLE', 'Available', 1, 1),
    (1, 'HOLD', 'Quality Hold', 0, 0),
    (1, 'DAMAGED', 'Damaged', 0, 0),
    (1, 'TRANSIT', 'In Transit', 0, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCWarehouse WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCWarehouse (CompanyID, WarehouseCode, WarehouseName, OrganisationUnitID, WarehouseTypeCode)
    VALUES
    (1, 'WH-CEN', 'Central Warehouse', 20, 'DC'),
    (1, 'WH-NOR', 'North Regional Warehouse', 21, 'REGIONAL'),
    (1, 'WH-SOU', 'South Regional Warehouse', 22, 'REGIONAL');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCStorageLocation WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCStorageLocation (CompanyID, WarehouseID, LocationCode, LocationName, LocationTypeCode, IsPickLocation)
    SELECT 1, w.WarehouseID, CONCAT(w.WarehouseCode, '-A1'), 'Primary Picking', 'PICK', 1
    FROM dbo.SCWarehouse w
    WHERE w.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCItem WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCItem
    (
        CompanyID, ItemCode, ItemName, ItemCategoryID, UnitOfMeasureID, StockTypeCode, ItemStatusCode,
        ReplenishmentMethodCode, LeadTimeProfileID, SafetyStockQty, MinStockQty, MaxStockQty, ReorderPointQty, ReorderQty, ABCClassCode, CreatedBy
    )
    SELECT
        1,
        v.ItemCode,
        v.ItemName,
        c.ItemCategoryID,
        u.UnitOfMeasureID,
        v.StockTypeCode,
        'ACTIVE',
        'MINMAX',
        lt.LeadTimeProfileID,
        v.SafetyStockQty,
        v.MinStockQty,
        v.MaxStockQty,
        v.ReorderPointQty,
        v.ReorderQty,
        v.ABCClassCode,
        1
    FROM (VALUES
        ('SKU-1001', 'Industrial Fastener Kit', 'FG', 'EA', 'FINISHED', 50, 100, 600, 140, 300, 'A'),
        ('SKU-1002', 'Hydraulic Seal Pack', 'RAW', 'EA', 'RAW', 35, 80, 450, 110, 220, 'A'),
        ('SKU-1003', 'Packaging Carton Medium', 'MRO', 'BOX', 'SUPPLY', 120, 200, 1200, 280, 500, 'B'),
        ('SKU-1004', 'Lubricant Grade-L', 'RAW', 'KG', 'RAW', 200, 250, 1600, 400, 600, 'C')
    ) v(ItemCode, ItemName, CategoryCode, UOMCode, StockTypeCode, SafetyStockQty, MinStockQty, MaxStockQty, ReorderPointQty, ReorderQty, ABCClassCode)
    JOIN dbo.SCItemCategory c ON c.CompanyID = 1 AND c.CategoryCode = v.CategoryCode
    JOIN dbo.SCUnitOfMeasure u ON u.CompanyID = 1 AND u.UOMCode = v.UOMCode
    CROSS JOIN (SELECT TOP 1 LeadTimeProfileID FROM dbo.SCLeadTimeProfile WHERE CompanyID = 1 ORDER BY LeadTimeProfileID) lt;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCInventoryBalance WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCInventoryBalance
    (
        CompanyID, ItemID, WarehouseID, StorageLocationID, StockStatusCode,
        OnHandQty, ReservedQty, InTransitQty, CommittedQty, AverageUnitCost, LastMovementAt, UpdatedBy
    )
    SELECT
        1,
        i.ItemID,
        w.WarehouseID,
        sl.StorageLocationID,
        'AVAILABLE',
        CASE
            WHEN i.ItemCode = 'SKU-1001' AND w.WarehouseCode = 'WH-CEN' THEN 85
            WHEN i.ItemCode = 'SKU-1002' AND w.WarehouseCode = 'WH-CEN' THEN 70
            WHEN i.ItemCode = 'SKU-1001' AND w.WarehouseCode = 'WH-NOR' THEN 20
            WHEN i.ItemCode = 'SKU-1004' AND w.WarehouseCode = 'WH-SOU' THEN 160
            ELSE 40
        END,
        CASE WHEN w.WarehouseCode = 'WH-CEN' THEN 10 ELSE 2 END,
        CASE WHEN w.WarehouseCode = 'WH-SOU' THEN 15 ELSE 3 END,
        CASE WHEN w.WarehouseCode = 'WH-NOR' THEN 8 ELSE 4 END,
        CASE
            WHEN i.ItemCode = 'SKU-1001' THEN 18.50
            WHEN i.ItemCode = 'SKU-1002' THEN 12.20
            WHEN i.ItemCode = 'SKU-1003' THEN 4.80
            ELSE 9.10
        END,
        DATEADD(DAY, -1 * ABS(CHECKSUM(NEWID())) % 380, SYSUTCDATETIME()),
        1
    FROM dbo.SCItem i
    CROSS JOIN dbo.SCWarehouse w
    JOIN dbo.SCStorageLocation sl ON sl.WarehouseID = w.WarehouseID
    WHERE i.CompanyID = 1
      AND w.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCReplenishmentRule WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCReplenishmentRule
    (
        CompanyID, ItemID, WarehouseID, StorageLocationID, RuleCode, RuleName,
        ReplenishmentMethodCode, MinStockQty, MaxStockQty, ReorderPointQty, ReorderQty,
        SafetyStockQty, ReviewCycleDays, PreferredSupplierID, CreatedBy
    )
    SELECT
        1,
        b.ItemID,
        b.WarehouseID,
        b.StorageLocationID,
        CONCAT('RULE-', i.ItemCode, '-', w.WarehouseCode),
        CONCAT('Auto ', i.ItemCode, ' @ ', w.WarehouseCode),
        'MINMAX',
        i.MinStockQty,
        i.MaxStockQty,
        i.ReorderPointQty,
        i.ReorderQty,
        i.SafetyStockQty,
        7,
        NULL,
        1
    FROM dbo.SCInventoryBalance b
    JOIN dbo.SCItem i ON i.ItemID = b.ItemID
    JOIN dbo.SCWarehouse w ON w.WarehouseID = b.WarehouseID
    WHERE b.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCDemandRecord WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCDemandRecord
    (
        CompanyID, DemandDate, ItemID, WarehouseID, OrganisationUnitID,
        DemandSourceCode, DemandDocumentType, DemandDocumentNo,
        Quantity, FulfilledQty, DemandStatusCode, RequiredByDate, CreatedBy
    )
    SELECT
        1,
        CAST(DATEADD(DAY, -1 * ABS(CHECKSUM(NEWID())) % 25, SYSUTCDATETIME()) AS DATE),
        i.ItemID,
        w.WarehouseID,
        w.OrganisationUnitID,
        CASE WHEN i.ItemCode IN ('SKU-1001','SKU-1002') THEN 'SALES' ELSE 'PROJECT' END,
        'SO',
        CONCAT('SO-', FORMAT(ABS(CHECKSUM(NEWID())) % 99999, '00000')),
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 160 WHEN i.ItemCode = 'SKU-1002' THEN 120 ELSE 90 END,
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 90 WHEN i.ItemCode = 'SKU-1002' THEN 70 ELSE 40 END,
        'PARTIAL',
        CAST(DATEADD(DAY, 10, SYSUTCDATETIME()) AS DATE),
        1
    FROM dbo.SCItem i
    JOIN dbo.SCWarehouse w ON w.CompanyID = 1
    WHERE i.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCSupplyRecord WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCSupplyRecord
    (
        CompanyID, SupplyDate, ItemID, WarehouseID, OrganisationUnitID, SupplierID,
        SupplySourceCode, SupplyDocumentType, SupplyDocumentNo,
        ExpectedDate, ReceivedDate, Quantity, ReceivedQty, SupplyStatusCode, CreatedBy
    )
    SELECT
        1,
        CAST(DATEADD(DAY, -7, SYSUTCDATETIME()) AS DATE),
        i.ItemID,
        w.WarehouseID,
        w.OrganisationUnitID,
        NULL,
        'PROCUREMENT',
        'PO',
        CONCAT('PO-', FORMAT(ABS(CHECKSUM(NEWID())) % 99999, '00000')),
        CAST(DATEADD(DAY, CASE WHEN w.WarehouseCode = 'WH-NOR' THEN -2 ELSE 3 END, SYSUTCDATETIME()) AS DATE),
        NULL,
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 200 ELSE 120 END,
        CASE WHEN w.WarehouseCode = 'WH-CEN' THEN 80 ELSE 0 END,
        CASE WHEN w.WarehouseCode = 'WH-CEN' THEN 'PARTIAL' ELSE 'INTRANSIT' END,
        1
    FROM dbo.SCItem i
    JOIN dbo.SCWarehouse w ON w.CompanyID = 1
    WHERE i.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCFulfillmentRecord WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCFulfillmentRecord
    (
        CompanyID, ItemID, WarehouseID, OrganisationUnitID, OrderTypeCode, OrderNo, OrderDate,
        PromiseDate, ActualShipDate, OrderedQty, FulfilledQty, BackOrderQty, FillRatePct, OnTimeFlag, StatusCode
    )
    SELECT
        1,
        i.ItemID,
        w.WarehouseID,
        w.OrganisationUnitID,
        'SALES',
        CONCAT('SO-', FORMAT(ABS(CHECKSUM(NEWID())) % 99999, '00000')),
        CAST(DATEADD(DAY, -14, SYSUTCDATETIME()) AS DATE),
        CAST(DATEADD(DAY, -7, SYSUTCDATETIME()) AS DATE),
        CAST(DATEADD(DAY, -5, SYSUTCDATETIME()) AS DATE),
        100,
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 85 ELSE 70 END,
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 15 ELSE 30 END,
        CASE WHEN i.ItemCode = 'SKU-1001' THEN 85 ELSE 70 END,
        1,
        'CLOSED'
    FROM dbo.SCItem i
    JOIN dbo.SCWarehouse w ON w.CompanyID = 1
    WHERE i.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCInventoryCount WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCInventoryCount
    (
        CompanyID, CountNo, CountDate, WarehouseID, StorageLocationID, ItemID, SystemQty,
        CountedQty, VarianceValue, ApprovalStatusCode, ApprovedBy, ApprovedAt, CreatedBy
    )
    SELECT
        1,
        CONCAT('CNT-', FORMAT(ABS(CHECKSUM(NEWID())) % 99999, '00000')),
        CAST(DATEADD(DAY, -5, SYSUTCDATETIME()) AS DATE),
        b.WarehouseID,
        b.StorageLocationID,
        b.ItemID,
        b.OnHandQty,
        b.OnHandQty - CASE WHEN i.ItemCode = 'SKU-1002' THEN 3 ELSE 0 END,
        CASE WHEN i.ItemCode = 'SKU-1002' THEN -36.6 ELSE 0 END,
        'APPROVED',
        1,
        SYSUTCDATETIME(),
        1
    FROM dbo.SCInventoryBalance b
    JOIN dbo.SCItem i ON i.ItemID = b.ItemID
    WHERE b.CompanyID = 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCAlert WHERE CompanyID = 1)
BEGIN
    INSERT INTO dbo.SCAlert
    (
        CompanyID, AlertCode, AlertSeverityCode, AlertTitle, AlertMessage,
        ItemID, WarehouseID, OrganisationUnitID, TriggeredAt, IsAcknowledged
    )
    SELECT
        1,
        'LOW_STOCK',
        'HIGH',
        CONCAT('Low stock: ', i.ItemCode),
        CONCAT('Available stock below reorder point for ', i.ItemName, ' in ', w.WarehouseName),
        b.ItemID,
        b.WarehouseID,
        w.OrganisationUnitID,
        SYSUTCDATETIME(),
        0
    FROM dbo.SCInventoryBalance b
    JOIN dbo.SCItem i ON i.ItemID = b.ItemID
    JOIN dbo.SCWarehouse w ON w.WarehouseID = b.WarehouseID
    WHERE b.CompanyID = 1
      AND (b.OnHandQty - b.ReservedQty - b.CommittedQty) <= i.ReorderPointQty;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SCDashboardConfig WHERE CompanyID = 1 AND DashboardCode = 'SC_EXEC_HOME')
BEGIN
    INSERT INTO dbo.SCDashboardConfig (CompanyID, DashboardCode, DashboardName, ConfigJson, CreatedBy)
    VALUES
    (1, 'SC_EXEC_HOME', 'Supply Chain Executive Home',
    N'{"cards":["TotalStockOnHand","TotalStockValue","LowStockItems","OutOfStockItems","ReorderDue","FillRate"],"widgets":["ReplenishmentDue","SupplyDemandGap","InventoryAging","OrgScorecard","Alerts","RecentMovements"]}', 1);
END
GO
