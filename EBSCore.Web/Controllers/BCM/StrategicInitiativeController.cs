using System;
using System.Data;
using System.Threading.Tasks;
using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.BCM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.BCM
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StrategicInitiativeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBStrategicInitiativeSP _storedProcedure;
        private readonly Common _common;
        private readonly User _currentUser;
        private readonly ILogger<StrategicInitiativeController> _logger;

        public StrategicInitiativeController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StrategicInitiativeController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBStrategicInitiativeSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _common = new Common();
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get(string? UnitID = null)
        {
            try
            {
                var operation = string.IsNullOrEmpty(UnitID)
                    ? "rtvStrategicInitiatives"
                    : "rtvStrategicInitiativesByUnit";

                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: operation,
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: UnitID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving strategic initiatives");
                return BadRequest("Error retrieving strategic initiatives");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(long initiativeID)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategicInitiative",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    InitiativeID: initiativeID.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving strategic initiative");
                return BadRequest("Error retrieving strategic initiative");
            }
        }

        [HttpPost]
        public async Task<object> Save(StrategicInitiative initiative)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(initiative.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveStrategicInitiative",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: initiative.UnitID,
                    InitiativeID: initiative.InitiativeID,
                    ObjectiveID: initiative.ObjectiveID,
                    Title: initiative.Title,
                    Description: initiative.Description,
                    OwnerID: initiative.OwnerID,
                    DepartmentID: initiative.DepartmentID,
                    Budget: initiative.Budget?.ToString(),
                    Progress: initiative.Progress?.ToString(),
                    StartDate: initiative.StartDate,
                    EndDate: initiative.EndDate,
                    Status: initiative.Status,
                    EscalationCriteria: initiative.EscalationCriteria,
                    EscalationContact: initiative.EscalationContact,
                    ActivationCriteria: initiative.ActivationCriteria,
                    ActivationTrigger: initiative.ActivationTrigger,
                    ActivationStatus: initiative.ActivationStatus,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving strategic initiative");
                return BadRequest("Error saving strategic initiative");
            }
        }

        [HttpDelete]
        public object Delete(StrategicInitiative initiative)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteStrategicInitiative",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    InitiativeID: initiative.InitiativeID);

                return "[]";
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting strategic initiative");
                return BadRequest("Error deleting strategic initiative");
            }
        }
    }
}
