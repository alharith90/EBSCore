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
    public class RiskRegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBRiskRegisterSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<RiskRegisterController> _logger;

        public RiskRegisterController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RiskRegisterController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBRiskRegisterSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRisks",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risks");
                return BadRequest("Error retrieving risks");
            }
        }

        [HttpGet]
        public object GetOne(long riskId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    RiskID: riskId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving risk");
                return BadRequest("Error retrieving risk");
            }
        }

        [HttpPost]
        public async Task<object> Save(RiskRegister risk)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    RiskID: risk.RiskID,
                    RiskTitle: risk.RiskTitle,
                    RiskDescription: risk.RiskDescription,
                    RiskCategory: risk.RiskCategory,
                    RiskSource: risk.RiskSource,
                    PotentialImpact: risk.PotentialImpact,
                    InherentLikelihood: risk.InherentLikelihood,
                    InherentImpact: risk.InherentImpact,
                    InherentRiskLevel: risk.InherentRiskLevel,
                    ExistingControls: risk.ExistingControls,
                    ControlEffectiveness: risk.ControlEffectiveness,
                    ResidualLikelihood: risk.ResidualLikelihood,
                    ResidualImpact: risk.ResidualImpact,
                    ResidualRiskLevel: risk.ResidualRiskLevel,
                    RiskAppetiteThreshold: risk.RiskAppetiteThreshold,
                    RiskResponseStrategy: risk.RiskResponseStrategy,
                    TreatmentDecision: risk.TreatmentDecision,
                    TreatmentPlanID: risk.TreatmentPlanID,
                    Likelihood: risk.Likelihood,
                    Impact: risk.Impact,
                    RiskScore: risk.RiskScore?.ToString(),
                    RiskResponse: risk.RiskResponse,
                    RiskOwner: risk.RiskOwner,
                    Status: risk.Status,
                    ReviewDate: risk.ReviewDate,
                    NextReviewDate: risk.NextReviewDate,
                    RiskTrend: risk.RiskTrend,
                    RelatedObjectives: risk.RelatedObjectives,
                    RelatedIncidents: risk.RelatedIncidents,
                    RelatedControls: risk.RelatedControls,
                    RelatedObligations: risk.RelatedObligations,
                    MonitoringFrequency: risk.MonitoringFrequency,
                    LastMonitoringDate: risk.LastMonitoringDate,
                    KRIName: risk.KRIName,
                    KRIValue: risk.KRIValue,
                    KRIStatus: risk.KRIStatus,
                    RiskAlertTrigger: risk.RiskAlertTrigger,
                    NextMonitoringDate: risk.NextMonitoringDate,
                    RiskHistoryNotes: risk.RiskHistoryNotes,
                    LastUpdatedBy: _currentUser.UserFullName,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving risk");
                return BadRequest("Error saving risk");
            }
        }

        [HttpDelete]
        public object Delete(RiskRegister risk)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRisk",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    RiskID: risk.RiskID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk");
                return BadRequest("Error deleting risk");
            }
        }
    }
}
