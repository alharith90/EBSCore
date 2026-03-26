using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.Operations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.Operations
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SupplyChainInventoryController : ControllerBase
    {
        private readonly DBSupplyChainInventorySP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<SupplyChainInventoryController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public SupplyChainInventoryController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SupplyChainInventoryController> logger)
        {
            _storedProcedure = new DBSupplyChainInventorySP(configuration);
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpPost]
        public object GetDashboard([FromBody] SupplyChainDashboardFilter filter)
        {
            try
            {
                var dataSet = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvSupplyChainDashboard",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    OrganisationUnitID: filter?.OrganisationUnitID,
                    IncludeChildren: filter?.IncludeChildren,
                    WarehouseID: filter?.WarehouseID,
                    StorageLocationID: filter?.StorageLocationID,
                    ItemID: filter?.ItemID,
                    SupplierID: filter?.SupplierID,
                    StartDate: filter?.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate: filter?.EndDate?.ToString("yyyy-MM-dd"),
                    PeriodCode: filter?.PeriodCode);

                var response = new SupplyChainDashboardResponse
                {
                    Kpis = DeserializeTable<SupplyChainKpi>(dataSet, 0),
                    InventoryBalances = DeserializeTable<InventoryBalanceRow>(dataSet, 1),
                    ReplenishmentDue = DeserializeTable<ReplenishmentDueRow>(dataSet, 2),
                    SupplyDemandGaps = DeserializeTable<SupplyDemandGapRow>(dataSet, 3),
                    InventoryAging = DeserializeTable<InventoryAgingRow>(dataSet, 4),
                    OrganisationScorecards = DeserializeTable<OrganisationSupplyScorecardRow>(dataSet, 5),
                    Alerts = DeserializeTable<SupplyChainAlertRow>(dataSet, 6),
                    RecentMovements = DeserializeTable<InventoryTransactionRow>(dataSet, 7)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving supply chain dashboard");
                return BadRequest("Error retrieving supply chain dashboard");
            }
        }

        [HttpGet]
        public object GetItems()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvItems",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return BadRequest("Error retrieving items");
            }
        }

        [HttpGet]
        public object GetInventoryTransactions()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvInventoryTransactions",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory transactions");
                return BadRequest("Error retrieving inventory transactions");
            }
        }

        [HttpPost]
        public object SaveItem([FromBody] object item)
        {
            return SavePayload("SaveItem", item, "Error saving item");
        }

        [HttpPost]
        public object SaveInventoryTransaction([FromBody] object transaction)
        {
            return SavePayload("SaveInventoryTransaction", transaction, "Error saving inventory transaction");
        }

        [HttpPost]
        public object SaveReplenishmentRule([FromBody] object rule)
        {
            return SavePayload("SaveReplenishmentRule", rule, "Error saving replenishment rule");
        }

        [HttpPost]
        public object SaveDemandRecord([FromBody] object demand)
        {
            return SavePayload("SaveDemandRecord", demand, "Error saving demand record");
        }

        [HttpPost]
        public object SaveSupplyRecord([FromBody] object supply)
        {
            return SavePayload("SaveSupplyRecord", supply, "Error saving supply record");
        }

        [HttpPost]
        public object SaveInventoryCount([FromBody] object count)
        {
            return SavePayload("SaveInventoryCount", count, "Error saving inventory count");
        }

        private object SavePayload(string operation, object payload, string errorMessage)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: operation,
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId,
                    SerializedObject: payload?.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return BadRequest(errorMessage);
            }
        }

        private static List<T> DeserializeTable<T>(DataSet ds, int index)
        {
            if (ds == null || ds.Tables.Count <= index)
            {
                return new List<T>();
            }

            var json = JsonConvert.SerializeObject(ds.Tables[index]);
            var value = JsonConvert.DeserializeObject<List<T>>(json);
            return value ?? new List<T>();
        }
    }
}
