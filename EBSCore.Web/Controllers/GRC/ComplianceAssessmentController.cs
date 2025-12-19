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
    public class ComplianceAssessmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBComplianceAssessmentSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ComplianceAssessmentController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public ComplianceAssessmentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ComplianceAssessmentController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBComplianceAssessmentSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvComplianceAssessments",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance assessments");
                return BadRequest("Error retrieving compliance assessments");
            }
        }

        [HttpGet]
        public object GetOne(long assessmentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvComplianceAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AssessmentID: assessmentId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance assessment");
                return BadRequest("Error retrieving compliance assessment");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object assessment)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveComplianceAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: assessment);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving compliance assessment");
                return BadRequest("Error saving compliance assessment");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long assessmentId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteComplianceAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AssessmentID: assessmentId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compliance assessment");
                return BadRequest("Error deleting compliance assessment");
            }
        }
    }
}
