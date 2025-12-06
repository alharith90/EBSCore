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
    public class AuditUniverseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBAuditUniverseSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<AuditUniverseController> _logger;

        public AuditUniverseController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuditUniverseController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBAuditUniverseSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditUniverse",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit universe");
                return BadRequest("Error retrieving audit universe");
            }
        }

        [HttpGet]
        public object GetOne(long entityProcessId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditUniverseItem",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    EntityProcessID: entityProcessId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit universe item");
                return BadRequest("Error retrieving audit universe item");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object auditUniverse)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveAuditUniverse",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SerializedObject: auditUniverse);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit universe");
                return BadRequest("Error saving audit universe");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long entityProcessId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteAuditUniverse",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    EntityProcessID: entityProcessId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audit universe");
                return BadRequest("Error deleting audit universe");
            }
        }
    }
}
