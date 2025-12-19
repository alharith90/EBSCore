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
    public class RiskTemplateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBRiskTemplateSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<RiskTemplateController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public RiskTemplateController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RiskTemplateController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBRiskTemplateSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRiskTemplates",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk templates");
                return BadRequest("Error retrieving risk templates");
            }
        }

        [HttpGet]
        public object GetOne(int templateId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRiskTemplate",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    TemplateID: templateId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk template");
                return BadRequest("Error retrieving risk template");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object riskTemplate)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveRiskTemplate",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: riskTemplate);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving risk template");
                return BadRequest("Error saving risk template");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int templateId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRiskTemplate",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    TemplateID: templateId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk template");
                return BadRequest("Error deleting risk template");
            }
        }
    }
}
