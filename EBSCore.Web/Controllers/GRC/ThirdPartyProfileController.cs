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
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
