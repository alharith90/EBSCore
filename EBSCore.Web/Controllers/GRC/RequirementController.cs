using EBSCore.AdoClass;
using EBSCore.Web.Models;
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
    public class RequirementController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBRequirementSP RequirementSP;
        private readonly User CurrentUser;
        private readonly ILogger<RequirementController> _logger;

        public RequirementController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<RequirementController> logger)
        {
            Configuration = configuration;
            RequirementSP = new DBRequirementSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get(long? unitId = null)
        {
            try
            {
                var operation = unitId.HasValue ? "rtvRequirementsByUnit" : "rtvRequirements";
                DataSet DSResult = (DataSet)RequirementSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: operation,
                    CompanyID: CurrentUser.CompanyID,
                    OrganizationUnitID: unitId?.ToString(),
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Requirements");
                return BadRequest("Error retrieving Requirements");
            }
        }

        [HttpPost]
        public async Task<object> Save(Requirement requirement)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                RequirementSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveRequirement",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    RequirementCode: requirement.RequirementCode,
                    RequirementNo: requirement.RequirementNo,
                    RequirementType: requirement.RequirementType,
                    RequirementTitle: requirement.RequirementTitle,
                    RequirementDescription: requirement.RequirementDescription,
                    Subcategory: requirement.Subcategory,
                    RequirementDetails: requirement.RequirementDetails,
                    RequirementTags: requirement.RequirementTags,
                    RequirementFrequency: requirement.RequirementFrequency,
                    ExternalAudit: requirement.ExternalAudit,
                    InternalAudit: requirement.InternalAudit,
                    AuditReference: requirement.AuditReference,
                    RiskCategory: requirement.RiskCategory,
                    ControlOwner: requirement.ControlOwner,
                    ControlOwnerFunction: requirement.ControlOwnerFunction,
                    EvidenceRequired: requirement.EvidenceRequired,
                    EvidenceDetails: requirement.EvidenceDetails,
                    ControlID: requirement.ControlID,
                    OrganizationUnitID: requirement.OrganizationUnitID,
                    EscalationProcess: requirement.EscalationProcess,
                    EscalationThreshold: requirement.EscalationThreshold,
                    BCMActivationType: requirement.BCMActivationType,
                    BCMActivationDecision: requirement.BCMActivationDecision,
                    EscalationContacts: requirement.EscalationContacts,
                    Status: requirement.Status,
                    CreatedBy: CurrentUser.UserID,
                    ModifiedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Requirement");
                return BadRequest("Error saving Requirement");
            }
        }

        [HttpGet]
        public object GetOne(long requirementCode)
        {
            try
            {
                DataSet DSResult = (DataSet)RequirementSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRequirement",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    RequirementCode: requirementCode.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Requirement");
                return BadRequest("Error retrieving Requirement");
            }
        }

        [HttpDelete]
        public object Delete(Requirement requirement)
        {
            try
            {
                RequirementSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRequirement",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    RequirementCode: requirement.RequirementCode);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Requirement");
                return BadRequest("Error deleting Requirement");
            }
        }
    }
}
