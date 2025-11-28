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

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StrategicObjectiveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBStrategicObjectiveSP _storedProcedure;
        private readonly Common _common;
        private readonly User _currentUser;
        private readonly ILogger<StrategicObjectiveController> _logger;

        public StrategicObjectiveController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StrategicObjectiveController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBStrategicObjectiveSP(_configuration);
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
                    ? "rtvStrategicObjectives"
                    : "rtvStrategicObjectivesByUnit";

                DataSet result = (DataSet)_storedProcedure.QueryDatabase(
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
                _logger.LogError(ex, "Error retrieving strategic objectives");
                return BadRequest("Error retrieving strategic objectives");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(long objectiveID)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategicObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objectiveID.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving strategic objective");
                return BadRequest("Error retrieving strategic objective");
            }
        }

        [HttpPost]
        public async Task<object> Save(StrategicObjective objective)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(objective.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                if (string.IsNullOrWhiteSpace(objective.Title))
                {
                    throw new Exception("Title is required");
                }

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveStrategicObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: objective.UnitID,
                    ObjectiveID: objective.ObjectiveID,
                    ObjectiveCode: objective.ObjectiveCode,
                    Strategy: objective.Strategy,
                    Title: objective.Title,
                    Description: objective.Description,
                    OwnerID: objective.OwnerID,
                    RiskLink: objective.RiskLink,
                    ComplianceLink: objective.ComplianceLink,
                    Status: objective.Status,
                    StartDate: objective.StartDate,
                    EndDate: objective.EndDate,
                    EscalationLevel: objective.EscalationLevel,
                    EscalationContact: objective.EscalationContact,
                    ActivationCriteria: objective.ActivationCriteria,
                    ActivationStatus: objective.ActivationStatus,
                    ActivationTrigger: objective.ActivationTrigger,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving strategic objective");
                return BadRequest("Error saving strategic objective");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(StrategicObjective objective)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteStrategicObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objective.ObjectiveID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting strategic objective");
                return BadRequest("Error deleting strategic objective");
            }
        }
    }
}
