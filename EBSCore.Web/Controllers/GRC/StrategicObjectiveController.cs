using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.GRC;
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
    public class StrategicObjectiveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBStrategicObjectiveSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<StrategicObjectiveController> _logger;

        public StrategicObjectiveController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<StrategicObjectiveController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBStrategicObjectiveSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvObjectives",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving objectives");
                return BadRequest("Error retrieving objectives");
            }
        }

        [HttpGet]
        public object GetOne(long objectiveId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvObjective",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    ObjectiveID: objectiveId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving objective");
                return BadRequest("Error retrieving objective");
            }
        }

        [HttpPost]
        public async Task<object> Save(StrategicObjective objective)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objective.ObjectiveID,
                    ObjectiveCode: objective.ObjectiveCode,
                    ObjectiveNameEN: objective.ObjectiveNameEN,
                    ObjectiveNameAR: objective.ObjectiveNameAR,
                    DescriptionEN: objective.DescriptionEN,
                    DescriptionAR: objective.DescriptionAR,
                    Category: objective.Category,
                    Perspective: objective.Perspective,
                    OwnerUserID: objective.OwnerUserID,
                    DepartmentID: objective.DepartmentID,
                    StartDate: objective.StartDate?.ToString("o"),
                    EndDate: objective.EndDate?.ToString("o"),
                    TargetValue: objective.TargetValue?.ToString(),
                    UnitEN: objective.UnitEN,
                    UnitAR: objective.UnitAR,
                    RiskAppetiteThreshold: objective.RiskAppetiteThreshold,
                    StatusID: objective.StatusID,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving objective");
                return BadRequest("Error saving objective");
            }
        }

        [HttpDelete]
        public object Delete(StrategicObjective objective)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objective.ObjectiveID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting objective");
                return BadRequest("Error deleting objective");
            }
        }
    }
}
