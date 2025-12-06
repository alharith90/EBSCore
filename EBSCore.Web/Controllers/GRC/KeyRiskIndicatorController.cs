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
    public class KeyRiskIndicatorController : ControllerBase
    {
        private readonly DBKeyRiskIndicatorSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<KeyRiskIndicatorController> _logger;

        public KeyRiskIndicatorController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<KeyRiskIndicatorController> logger)
        {
            _storedProcedure = new DBKeyRiskIndicatorSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvKRIs",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KRIs");
                return BadRequest("Error retrieving KRIs");
            }
        }

        [HttpGet]
        public object GetOne(long indicatorId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvKRI",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    IndicatorID: indicatorId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving KRI");
                return BadRequest("Error retrieving KRI");
            }
        }

        [HttpPost]
        public async Task<object> Save(KeyRiskIndicator kri)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveKRI",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IndicatorID: kri.IndicatorID,
                    IndicatorName: kri.IndicatorName,
                    RelatedRisk: kri.RelatedRisk,
                    MeasurementFrequency: kri.MeasurementFrequency,
                    DataSource: kri.DataSource,
                    ThresholdValue: kri.ThresholdValue,
                    CurrentValue: kri.CurrentValue,
                    Status: kri.Status,
                    Owner: kri.Owner,
                    LastUpdateDate: kri.LastUpdateDate?.ToString("o"),
                    CreatedBy: _currentUser.UserID,
                    ModifiedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving KRI");
                return BadRequest("Error saving KRI");
            }
        }

        [HttpDelete]
        public object Delete(KeyRiskIndicator kri)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteKRI",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    IndicatorID: kri.IndicatorID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting KRI");
                return BadRequest("Error deleting KRI");
            }
        }
    }
}
