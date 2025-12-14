using System;
using System.Data;
using System.Globalization;
using System.Linq;
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
    public class RecoveryStrategyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBRecoveryStrategySP _storedProcedure;
        private readonly Common _common;
        private readonly User _currentUser;
        private readonly ILogger<RecoveryStrategyController> _logger;

        public RecoveryStrategyController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RecoveryStrategyController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBRecoveryStrategySP(_configuration);
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
                    ? "rtvRecoveryStrategies"
                    : "rtvRecoveryStrategiesByUnit";

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
                _logger.LogError(ex, "Error retrieving recovery strategies");
                return BadRequest("Error retrieving recovery strategies");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(long recoveryStrategyID)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvRecoveryStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    RecoveryStrategyID: recoveryStrategyID.ToString());

                if (result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return NotFound();
                }

                var strategyRow = result.Tables[0].Rows[0];
                var strategy = new RecoveryStrategy
                {
                    RecoveryStrategyID = strategyRow["RecoveryStrategyID"].ToString(),
                    CompanyID = strategyRow["CompanyID"].ToString(),
                    UnitID = strategyRow["UnitID"].ToString(),
                    FailureScenario = strategyRow["FailureScenario"].ToString() ?? string.Empty,
                    Strategy = strategyRow["Strategy"].ToString() ?? string.Empty,
                    ConfidenceLevel = strategyRow["ConfidenceLevel"].ToString(),
                    CreatedAt = strategyRow["CreatedAt"].ToString(),
                    UpdatedAt = strategyRow["UpdatedAt"].ToString(),
                    CreatedBy = strategyRow["CreatedBy"].ToString(),
                    ModifiedBy = strategyRow["ModifiedBy"].ToString()
                };

                if (decimal.TryParse(strategyRow["CostImpact"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var costImpact))
                {
                    strategy.CostImpact = costImpact;
                }

                if (result.Tables.Count > 1)
                {
                    strategy.Steps = result.Tables[1].AsEnumerable()
                        .Select(step => new RecoveryStrategyStep
                        {
                            StepID = step.Field<int?>("StepID"),
                            RecoveryStrategyID = step.Field<int?>("RecoveryStrategyID"),
                            StepOrder = step.Field<int?>("StepOrder") ?? 0,
                            StepDescription = step.Field<string>("StepDescription") ?? string.Empty,
                            ValidationCheck = step.Field<string>("ValidationCheck")
                        })
                        .OrderBy(s => s.StepOrder)
                        .ToList();
                }

                return Ok(strategy);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving recovery strategy");
                return BadRequest("Error retrieving recovery strategy");
            }
        }

        [HttpPost]
        public async Task<object> Save(RecoveryStrategy model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                var stepsJson = JsonConvert.SerializeObject(model.Steps ?? new());

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveRecoveryStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: model.UnitID,
                    RecoveryStrategyID: model.RecoveryStrategyID,
                    FailureScenario: model.FailureScenario,
                    Strategy: model.Strategy,
                    CostImpact: model.CostImpact?.ToString(CultureInfo.InvariantCulture),
                    ConfidenceLevel: model.ConfidenceLevel,
                    StepsJson: stepsJson,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving recovery strategy");
                return BadRequest("Error saving recovery strategy");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(RecoveryStrategy model)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRecoveryStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    RecoveryStrategyID: model.RecoveryStrategyID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting recovery strategy");
                return BadRequest("Error deleting recovery strategy");
            }
        }
    }
}
