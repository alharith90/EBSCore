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
    public class BCPPlanController : ControllerBase
    {
        private readonly DBBCPPlanSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<BCPPlanController> _logger;

        public BCPPlanController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<BCPPlanController> logger)
        {
            _storedProcedure = new DBBCPPlanSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvBCPPlans",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving BCP plans");
                return BadRequest("Error retrieving BCP plans");
            }
        }

        [HttpGet]
        public object GetOne(long bcpId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvBCPPlan",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    BCPID: bcpId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving BCP plan");
                return BadRequest("Error retrieving BCP plan");
            }
        }

        [HttpPost]
        public async Task<object> Save(BusinessContinuityPlan plan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveBCPPlan",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    BCPID: plan.BCPID,
                    PlanName: plan.PlanName,
                    Scope: plan.Scope,
                    RecoveryTeamRoles: plan.RecoveryTeamRoles,
                    ContactList: plan.ContactList,
                    InvocationCriteria: plan.InvocationCriteria,
                    RecoveryLocations: plan.RecoveryLocations,
                    RecoveryStrategyDetails: plan.RecoveryStrategyDetails,
                    KeySteps: plan.KeySteps,
                    RequiredResources: plan.RequiredResources,
                    DependentSystems: plan.DependentSystems,
                    PlanRTO: plan.PlanRTO,
                    PlanRPO: plan.PlanRPO,
                    BackupSource: plan.BackupSource,
                    AlternateSupplier: plan.AlternateSupplier,
                    LastTestDate: plan.LastTestDate?.ToString("o"),
                    TestResultSummary: plan.TestResultSummary,
                    PlanOwner: plan.PlanOwner,
                    PlanStatusID: plan.PlanStatusID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving BCP plan");
                return BadRequest("Error saving BCP plan");
            }
        }

        [HttpDelete]
        public object Delete(BusinessContinuityPlan plan)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteBCPPlan",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    BCPID: plan.BCPID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting BCP plan");
                return BadRequest("Error deleting BCP plan");
            }
        }
    }
}
