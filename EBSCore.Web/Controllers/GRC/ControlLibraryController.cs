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
    public class ControlLibraryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBControlLibrarySP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ControlLibraryController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public ControlLibraryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ControlLibraryController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBControlLibrarySP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvControls",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving controls");
                return BadRequest("Error retrieving controls");
            }
        }

        [HttpGet]
        public object GetOne(long controlId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvControl",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    ControlID: controlId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving control");
                return BadRequest("Error retrieving control");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object control)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveControl",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    SerializedObject: control);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving control");
                return BadRequest("Error saving control");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long controlId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteControl",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    ControlID: controlId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting control");
                return BadRequest("Error deleting control");
            }
        }
    }
}
