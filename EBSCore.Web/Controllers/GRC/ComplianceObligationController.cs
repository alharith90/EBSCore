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
    public class ComplianceObligationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBComplianceObligationSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ComplianceObligationController> _logger;

        public ComplianceObligationController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ComplianceObligationController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBComplianceObligationSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvObligations",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance obligations");
                return BadRequest("Error retrieving compliance obligations");
            }
        }

        [HttpGet]
        public object GetOne(long obligationId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvObligation",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObligationID: obligationId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving compliance obligation");
                return BadRequest("Error retrieving compliance obligation");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object obligation)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveObligation",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SerializedObject: obligation);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving compliance obligation");
                return BadRequest("Error saving compliance obligation");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long obligationId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteObligation",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObligationID: obligationId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compliance obligation");
                return BadRequest("Error deleting compliance obligation");
            }
        }
    }
}
