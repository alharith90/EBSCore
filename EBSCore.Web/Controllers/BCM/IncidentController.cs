using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EBSCore.Web.AppCode;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.BCM
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IncidentController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBIncidentSP IncidentSP;
        private readonly User CurrentUser;
        private readonly ILogger<IncidentController> _logger;

        public IncidentController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<IncidentController> logger)
        {
            Configuration = configuration;
            IncidentSP = new DBIncidentSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get(string? UnitID = null)
        {
            try
            {
                string operation = string.IsNullOrEmpty(UnitID) ? "rtvIncidents" : "rtvIncidentsByUnit";
                DataSet DSResult = (DataSet)IncidentSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: operation,
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID,
                    UnitID: UnitID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incidents");
                return BadRequest("Error retrieving incidents");
            }
        }

        [HttpGet]
        public object GetOne(long IncidentID)
        {
            try
            {
                DataSet DSResult = (DataSet)IncidentSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvIncident",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    IncidentID: IncidentID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident");
                return BadRequest("Error retrieving incident");
            }
        }

        [HttpPost]
        public async Task<object> Save(Incident Incident)
        {
            try
            {
                if (Incident.UnitID == null || Incident.UnitID == "")
                {
                    throw new Exception("Unit ID is required");
                }

                if (Incident.BCPActivated && !Incident.EscalatedToBC)
                {
                    Incident.EscalatedToBC = true;
                }

                if (Incident.BCPActivated && Incident.ActivationTime == null)
                {
                    Incident.ActivationTime = DateTime.UtcNow;
                }

                IncidentSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveIncident",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: Incident.UnitID,
                    IncidentID: Incident.IncidentID?.ToString(),
                    Title: Incident.Title,
                    Description: Incident.Description,
                    IncidentDate: Incident.IncidentDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                    ReportedBy: Incident.ReportedBy,
                    AffectedAssets: Incident.AffectedAssets,
                    RelatedRiskIDs: Incident.RelatedRiskIDs,
                    ImpactedActivities: Incident.ImpactedActivities,
                    EscalationLevel: Incident.EscalationLevel,
                    EscalationNotes: Incident.EscalationNotes,
                    EscalatedToBC: Incident.EscalatedToBC.ToString(),
                    BCPActivated: Incident.BCPActivated.ToString(),
                    ActivationReason: Incident.ActivationReason,
                    ActivationTime: Incident.ActivationTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    RecoveryStartTime: Incident.RecoveryStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status: Incident.Status,
                    Notes: Incident.Notes,
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving incident");
                return BadRequest("Error saving incident");
            }
        }

        [HttpDelete]
        public object Delete(Incident Incident)
        {
            try
            {
                IncidentSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteIncident",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    IncidentID: Incident.IncidentID?.ToString());

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident");
                return BadRequest("Error deleting incident");
            }
        }
    }
}
