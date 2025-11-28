using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ManagementReviewReportController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBManagementReviewReportSP ManagementReviewReportSP;
        private readonly Common Common;
        private readonly User CurrentUser;
        private readonly ILogger<ManagementReviewReportController> _logger;

        public ManagementReviewReportController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ManagementReviewReportController> logger)
        {
            Configuration = configuration;
            ManagementReviewReportSP = new DBManagementReviewReportSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)ManagementReviewReportSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvManagementReviewReports",
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Management Review Reports");
                return BadRequest("Error retrieving Management Review Reports");
            }
        }

        [HttpPost]
        public async Task<object> Save(ManagementReviewReport report)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(report.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }
                if (string.IsNullOrWhiteSpace(report.ReportTitle))
                {
                    throw new Exception("Report title is required");
                }

                ManagementReviewReportSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveManagementReviewReport",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: report.UnitID,
                    ReportID: report.ReportID,
                    ReportTitle: report.ReportTitle,
                    MeetingDate: report.MeetingDate,
                    Summary: report.Summary,
                    Decisions: report.Decisions,
                    FollowUpActions: report.FollowUpActions,
                    NextReviewDate: report.NextReviewDate,
                    Status: report.Status,
                    Notes: report.Notes,
                    CreatedBy: CurrentUser.UserID,
                    ModifiedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Management Review Report");
                return BadRequest("Error saving Management Review Report");
            }
        }

        [HttpGet]
        public object GetOne(long ReportID)
        {
            try
            {
                DataSet DSResult = (DataSet)ManagementReviewReportSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvManagementReviewReport",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    ReportID: ReportID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Management Review Report");
                return BadRequest("Error retrieving Management Review Report");
            }
        }

        [HttpGet]
        public object GetByUnit(long? UnitID = null)
        {
            try
            {
                DataSet DSResult = (DataSet)ManagementReviewReportSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvManagementReviewReportsByUnit",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: UnitID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Management Review Reports");
                return BadRequest("Error retrieving Management Review Reports");
            }
        }

        [HttpDelete]
        public object Delete(ManagementReviewReport report)
        {
            try
            {
                ManagementReviewReportSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteManagementReviewReport",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    ReportID: report.ReportID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Management Review Report");
                return BadRequest("Error deleting Management Review Report");
            }
        }
    }
}
