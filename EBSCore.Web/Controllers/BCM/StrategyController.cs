using System;
using System.Data;
using System.Globalization;
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
    public class StrategyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBStrategySP _storedProcedure;
        private readonly Common _common;
        private readonly User _currentUser;
        private readonly ILogger<StrategyController> _logger;

        public StrategyController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StrategyController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBStrategySP(_configuration);
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
                    ? "rtvStrategies"
                    : "rtvStrategiesByUnit";

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
                _logger.LogError(ex, "Error retrieving strategies");
                return BadRequest("Error retrieving strategies");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(long strategyID)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    StrategyID: strategyID.ToString(CultureInfo.InvariantCulture));

                if (result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return NotFound();
                }

                var strategyRow = result.Tables[0].Rows[0];
                var strategy = new Strategy
                {
                    StrategyID = strategyRow["StrategyID"].ToString(),
                    CompanyID = strategyRow["CompanyID"].ToString(),
                    UnitID = strategyRow["UnitID"].ToString(),
                    Title = strategyRow["Title"].ToString(),
                    TitleAr = strategyRow["TitleAr"].ToString(),
                    Vision = strategyRow["Vision"].ToString(),
                    Mission = strategyRow["Mission"].ToString(),
                    EscalationCriteria = strategyRow["EscalationCriteria"].ToString(),
                    ActivationCriteria = strategyRow["ActivationCriteria"].ToString(),
                    Status = strategyRow["Status"].ToString(),
                    CreatedBy = strategyRow["CreatedBy"].ToString(),
                    ModifiedBy = strategyRow["ModifiedBy"].ToString()
                };

                if (int.TryParse(strategyRow["OwnerID"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var ownerId))
                {
                    strategy.OwnerID = ownerId;
                }

                if (DateTime.TryParse(strategyRow["StartDate"].ToString(), out var startDate))
                {
                    strategy.StartDate = startDate;
                }

                if (DateTime.TryParse(strategyRow["EndDate"].ToString(), out var endDate))
                {
                    strategy.EndDate = endDate;
                }

                if (DateTime.TryParse(strategyRow["CreatedAt"].ToString(), out var createdAt))
                {
                    strategy.CreatedAt = createdAt;
                }

                if (DateTime.TryParse(strategyRow["UpdatedAt"].ToString(), out var updatedAt))
                {
                    strategy.UpdatedAt = updatedAt;
                }

                return Ok(strategy);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving strategy");
                return BadRequest("Error retrieving strategy");
            }
        }

        [HttpPost]
        public async Task<object> Save(Strategy model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    throw new Exception("Strategy title is required");
                }

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: model.UnitID,
                    StrategyID: model.StrategyID,
                    Title: model.Title,
                    TitleAr: model.TitleAr,
                    Vision: model.Vision,
                    Mission: model.Mission,
                    EscalationCriteria: model.EscalationCriteria,
                    ActivationCriteria: model.ActivationCriteria,
                    StartDate: model.StartDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EndDate: model.EndDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    OwnerID: model.OwnerID?.ToString(CultureInfo.InvariantCulture),
                    Status: model.Status,
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving strategy");
                return BadRequest("Error saving strategy");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(Strategy model)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteStrategy",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    StrategyID: model.StrategyID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting strategy");
                return BadRequest("Error deleting strategy");
            }
        }
    }
}
