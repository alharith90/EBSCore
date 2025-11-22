using EBSCore.AdoClass;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SWorkflowController : ControllerBase
    {
        private readonly DBS7SWorkflowSP workflowSp;
        private readonly User? currentUser;
        private readonly ILogger<S7SWorkflowController> _logger;
        private readonly IConfiguration _configuration;

        public S7SWorkflowController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<S7SWorkflowController> logger)
        {
            _configuration = configuration;
            this.workflowSp = new DBS7SWorkflowSP(_configuration);
            currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Retrieve(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Retrieving workflows page {PageNumber} size {PageSize} for user {UserId}", pageNumber, pageSize, currentUser.UserID);
                var data = workflowSp.QueryDatabase(SqlQueryType.FillDataset, "Retrieve", currentUser.UserID ?? string.Empty, currentUser.CompanyID ?? string.Empty, currentUser.UnitID ?? string.Empty, PageNumber: pageNumber.ToString(), PageSize: pageSize.ToString());
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows list");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving workflows");
            }
        }

        [HttpGet]
        [Route("{workflowId}")]
        public IActionResult RetrieveDetails(int workflowId)
        {
            try
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Retrieving workflow details for {WorkflowId}", workflowId);
                var data = workflowSp.QueryDatabase(SqlQueryType.FillDataset, "RetrieveDetails", currentUser.UserID ?? string.Empty, currentUser.CompanyID ?? string.Empty, currentUser.UnitID ?? string.Empty, WorkflowID: workflowId.ToString());
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow details for {WorkflowId}", workflowId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving workflow details");
            }
        }

        [HttpPost]
        public IActionResult Save([FromBody] WorkflowDefinition workflow)
        {
            try
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Saving workflow {WorkflowCode} ({WorkflowId})", workflow.WorkflowCode, workflow.WorkflowID);
                var nodes = System.Text.Json.JsonSerializer.Serialize(workflow.Nodes ?? new List<WorkflowNodeModel>());
                var connections = System.Text.Json.JsonSerializer.Serialize(workflow.Connections ?? new List<WorkflowConnectionModel>());
                var triggers = System.Text.Json.JsonSerializer.Serialize(workflow.Triggers ?? new List<WorkflowTriggerModel>());

                var result = workflowSp.QueryDatabase(
                    SqlQueryType.ExecuteScalar,
                    "Save",
                    currentUser.UserID ?? string.Empty,
                    currentUser.CompanyID ?? string.Empty,
                    currentUser.UnitID ?? string.Empty,
                    workflow.WorkflowID?.ToString() ?? string.Empty,
                    workflow.WorkflowCode ?? string.Empty,
                    workflow.WorkflowName ?? string.Empty,
                    workflow.WorkflowDescription ?? string.Empty,
                    workflow.Status ?? string.Empty,
                    workflow.Priority ?? string.Empty,
                    workflow.Frequency ?? string.Empty,
                    workflow.Notes ?? string.Empty,
                    workflow.IsActive.ToString(),
                    nodes,
                    connections,
                    triggers
                );

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error saving workflow {WorkflowId}", workflow.WorkflowID);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving workflow");
            }
        }

        [HttpDelete]
        [Route("{workflowId}")]
        public IActionResult Delete(int workflowId)
        {
            try
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Deleting workflow {WorkflowId}", workflowId);
                workflowSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "Delete", currentUser.UserID ?? string.Empty, currentUser.CompanyID ?? string.Empty, currentUser.UnitID ?? string.Empty, WorkflowID: workflowId.ToString());
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow {WorkflowId}", workflowId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting workflow");
            }
        }
    }
}
