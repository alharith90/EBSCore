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
    public class ProcessControlController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBProcessControlSP _storedProcedure;
        private readonly User _currentUser;
        private readonly ILogger<ProcessControlController> _logger;
        private readonly Common _common;

        public ProcessControlController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProcessControlController> logger)
        {
            _configuration = configuration;
            _storedProcedure = new DBProcessControlSP(_configuration);
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
                    Operation: "rtvProcessControls",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessID: processId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving process controls");
                return BadRequest("Error retrieving process controls");
            }
        }

        [HttpGet]
        public async Task<object> GetByStep(int stepId)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvProcessControlsByStep",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    StepID: stepId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving process controls by step");
                return BadRequest("Error retrieving process controls by step");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(int processControlId)
        {
            try
            {
                var result = (DataSet)_storedProcedure.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvProcessControl",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessControlID: processControlId.ToString());

                return Ok(JsonConvert.SerializeObject(result.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error retrieving process control");
                return BadRequest("Error retrieving process control");
            }
        }

        [HttpPost]
        public async Task<object> Save(ProcessControlSaveRequest request)
        {
            try
            {
                if (request.ProcessID <= 0)
                {
                    throw new Exception("Process ID is required to save controls");
                }

                if (request.Controls == null || request.Controls.Count == 0)
                {
                    return Ok("[]");
                }

                var normalizedControls = request.Controls
                    .Select(control =>
                    {
                        control.ProcessID = request.ProcessID;
                        return control;
                    })
                    .ToList();

                var controlsJson = JsonConvert.SerializeObject(normalizedControls ?? new List<ProcessControl>());

                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveProcessControls",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessID: request.ProcessID.ToString(),
                    ControlsJson: controlsJson,
                    CreatedBy: _currentUser.UserID,
                    UpdatedBy: _currentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error saving process controls");
                return BadRequest("Error saving process controls");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(ProcessControl control)
        {
            try
            {
                _storedProcedure.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteProcessControl",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    ProcessControlID: control.ProcessControlID?.ToString());

                return Ok("[]");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                _logger.LogError(ex, "Error deleting process control");
                return BadRequest("Error deleting process control");
            }
        }
    }
}
