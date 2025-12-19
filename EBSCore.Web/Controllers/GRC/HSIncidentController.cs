using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
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
    public class HSIncidentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBHSIncidentSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<HSIncidentController> _logger;

        public HSIncidentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<HSIncidentController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBHSIncidentSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvHSIncidents",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving HS incidents");
                return BadRequest("Error retrieving HS incidents");
            }
        }

        [HttpGet]
        public object GetOne(long incidentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvHSIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IncidentID: incidentId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving HS incident");
                return BadRequest("Error retrieving HS incident");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object incident)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveHSIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SerializedObject: incident);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HS incident");
                return BadRequest("Error saving HS incident");
            }
        }

        [HttpDelete]
        public object Delete(long incidentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "DeleteHSIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IncidentID: incidentId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting HS incident");
                return BadRequest("Error deleting HS incident");
            }
        }
    }
}
