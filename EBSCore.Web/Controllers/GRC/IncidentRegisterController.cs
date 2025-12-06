using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.GRC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IncidentRegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBIncidentRegisterSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<IncidentRegisterController> _logger;

        public IncidentRegisterController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<IncidentRegisterController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBIncidentRegisterSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvIncidents",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incidents");
                return BadRequest("Error retrieving incidents");
            }
        }

        [HttpGet]
        public object GetOne(long incidentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IncidentID: incidentId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident");
                return BadRequest("Error retrieving incident");
            }
        }

        [HttpPost]
        public async Task<object> Save(IncidentRegister incident)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IncidentID: incident.IncidentID,
                    IncidentDescription: incident.IncidentDescription,
                    IncidentDate: incident.IncidentDate,
                    ImpactedArea: incident.ImpactedArea,
                    Severity: incident.Severity,
                    RootCause: incident.RootCause,
                    ActionsTaken: incident.ActionsTaken,
                    IncidentOwner: incident.IncidentOwner,
                    Status: incident.Status,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving incident");
                return BadRequest("Error saving incident");
            }
        }

        [HttpDelete]
        public object Delete(IncidentRegister incident)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IncidentID: incident.IncidentID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident");
                return BadRequest("Error deleting incident");
            }
        }
    }
}
