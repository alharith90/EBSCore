using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProcessStepController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBProcessStepSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ProcessStepController> _logger;
        private readonly Common _common;

        public ProcessStepController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProcessStepController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBProcessStepSP(_configuration);
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
            _common = new Common();
        }

        [HttpGet]
        public async Task<object> GetByProcess(int processId)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvProcessSteps",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessID: processId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving process steps");
                return BadRequest("Error retrieving process steps");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(int stepId)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvProcessStep",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    StepID: stepId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving process step");
                return BadRequest("Error retrieving process step");
            }
        }

        [HttpPost]
        public async Task<object> Save(ProcessStepSaveRequest request)
        {
            try
            {
                if (request.ProcessID <= 0)
                {
                    throw new Exception("Process ID is required to save steps");
                }

                var orderedSteps = request.Steps
                    .Select((step, index) =>
                    {
                        step.ProcessID = request.ProcessID;
                        step.StepOrder = step.StepOrder <= 0 ? index + 1 : step.StepOrder;
                        return step;
                    })
                    .ToList();

                var stepsJson = JsonConvert.SerializeObject(orderedSteps ?? new List<ProcessStep>());

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveProcessSteps",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessID: request.ProcessID.ToString(),
                    StepsJson: stepsJson,
                    CreatedBy: _currentUser.UserID,
                    UpdatedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving process steps");
                return BadRequest("Error saving process steps");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(ProcessStep step)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteProcessStep",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    StepID: step.StepID?.ToString());

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting process step");
                return BadRequest("Error deleting process step");
            }
        }
    }
}
