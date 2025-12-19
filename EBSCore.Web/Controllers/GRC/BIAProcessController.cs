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
    public class BIAProcessController : ControllerBase
    {
        private readonly DBBIAProcessSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<BIAProcessController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public BIAProcessController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<BIAProcessController> logger)
        {
            _storedProcedure = new DBBIAProcessSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvBIAProcesses",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving BIA processes");
                return BadRequest("Error retrieving BIA processes");
            }
        }

        [HttpGet]
        public object GetOne(long biaId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvBIAProcess",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId,
                    BIAID: biaId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving BIA process");
                return BadRequest("Error retrieving BIA process");
            }
        }

        [HttpPost]
        public async Task<object> Save(BusinessImpactAnalysis process)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveBIAProcess",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    BIAID: process.BIAID,
                    ProcessName: process.ProcessName,
                    Department: process.Department,
                    ProcessOwner: process.ProcessOwner,
                    CriticalityLevel: process.CriticalityLevel,
                    MAO: process.MAO,
                    Impact1Hour: process.Impact1Hour,
                    Impact1Day: process.Impact1Day,
                    Impact1Week: process.Impact1Week,
                    ImpactDimensions: process.ImpactDimensions,
                    RTO: process.RTO,
                    RPO: process.RPO,
                    MinimumResources: process.MinimumResources,
                    InternalDependencies: process.InternalDependencies,
                    ExternalDependencies: process.ExternalDependencies,
                    RecoveryStrategies: process.RecoveryStrategies,
                    StrategyLibraryRef: process.StrategyLibraryRef,
                    AllowableDataLoss: process.AllowableDataLoss,
                    BackupAvailability: process.BackupAvailability,
                    HasAlternateWorkaround: process.HasAlternateWorkaround,
                    BCPLink: process.BCPLink,
                    LastReviewDate: process.LastReviewDate?.ToString("o"));

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving BIA process");
                return BadRequest("Error saving BIA process");
            }
        }

        [HttpDelete]
        public object Delete(BusinessImpactAnalysis process)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteBIAProcess",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    BIAID: process.BIAID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting BIA process");
                return BadRequest("Error deleting BIA process");
            }
        }
    }
}
