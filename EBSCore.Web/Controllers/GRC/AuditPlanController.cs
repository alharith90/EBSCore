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
    public class AuditPlanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBAuditPlanSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<AuditPlanController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public AuditPlanController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuditPlanController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBAuditPlanSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditPlans",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit plans");
                return BadRequest("Error retrieving audit plans");
            }
        }

        [HttpGet]
        public object GetOne(long auditId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAuditPlan",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AuditID: auditId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit plan");
                return BadRequest("Error retrieving audit plan");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object auditPlan)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveAuditPlan",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: auditPlan);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit plan");
                return BadRequest("Error saving audit plan");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long auditId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteAuditPlan",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AuditID: auditId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting audit plan");
                return BadRequest("Error deleting audit plan");
            }
        }
    }
}
