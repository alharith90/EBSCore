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
    public class InformationSystemController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBInformationSystemSP InformationSystemSP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public InformationSystemController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            InformationSystemSP = new DBInformationSystemSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1" , string? PageSize = "10" , string? SortColumn="",string? SortDirection="")
        {
            try
            {
                DataSet DSResult = (DataSet)InformationSystemSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvInformationSystems",
                    UserID:CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID, 
                    SortColumn:SortColumn,
                    SortDirection:SortDirection,
                    PageNumber:PageNumber);

                // Create a wrapper object
                var Result = new
                {
                    Data = DSResult.Tables[0],
                    PageCount = DSResult.Tables.Count > 1 ? DSResult.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(Result));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving Information Systems");
            }
        }
        [HttpGet]
        public object GetOne(string SystemID)
        {
            try
            {
                DataSet DSResult = (DataSet)InformationSystemSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvInformationSystems",
                    UserID:CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    SystemID: SystemID);

                // Create a wrapper object
                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving Information System");
            }
        }

        [HttpPost]
        public async Task<object> Save(InformationSystem infoSystem)
        {
            try
            {
                InformationSystemSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveInformationSystem",
                    UserID: CurrentUser.UserID,
                    SystemID: infoSystem.SystemID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: infoSystem.UnitID,
                    SystemName: infoSystem.SystemName,
                    RPO: infoSystem.RPO,
                    ApplicationLifecycleStatus: infoSystem.ApplicationLifecycleStatus,
                    Type: infoSystem.Type,
                    RequiredFor: infoSystem.RequiredFor,
                    SystemDescription: infoSystem.SystemDescription,
                    PrimaryOwnerId: infoSystem.PrimaryOwnerId,
                    SecondaryOwner: infoSystem.SecondaryOwner,
                    BusinessOwner: infoSystem.BusinessOwner,
                    InternetFacing: infoSystem.InternetFacing.ToString(),
                    ThirdPartyAccess: infoSystem.ThirdPartyAccess.ToString(),
                    NumberOfUsers: infoSystem.NumberOfUsers,
                    LicenseType: infoSystem.LicenseType,
                    Infrastructure: infoSystem.Infrastructure,
                    MFAEnabled: infoSystem.MFAEnabled,
                    MFAStatusDetails: infoSystem.MFAStatusDetails,
                    AssociatedInformationSystems: infoSystem.AssociatedInformationSystems,
                    Confidentiality: infoSystem.Confidentiality,
                    Integrity: infoSystem.Integrity,
                    Availability: infoSystem.Availability,
                    OverallCategorizationRating: infoSystem.OverallCategorizationRating,
                    HighestInformationClassification: infoSystem.HighestInformationClassification,
                    RiskHighlightedByIT: infoSystem.RiskHighlightedByIT,
                    AdditionalNote: infoSystem.AdditionalNote,
                    Logo: infoSystem.Logo,
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving Information System");
            }
        }



        [HttpDelete]
        public object Delete(InformationSystem infoSystem)
        {
            try
            {
                InformationSystemSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteInformationSystem",
                    UserID: CurrentUser.UserID, 
                    SystemID: infoSystem.SystemID,
                    CompanyID: CurrentUser.CompanyID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting Information System");
            }
        }
    }
}
