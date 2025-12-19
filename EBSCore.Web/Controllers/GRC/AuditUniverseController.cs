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

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

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
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

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
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
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
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
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
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
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
