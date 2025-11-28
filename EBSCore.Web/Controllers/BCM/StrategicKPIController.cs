using System;
using System.Data;
using System.Threading.Tasks;
using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.BCM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StrategicKPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBStrategicKPISP _storedProcedure;
        private readonly Common _common;
        private readonly User _currentUser;
        private readonly ILogger<StrategicKPIController> _logger;

        public StrategicKPIController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StrategicKPIController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBStrategicKPISP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _common = new Common();
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategicKPIs",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving Strategic KPIs");
                return BadRequest("Error retrieving Strategic KPIs");
            }
        }

        [HttpGet]
        public async Task<object> GetByUnit(long? UnitID = null)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategicKPIsByUnit",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: UnitID?.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving Strategic KPIs");
                return BadRequest("Error retrieving Strategic KPIs");
            }
        }

        [HttpGet]
        public object GetOne(long kpiID)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategicKPI",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    KPIID: kpiID.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving Strategic KPI");
                return BadRequest("Error retrieving Strategic KPI");
            }
        }

        [HttpPost]
        public async Task<object> Save(StrategicKPI kpi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(kpi.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                if (string.IsNullOrWhiteSpace(kpi.Title))
                {
                    throw new Exception("Title is required");
                }

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveStrategicKPI",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: kpi.UnitID,
                    KPIID: kpi.KPIID,
                    ObjectiveID: kpi.ObjectiveID,
                    Title: kpi.Title,
                    Description: kpi.Description,
                    TargetValue: kpi.TargetValue?.ToString(),
                    CurrentValue: kpi.CurrentValue?.ToString(),
                    Unit: kpi.Unit,
                    Frequency: kpi.Frequency,
                    Status: kpi.Status,
                    Owner: kpi.Owner,
                    EscalationPlan: kpi.EscalationPlan,
                    ActivationCriteria: kpi.ActivationCriteria,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving Strategic KPI");
                return BadRequest("Error saving Strategic KPI");
            }
        }

        [HttpDelete]
        public object Delete(StrategicKPI kpi)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteStrategicKPI",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    KPIID: kpi.KPIID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting Strategic KPI");
                return BadRequest("Error deleting Strategic KPI");
            }
        }
    }
}
