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
    public class AuditFindingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBAuditFindingSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<AuditFindingController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public AuditFindingController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuditFindingController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBAuditFindingSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditFindings",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit findings");
                return BadRequest("Error retrieving audit findings");
            }
        }

        [HttpGet]
        public object GetOne(long findingId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditFinding",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    FindingID: findingId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit finding");
                return BadRequest("Error retrieving audit finding");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object auditFinding)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveAuditFinding",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: auditFinding);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit finding");
                return BadRequest("Error saving audit finding");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long findingId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteAuditFinding",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    FindingID: findingId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audit finding");
                return BadRequest("Error deleting audit finding");
            }
        }
    }
}
