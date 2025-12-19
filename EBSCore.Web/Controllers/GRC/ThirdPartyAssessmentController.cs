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
    public class ThirdPartyAssessmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBThirdPartyAssessmentSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ThirdPartyAssessmentController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public ThirdPartyAssessmentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ThirdPartyAssessmentController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBThirdPartyAssessmentSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyAssessments",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party assessments");
                return BadRequest("Error retrieving third party assessments");
            }
        }

        [HttpGet]
        public object GetOne(int assessmentId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AssessmentID: assessmentId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party assessment");
                return BadRequest("Error retrieving third party assessment");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object assessment)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveThirdPartyAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: assessment);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving third party assessment");
                return BadRequest("Error saving third party assessment");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int assessmentId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteThirdPartyAssessment",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    AssessmentID: assessmentId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting third party assessment");
                return BadRequest("Error deleting third party assessment");
            }
        }
    }
}
