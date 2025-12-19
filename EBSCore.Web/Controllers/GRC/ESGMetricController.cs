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

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

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
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

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
                    CompanyID: CurrentCompanyId,
                    MetricID: metricId,
                    UserID: CurrentUserId);

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
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
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
                    UpdatedBy: CurrentUserId,
                    CreatedBy: CurrentUserId);

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
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
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
