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
    public class RiskCategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBRiskCategorySP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<RiskCategoryController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public RiskCategoryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RiskCategoryController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBRiskCategorySP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRiskCategories",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk categories");
                return BadRequest("Error retrieving risk categories");
            }
        }

        [HttpGet]
        public object GetOne(int categoryId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRiskCategory",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    CategoryID: categoryId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk category");
                return BadRequest("Error retrieving risk category");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object riskCategory)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveRiskCategory",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: riskCategory);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving risk category");
                return BadRequest("Error saving risk category");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int categoryId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRiskCategory",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    CategoryID: categoryId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk category");
                return BadRequest("Error deleting risk category");
            }
        }
    }
}
