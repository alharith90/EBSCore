using System;
using System.Collections.Generic;

namespace EBSCore.Web.Models.Operations
{
    public class SupplyChainDashboardFilter
    {
        public int? OrganisationUnitID { get; set; }
        public bool IncludeChildren { get; set; }
        public int? WarehouseID { get; set; }
        public int? StorageLocationID { get; set; }
        public int? ItemID { get; set; }
        public int? SupplierID { get; set; }
        public string PeriodCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ItemMaster
    {
        public int? ItemID { get; set; }
        public int? CompanyID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? ItemCategoryID { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCategoryName { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public string StockTypeCode { get; set; }
        public string ItemStatusCode { get; set; }
        public string ReplenishmentMethodCode { get; set; }
        public int? PreferredSupplierID { get; set; }
        public int? LeadTimeProfileID { get; set; }
        public decimal? SafetyStockQty { get; set; }
        public decimal? MinStockQty { get; set; }
        public decimal? MaxStockQty { get; set; }
        public decimal? ReorderPointQty { get; set; }
        public decimal? ReorderQty { get; set; }
        public string ABCClassCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class InventoryBalanceRow
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public int? StorageLocationID { get; set; }
        public string StorageLocationCode { get; set; }
        public decimal OnHandQty { get; set; }
        public decimal ReservedQty { get; set; }
        public decimal InTransitQty { get; set; }
        public decimal CommittedQty { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal StockValue { get; set; }
        public string StockStatusCode { get; set; }
    }

    public class InventoryTransactionRow
    {
        public long InventoryTransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string MovementTypeCode { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? WarehouseID { get; set; }
        public int? StorageLocationID { get; set; }
        public int? ToWarehouseID { get; set; }
        public int? ToStorageLocationID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string RelatedDocType { get; set; }
        public string RelatedDocNo { get; set; }
        public string ApprovalStatusCode { get; set; }
        public string ReasonCode { get; set; }
    }

    public class ReplenishmentDueRow
    {
        public int ReplenishmentRuleID { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal MinStockQty { get; set; }
        public decimal ReorderPointQty { get; set; }
        public decimal SuggestedReplenishmentQty { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string SupplierName { get; set; }
        public int DelayDays { get; set; }
    }

    public class SupplyDemandGapRow
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal DemandQty { get; set; }
        public decimal SupplyQty { get; set; }
        public decimal GapQty { get; set; }
        public string GapStatus { get; set; }
    }

    public class InventoryAgingRow
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public decimal Age0To30Qty { get; set; }
        public decimal Age31To60Qty { get; set; }
        public decimal Age61To90Qty { get; set; }
        public decimal Age91To180Qty { get; set; }
        public decimal Age181To365Qty { get; set; }
        public decimal Age365PlusQty { get; set; }
        public decimal SlowMovingQty { get; set; }
        public decimal ObsoleteQty { get; set; }
    }

    public class OrganisationSupplyScorecardRow
    {
        public int OrganisationUnitID { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int ItemCount { get; set; }
        public decimal OnHandQty { get; set; }
        public int LowStockCount { get; set; }
        public int StockOutCount { get; set; }
        public int ReorderDueCount { get; set; }
        public int DelayedReplenishmentCount { get; set; }
        public decimal FillRatePct { get; set; }
        public decimal InventoryTurnover { get; set; }
        public decimal AgingExposurePct { get; set; }
        public decimal SupplyHealthScore { get; set; }
    }

    public class SupplyChainAlertRow
    {
        public long AlertID { get; set; }
        public string AlertCode { get; set; }
        public string AlertSeverityCode { get; set; }
        public string AlertTitle { get; set; }
        public string AlertMessage { get; set; }
        public int? ItemID { get; set; }
        public int? WarehouseID { get; set; }
        public int? OrganisationUnitID { get; set; }
        public DateTime TriggeredAt { get; set; }
        public bool IsAcknowledged { get; set; }
    }

    public class SupplyChainKpi
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
        public decimal? PreviousValue { get; set; }
        public string Trend { get; set; }
    }

    public class SupplyChainDashboardResponse
    {
        public List<SupplyChainKpi> Kpis { get; set; } = new();
        public List<InventoryBalanceRow> InventoryBalances { get; set; } = new();
        public List<ReplenishmentDueRow> ReplenishmentDue { get; set; } = new();
        public List<SupplyDemandGapRow> SupplyDemandGaps { get; set; } = new();
        public List<InventoryAgingRow> InventoryAging { get; set; } = new();
        public List<OrganisationSupplyScorecardRow> OrganisationScorecards { get; set; } = new();
        public List<SupplyChainAlertRow> Alerts { get; set; } = new();
        public List<InventoryTransactionRow> RecentMovements { get; set; } = new();
    }
}
