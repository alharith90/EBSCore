using EBSCore.AdoClass;
using EBSCore.Web.Models;
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
using EBSCore.Web.AppCode;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EnvironmentalObjectiveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBEnvironmentalObjectiveSP _objectiveSP;
        private readonly User _currentUser;
        private readonly ILogger<EnvironmentalObjectiveController> _logger;

        public EnvironmentalObjectiveController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EnvironmentalObjectiveController> logger)
        {
            _configuration = configuration;
            _objectiveSP = new DBEnvironmentalObjectiveSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet dsResult = (DataSet)_objectiveSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvEnvironmentalObjectives",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Environmental Objectives");
                return BadRequest("Error retrieving Environmental Objectives");
            }
        }

        [HttpGet]
        public object GetOne(long objectiveId)
        {
            try
            {
                DataSet dsResult = (DataSet)_objectiveSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvEnvironmentalObjective",
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objectiveId,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Environmental Objective");
                return BadRequest("Error retrieving Environmental Objective");
            }
        }

        [HttpPost]
        public async Task<object> Save(EnvironmentalObjective objective)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _objectiveSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveEnvironmentalObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objective.ObjectiveID,
                    UnitID: objective.UnitID,
                    ObjectiveDescription: objective.ObjectiveDescription,
                    TargetValue: objective.TargetValue,
                    Unit: objective.Unit,
                    BaselineValue: objective.BaselineValue,
                    CurrentValue: objective.CurrentValue,
                    TargetDate: objective.TargetDate,
                    ResponsibleOwner: objective.ResponsibleOwner,
                    Status: objective.Status,
                    CreatedAt: objective.CreatedAt,
                    UpdatedAt: objective.UpdatedAt,
                    UpdatedBy: _currentUser.UserID,
                    CreatedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Environmental Objective");
                return BadRequest("Error saving Environmental Objective");
            }
        }

        [HttpDelete]
        public object Delete(long objectiveId)
        {
            try
            {
                _objectiveSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteEnvironmentalObjective",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ObjectiveID: objectiveId);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Environmental Objective");
                return BadRequest("Error deleting Environmental Objective");
            }
        }
    }
}
