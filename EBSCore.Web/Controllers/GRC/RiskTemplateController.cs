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
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
