using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBSupplyChainInventorySP : DBParentStoredProcedureClass
    {
        public DBSupplyChainInventorySP(IConfiguration configuration)
            : base("SupplyChainInventorySP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? OrganisationUnitID = null,
            bool? IncludeChildren = null,
            int? WarehouseID = null,
            int? StorageLocationID = null,
            int? ItemID = null,
            int? SupplierID = null,
            string StartDate = null,
            string EndDate = null,
            string PeriodCode = null,
            string StockStatusCode = null,
            string TransactionTypeCode = null,
            string DemandSourceCode = null,
            string SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                OrganisationUnitID,
                IncludeChildren,
                WarehouseID,
                StorageLocationID,
                ItemID,
                SupplierID,
                StartDate,
                EndDate,
                PeriodCode,
                StockStatusCode,
                TransactionTypeCode,
                DemandSourceCode,
                SerializedObject
            });
        }
    }
}
