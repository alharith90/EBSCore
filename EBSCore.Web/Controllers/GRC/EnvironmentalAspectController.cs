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
    public class EnvironmentalAspectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBEnvironmentalAspectSP _aspectSP;
        private readonly User _currentUser;
        private readonly ILogger<EnvironmentalAspectController> _logger;

        public EnvironmentalAspectController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<EnvironmentalAspectController> logger)
        {
            _configuration = configuration;
            _aspectSP = new DBEnvironmentalAspectSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet dsResult = (DataSet)_aspectSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAspects",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Environmental Aspects");
                return BadRequest("Error retrieving Environmental Aspects");
            }
        }

        [HttpGet]
        public object GetOne(long aspectId)
        {
            try
            {
                DataSet dsResult = (DataSet)_aspectSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvAspect",
                    CompanyID: _currentUser.CompanyID,
                    AspectID: aspectId,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Environmental Aspect");
                return BadRequest("Error retrieving Environmental Aspect");
            }
        }

        [HttpPost]
        public async Task<object> Save(EnvironmentalAspect aspect)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _aspectSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveAspect",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    AspectID: aspect.AspectID,
                    UnitID: aspect.UnitID,
                    AspectDescription: aspect.AspectDescription,
                    EnvironmentalImpact: aspect.EnvironmentalImpact,
                    ImpactSeverity: aspect.ImpactSeverity,
                    FrequencyLikelihood: aspect.FrequencyLikelihood,
                    SignificanceRating: aspect.SignificanceRating,
                    ControlsInPlace: aspect.ControlsInPlace,
                    LegalRequirement: aspect.LegalRequirement,
                    ImprovementActions: aspect.ImprovementActions,
                    AspectOwner: aspect.AspectOwner,
                    MonitoringMetric: aspect.MonitoringMetric,
                    LastEvaluationDate: aspect.LastEvaluationDate,
                    Status: aspect.Status,
                    CreatedAt: aspect.CreatedAt,
                    UpdatedAt: aspect.UpdatedAt,
                    UpdatedBy: _currentUser.UserID,
                    CreatedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Environmental Aspect");
                return BadRequest("Error saving Environmental Aspect");
            }
        }

        [HttpDelete]
        public object Delete(long aspectId)
        {
            try
            {
                _aspectSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteAspect",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    AspectID: aspectId);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Environmental Aspect");
                return BadRequest("Error deleting Environmental Aspect");
            }
        }
    }
}
