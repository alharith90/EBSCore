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
    public class ThirdPartyProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBThirdPartyProfileSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ThirdPartyProfileController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public ThirdPartyProfileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ThirdPartyProfileController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBThirdPartyProfileSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyProfiles",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party profiles");
                return BadRequest("Error retrieving third party profiles");
            }
        }

        [HttpGet]
        public object GetOne(int thirdPartyId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvThirdPartyProfile",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    ThirdPartyID: thirdPartyId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving third party profile");
                return BadRequest("Error retrieving third party profile");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object thirdPartyProfile)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveThirdPartyProfile",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: thirdPartyProfile);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving third party profile");
                return BadRequest("Error saving third party profile");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int thirdPartyId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteThirdPartyProfile",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    ThirdPartyID: thirdPartyId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting third party profile");
                return BadRequest("Error deleting third party profile");
            }
        }
    }
}
