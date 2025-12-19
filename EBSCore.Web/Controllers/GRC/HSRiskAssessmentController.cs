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
    public class HSRiskAssessmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBHSRiskAssessmentSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<HSRiskAssessmentController> _logger;

        public HSRiskAssessmentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<HSRiskAssessmentController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBHSRiskAssessmentSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvHSRisks",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving HS risks");
                return BadRequest("Error retrieving HS risks");
            }
        }

        [HttpGet]
        public object GetOne(long hazardId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvHSRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    HazardID: hazardId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving HS risk");
                return BadRequest("Error retrieving HS risk");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] object risk)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveHSRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SerializedObject: risk);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HS risk");
                return BadRequest("Error saving HS risk");
            }
        }

        [HttpDelete]
        public object Delete(long hazardId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "DeleteHSRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    HazardID: hazardId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting HS risk");
                return BadRequest("Error deleting HS risk");
            }
        }
    }
}
