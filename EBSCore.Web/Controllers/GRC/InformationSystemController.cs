using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using InformationSystemModel = EBSCore.Web.Models.GRC.InformationSystem;
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
    public class InformationSystemGRCController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBInformationSystemSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<InformationSystemGRCController> _logger;

        public InformationSystemGRCController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<InformationSystemGRCController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBInformationSystemSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvInformationSystems",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                var wrapped = new
                {
                    Data = result.Tables[0],
                    PageCount = result.Tables.Count > 1 ? result.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(wrapped));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving information systems");
                return BadRequest("Error retrieving information systems");
            }
        }

        [HttpGet]
        public object GetOne(int systemId)
        {
            try
            {
                DataSet result = (DataSet)_storedProcedure.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvInformationSystems",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    SystemID: systemId);

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving information system");
                return BadRequest("Error retrieving information system");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] InformationSystemModel infoSystem)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveInformationSystem",
                    UserID: _currentUser.UserID,
                    SystemID: infoSystem.SystemID,
                    CompanyID: _currentUser.CompanyID,
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
                    CreatedBy: _currentUser.UserID,
                    UpdatedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving information system");
                return BadRequest("Error saving information system");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int systemId)
        {
            try
            {
                _storedProcedure.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteInformationSystem",
                    UserID: _currentUser.UserID,
                    SystemID: systemId,
                    CompanyID: _currentUser.CompanyID);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting information system");
                return BadRequest("Error deleting information system");
            }
        }
    }
}
