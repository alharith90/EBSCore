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
    public class StakeholderController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBStakeholderSP StakeholderSP;
        private readonly User CurrentUser;
        private readonly ILogger<StakeholderController> _logger;

        public StakeholderController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<StakeholderController> logger)
        {
            Configuration = configuration;
            StakeholderSP = new DBStakeholderSP(configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)StakeholderSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStakeholders",
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stakeholders");
                return BadRequest("Error retrieving Stakeholders");
            }
        }

        [HttpGet]
        public object GetOne(long StakeholderID)
        {
            try
            {
                DataSet DSResult = (DataSet)StakeholderSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStakeholder",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    StakeholderID: StakeholderID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stakeholder");
                return BadRequest("Error retrieving Stakeholder");
            }
        }

        [HttpGet]
        public object GetByUnit(long? UnitID = null)
        {
            try
            {
                DataSet DSResult = (DataSet)StakeholderSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvStakeholdersByUnit",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: UnitID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stakeholder list");
                return BadRequest("Error retrieving Stakeholder list");
            }
        }

        [HttpPost]
        public async Task<object> Save(Stakeholder stakeholder)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stakeholder.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                StakeholderSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveStakeholder",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: stakeholder.UnitID,
                    StakeholderID: stakeholder.StakeholderID,
                    StakeholderName: stakeholder.StakeholderName,
                    StakeholderType: stakeholder.StakeholderType,
                    Role: stakeholder.Role,
                    ContactEmail: stakeholder.ContactEmail,
                    ContactPhone: stakeholder.ContactPhone,
                    IsCritical: stakeholder.IsCritical?.ToString(),
                    Notes: stakeholder.Notes,
                    CreatedBy: CurrentUser.UserID,
                    ModifiedBy: CurrentUser.UserID
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Stakeholder");
                return BadRequest("Error saving Stakeholder");
            }
        }

        [HttpDelete]
        public object Delete(Stakeholder stakeholder)
        {
            try
            {
                StakeholderSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteStakeholder",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    StakeholderID: stakeholder.StakeholderID
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Stakeholder");
                return BadRequest("Error deleting Stakeholder");
            }
        }
    }
}
