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
    public class SustainabilityInitiativeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBSustainabilityInitiativeSP _initiativeSP;
        private readonly User _currentUser;
        private readonly ILogger<SustainabilityInitiativeController> _logger;

        public SustainabilityInitiativeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<SustainabilityInitiativeController> logger)
        {
            _configuration = configuration;
            _initiativeSP = new DBSustainabilityInitiativeSP(configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet dsResult = (DataSet)_initiativeSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvSustainabilityInitiatives",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Sustainability Initiatives");
                return BadRequest("Error retrieving Sustainability Initiatives");
            }
        }

        [HttpGet]
        public object GetOne(long initiativeId)
        {
            try
            {
                DataSet dsResult = (DataSet)_initiativeSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvSustainabilityInitiative",
                    CompanyID: _currentUser.CompanyID,
                    InitiativeID: initiativeId,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Sustainability Initiative");
                return BadRequest("Error retrieving Sustainability Initiative");
            }
        }

        [HttpPost]
        public async Task<object> Save(SustainabilityInitiative initiative)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Missing required fields");
                }

                _initiativeSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveSustainabilityInitiative",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    InitiativeID: initiative.InitiativeID,
                    InitiativeName: initiative.InitiativeName,
                    Description: initiative.Description,
                    ESGCategory: initiative.ESGCategory,
                    StartDate: initiative.StartDate,
                    EndDate: initiative.EndDate,
                    ResponsibleDepartment: initiative.ResponsibleDepartment,
                    KeyMetrics: initiative.KeyMetrics,
                    BudgetAllocated: initiative.BudgetAllocated,
                    Outcome: initiative.Outcome,
                    Status: initiative.Status,
                    CreatedAt: initiative.CreatedAt,
                    UpdatedAt: initiative.UpdatedAt,
                    UpdatedBy: _currentUser.UserID,
                    CreatedBy: _currentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Sustainability Initiative");
                return BadRequest("Error saving Sustainability Initiative");
            }
        }

        [HttpDelete]
        public object Delete(long initiativeId)
        {
            try
            {
                _initiativeSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteSustainabilityInitiative",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    InitiativeID: initiativeId);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Sustainability Initiative");
                return BadRequest("Error deleting Sustainability Initiative");
            }
        }
    }
}
