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
    public class ComplianceIssueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBComplianceIssueSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ComplianceIssueController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public ComplianceIssueController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ComplianceIssueController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBComplianceIssueSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvComplianceIssues",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance issues");
                return BadRequest("Error retrieving compliance issues");
            }
        }

        [HttpGet]
        public object GetOne(long issueId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvComplianceIssue",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    IssueID: issueId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance issue");
                return BadRequest("Error retrieving compliance issue");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object issue)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveComplianceIssue",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: issue);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving compliance issue");
                return BadRequest("Error saving compliance issue");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long issueId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteComplianceIssue",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    IssueID: issueId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compliance issue");
                return BadRequest("Error deleting compliance issue");
            }
        }
    }
}
