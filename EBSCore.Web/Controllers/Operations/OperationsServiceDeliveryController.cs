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
using System.Linq;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.Operations
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OperationsServiceDeliveryController : ControllerBase
    {
        private readonly DBOperationsServiceDeliverySP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<OperationsServiceDeliveryController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public OperationsServiceDeliveryController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<OperationsServiceDeliveryController> logger)
        {
            _storedProcedure = new DBOperationsServiceDeliverySP(configuration);
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpPost]
        public async Task<object> GetDashboard([FromBody] OperationsDashboardFilter filter)
        {
            try
            {
                var dataSet = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvOperationsDashboard",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    OrganisationUnitID: filter?.OrganisationUnitID,
                    IncludeChildren: filter?.IncludeChildren,
                    StartDate: filter?.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate: filter?.EndDate?.ToString("yyyy-MM-dd"),
                    AssignmentGroupID: filter?.AssignmentGroupID,
                    AssignedUserID: filter?.AssignedUserID,
                    PriorityCode: filter?.PriorityCode,
                    StatusCode: filter?.StatusCode);

                var response = new OperationsDashboardResponse
                {
                    Kpis = DeserializeTable<OperationsKpi>(dataSet, 0),
                    QueueBacklog = DeserializeTable<QueueBacklogRow>(dataSet, 1),
                    OldestOpenServiceRequests = DeserializeTable<ServiceRequest>(dataSet, 2),
                    CriticalIncidents = DeserializeTable<OperationsIncident>(dataSet, 3),
                    OverdueWorkOrders = DeserializeTable<OperationsWorkOrder>(dataSet, 4)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operations dashboard");
                return BadRequest("Error retrieving operations dashboard");
            }
        }

        [HttpGet]
        public object GetServiceRequests()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvServiceRequests",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving service requests");
                return BadRequest("Error retrieving service requests");
            }
        }

        [HttpPost]
        public object SaveServiceRequest([FromBody] object serviceRequest)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveServiceRequest",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: serviceRequest?.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving service request");
                return BadRequest("Error saving service request");
            }
        }

        [HttpGet]
        public object GetIncidents()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvOperationsIncidents",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operations incidents");
                return BadRequest("Error retrieving operations incidents");
            }
        }

        [HttpGet]
        public object GetWorkOrders()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvOperationsWorkOrders",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving operations work orders");
                return BadRequest("Error retrieving operations work orders");
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
