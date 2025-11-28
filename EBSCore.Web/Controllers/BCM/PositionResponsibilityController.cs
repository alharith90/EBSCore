using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.BCM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.BCM
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PositionResponsibilityController : ControllerBase
    {
        private readonly DBBcmPositionResponsibilitySP _sp;
        private readonly User _currentUser;
        private readonly ILogger<PositionResponsibilityController> _logger;

        public PositionResponsibilityController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<PositionResponsibilityController> logger)
        {
            _sp = new DBBcmPositionResponsibilitySP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet ds = (DataSet)_sp.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPositionsResponsibilities",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving positions and responsibilities");
                return BadRequest("Error retrieving positions and responsibilities");
            }
        }

        [HttpGet]
        public object GetByUnit(long? UnitID = null)
        {
            try
            {
                DataSet ds = (DataSet)_sp.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPositionsResponsibilitiesByUnit",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    UnitID: UnitID?.ToString());

                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving positions and responsibilities");
                return BadRequest("Error retrieving positions and responsibilities");
            }
        }

        [HttpGet]
        public object GetOne(long PositionID)
        {
            try
            {
                DataSet ds = (DataSet)_sp.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPositionResponsibility",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    PositionID: PositionID.ToString());

                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving position record");
                return BadRequest("Error retrieving position record");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] BcmPositionResponsibility position)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(position.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                _sp.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SavePositionResponsibility",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: position.UnitID,
                    PositionID: position.PositionID,
                    PositionTitle: position.PositionTitle,
                    PositionCode: position.PositionCode,
                    Responsibilities: position.Responsibilities,
                    AuthorityLevel: position.AuthorityLevel,
                    ContactDetails: position.ContactDetails,
                    Status: position.Status,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving position and responsibility");
                return BadRequest("Error saving position and responsibility");
            }
        }

        [HttpDelete]
        public object Delete([FromBody] BcmPositionResponsibility position)
        {
            try
            {
                _sp.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeletePositionResponsibility",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    PositionID: position.PositionID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting position and responsibility");
                return BadRequest("Error deleting position and responsibility");
            }
        }
    }
}
