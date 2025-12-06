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
    public class ThirdPartyIncidentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBThirdPartyIncidentSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ThirdPartyIncidentController> _logger;

        public ThirdPartyIncidentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ThirdPartyIncidentController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBThirdPartyIncidentSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyIncidents",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party incidents");
                return BadRequest("Error retrieving third party incidents");
            }
        }

        [HttpGet]
        public object GetOne(int issueIncidentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IssueIncidentID: issueIncidentId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party incident");
                return BadRequest("Error retrieving third party incident");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object incident)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveThirdPartyIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SerializedObject: incident);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving third party incident");
                return BadRequest("Error saving third party incident");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int issueIncidentId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteThirdPartyIncident",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IssueIncidentID: issueIncidentId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting third party incident");
                return BadRequest("Error deleting third party incident");
            }
        }
    }
}
