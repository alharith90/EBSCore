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
    public class ESGMetricController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBESGMetricSP _metricSP;
        private readonly User _currentUser;
        private readonly ILogger<ESGMetricController> _logger;

        public ESGMetricController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ESGMetricController> logger)
        {
            _configuration = configuration;
            _metricSP = new DBESGMetricSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet dsResult = (DataSet)_metricSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvESGMetrics",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ESG Metrics");
                return BadRequest("Error retrieving ESG Metrics");
            }
        }

        [HttpGet]
        public object GetOne(long metricId)
        {
            try
            {
                DataSet dsResult = (DataSet)_metricSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvESGMetric",
                    CompanyID: _currentUser.CompanyID,
                    MetricID: metricId,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ESG Metric");
                return BadRequest("Error retrieving ESG Metric");
            }
        }

        [HttpPost]
        public async Task<object> Save(ESGMetric metric)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _metricSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveESGMetric",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    MetricID: metric.MetricID,
                    MetricName: metric.MetricName,
                    Category: metric.Category,
                    Description: metric.Description,
                    UnitOfMeasure: metric.UnitOfMeasure,
                    DataSource: metric.DataSource,
                    ReportingFrequency: metric.ReportingFrequency,
                    TargetValue: metric.TargetValue,
                    LatestValue: metric.LatestValue,
                    MeasurementDate: metric.MeasurementDate,
                    Owner: metric.Owner,
                    RelatedObjective: metric.RelatedObjective,
                    RelatedRisk: metric.RelatedRisk,
                    Trend: metric.Trend,
                    Comments: metric.Comments,
                    CreatedAt: metric.CreatedAt,
                    UpdatedAt: metric.UpdatedAt,
                    UpdatedBy: _currentUser.UserID,
                    CreatedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving ESG Metric");
                return BadRequest("Error saving ESG Metric");
            }
        }

        [HttpDelete]
        public object Delete(long metricId)
        {
            try
            {
                _metricSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteESGMetric",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    MetricID: metricId);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ESG Metric");
                return BadRequest("Error deleting ESG Metric");
            }
        }
    }
}
