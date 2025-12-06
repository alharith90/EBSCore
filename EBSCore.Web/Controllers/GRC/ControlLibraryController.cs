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
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
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
