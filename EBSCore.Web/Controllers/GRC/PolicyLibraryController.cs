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
    public class PolicyLibraryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBPolicyLibrarySP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<PolicyLibraryController> _logger;

        private long? CurrentUserId => long.TryParse(_currentUser?.UserID, out var userId) ? userId : null;
        private int? CurrentCompanyId => int.TryParse(_currentUser?.CompanyID, out var companyId) ? companyId : null;

        public PolicyLibraryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<PolicyLibraryController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBPolicyLibrarySP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPolicies",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving policies");
                return BadRequest("Error retrieving policies");
            }
        }

        [HttpGet]
        public object GetOne(long policyId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvPolicy",
                    CompanyID: CurrentCompanyId,
                    UserID: CurrentUserId,
                    PolicyID: policyId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving policy");
                return BadRequest("Error retrieving policy");
            }
        }

        [HttpPost]
        public async Task<object> Save(PolicyLibrary policy)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SavePolicy",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    PolicyID: policy.PolicyID,
                    PolicyCode: policy.PolicyCode,
                    PolicyNameEN: policy.PolicyNameEN,
                    PolicyNameAR: policy.PolicyNameAR,
                    PolicyType: policy.PolicyType,
                    CategoryEN: policy.CategoryEN,
                    CategoryAR: policy.CategoryAR,
                    DescriptionEN: policy.DescriptionEN,
                    DescriptionAR: policy.DescriptionAR,
                    EffectiveDate: policy.EffectiveDate?.ToString("o"),
                    ReviewDate: policy.ReviewDate?.ToString("o"),
                    OwnerUserID: policy.OwnerUserID,
                    ApproverUserID: policy.ApproverUserID,
                    StatusID: policy.StatusID,
                    RelatedRegulationIDs: policy.RelatedRegulationIDs,
                    RelatedControlIDs: policy.RelatedControlIDs,
                    VersionNumber: policy.VersionNumber,
                    DocumentPath: policy.DocumentPath,
                    IsMandatory: policy.IsMandatory,
                    AppliesToRoles: policy.AppliesToRoles,
                    CreatedBy: CurrentUserId,
                    CreatedAt: policy.CreatedAt?.ToString("o"),
                    UpdatedBy: CurrentUserId,
                    UpdatedAt: DateTime.UtcNow.ToString("o"),
                    ModifiedBy: CurrentUserId);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving policy");
                return BadRequest("Error saving policy");
            }
        }

        [HttpDelete]
        public object Delete(PolicyLibrary policy)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeletePolicy",
                    UserID: CurrentUserId,
                    CompanyID: CurrentCompanyId,
                    PolicyID: policy.PolicyID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting policy");
                return BadRequest("Error deleting policy");
            }
        }
    }
}
