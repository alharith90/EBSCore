using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExecutionController : ControllerBase
    {
        private readonly DBWorkflowExecutionSP _executionSP;
        private readonly User? _currentUser;
        private readonly ILogger<ExecutionController> _logger;

        public ExecutionController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<ExecutionController> logger)
        {
            _executionSP = new DBWorkflowExecutionSP(configuration);
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet("{workflowId}")]
        public async Task<object> GetByWorkflow(int workflowId, string? PageNumber = "1", string? PageSize = "20")
        {
            try
            {
                _logger.LogInformation("Retrieving executions for workflow {WorkflowId} page {PageNumber} size {PageSize}", workflowId, PageNumber, PageSize);
                var ds = (DataSet)_executionSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "rtvExecutions",
                    WorkflowID: workflowId.ToString(),
                    PageNumber: PageNumber,
                    PageSize: PageSize
                );

                var list = new List<WorkflowExecutionModel>();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new WorkflowExecutionModel
                        {
                            ExecutionID = Convert.ToInt64(row["ExecutionID"]),
                            WorkflowID = Convert.ToInt32(row["WorkflowID"]),
                            Status = row["Status"].ToString(),
                            TriggerType = row["TriggerType"].ToString(),
                            TriggerDataJson = row["TriggerDataJson"].ToString(),
                            WebhookRequestJson = row["WebhookRequestJson"].ToString(),
                            ErrorMessage = row["ErrorMessage"].ToString()
                        });
                    }
                }

                var totalCount = ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0
                    ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"])
                    : list.Count;

                return Ok(new { Data = list, TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving executions for workflow {WorkflowId}", workflowId);
                return BadRequest("Error retrieving executions");
            }
        }

        [HttpGet("detail/{executionId}")]
        public async Task<object> Get(long executionId)
        {
            try
            {
                _logger.LogInformation("Retrieving execution detail for {ExecutionId}", executionId);
                var ds = (DataSet)_executionSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "rtvExecution",
                    ExecutionID: executionId.ToString()
                );

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return NotFound();
                }

                var detail = new WorkflowExecutionDetail();
                var executionRow = ds.Tables[0].Rows[0];
                detail.Execution = new WorkflowExecutionModel
                {
                    ExecutionID = Convert.ToInt64(executionRow["ExecutionID"]),
                    WorkflowID = Convert.ToInt32(executionRow["WorkflowID"]),
                    Status = executionRow["Status"].ToString(),
                    TriggerType = executionRow["TriggerType"].ToString(),
                    TriggerDataJson = executionRow["TriggerDataJson"].ToString(),
                    WebhookRequestJson = executionRow["WebhookRequestJson"].ToString(),
                    ErrorMessage = executionRow["ErrorMessage"].ToString()
                };

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow stepRow in ds.Tables[1].Rows)
                    {
                        detail.Steps.Add(new ExecutionStepModel
                        {
                            ExecutionStepID = Convert.ToInt64(stepRow["ExecutionStepID"]),
                            ExecutionID = Convert.ToInt64(stepRow["ExecutionID"]),
                            NodeID = Convert.ToInt32(stepRow["NodeID"]),
                            Status = stepRow["Status"].ToString(),
                            SelectedOutputKey = stepRow["SelectedOutputKey"].ToString(),
                            OutputJson = stepRow["OutputJson"].ToString(),
                            ErrorMessage = stepRow["ErrorMessage"].ToString()
                        });
                    }
                }

                return Ok(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving execution detail for {ExecutionId}", executionId);
                return BadRequest("Error retrieving execution");
            }
        }

        [HttpPost("{workflowId}")]
        public async Task<object> Start(int workflowId, [FromBody] WorkflowExecutionRequest request)
        {
            try
            {
                _logger.LogInformation("Starting manual execution for workflow {WorkflowId}", workflowId);
                if (_currentUser?.UserID == null)
                {
                    return Unauthorized();
                }

                var executionId = _executionSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                    Operation: "StartManualExecution",
                    UserID: _currentUser.UserID,
                    WorkflowID: workflowId.ToString(),
                    PayloadJson: request?.PayloadJson ?? string.Empty
                );

                return Ok(new { ExecutionID = executionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting execution for workflow {WorkflowId}", workflowId);
                return BadRequest("Error starting execution");
            }
        }
    }
}
