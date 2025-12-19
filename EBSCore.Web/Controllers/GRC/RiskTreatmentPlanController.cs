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
    public class RiskTreatmentPlanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBRiskTreatmentPlanSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<RiskTreatmentPlanController> _logger;

        public RiskTreatmentPlanController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RiskTreatmentPlanController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBRiskTreatmentPlanSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPlans",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk treatment plans");
                return BadRequest("Error retrieving risk treatment plans");
            }
        }

        [HttpGet]
        public object GetOne(long actionId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPlan",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ActionID: actionId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk treatment plan");
                return BadRequest("Error retrieving risk treatment plan");
            }
        }

        [HttpPost]
        public async Task<object> Save(RiskTreatmentPlan plan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SavePlan",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ActionID: plan.ActionID?.ToString(),
                    RelatedRisk: plan.RelatedRisk,
                    MitigationAction: plan.MitigationAction,
                    ActionOwner: plan.ActionOwner,
                    DueDate: plan.DueDate?.ToString("o"),
                    CompletionStatus: plan.CompletionStatus,
                    TreatmentType: plan.TreatmentType,
                    AssociatedControl: plan.AssociatedControl,
                    ProgressNotes: plan.ProgressNotes,
                    Verification: plan.Verification,
                    Effectiveness: plan.Effectiveness,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving risk treatment plan");
                return BadRequest("Error saving risk treatment plan");
            }
        }

        [HttpDelete]
        public object Delete(RiskTreatmentPlan plan)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeletePlan",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ActionID: plan.ActionID?.ToString());

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk treatment plan");
                return BadRequest("Error deleting risk treatment plan");
            }
        }
    }
}
