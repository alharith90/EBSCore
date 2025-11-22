using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SWorkflowExecutionController : ControllerBase
    {
        private readonly DBS7SWorkflowExecutionSP executionSp;
        private readonly User? currentUser;
        private readonly ILogger<S7SWorkflowExecutionController> _logger;

        public S7SWorkflowExecutionController(DBS7SWorkflowExecutionSP executionSp, IHttpContextAccessor httpContextAccessor, ILogger<S7SWorkflowExecutionController> logger)
        {
            this.executionSp = executionSp;
            currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpPost]
        public IActionResult StartManual([FromBody] ExecutionStartModel request)
        {
            try
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                _logger.LogInformation("Starting manual workflow execution for {WorkflowId}", request.WorkflowID);
                var result = executionSp.QueryDatabase(SqlQueryType.ExecuteScalar, "ExecutionStartManual", currentUser.UserID ?? string.Empty, WorkflowID: request.WorkflowID.ToString(), PayloadJson: request.PayloadJson ?? string.Empty);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error starting manual execution for {WorkflowId}", request.WorkflowID);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error starting execution");
            }
        }

        [HttpPost]
        public IActionResult StartWebhook([FromBody] ExecutionStartModel request)
        {
            try
            {
                _logger.LogInformation("Starting webhook workflow execution for {WorkflowId}", request.WorkflowID);
                var result = executionSp.QueryDatabase(SqlQueryType.ExecuteScalar, "ExecutionStartWebhook", currentUser?.UserID?.ToString() ?? string.Empty, WorkflowID: request.WorkflowID.ToString(), PayloadJson: request.PayloadJson ?? string.Empty, RequestJson: request.PayloadJson ?? string.Empty);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error starting webhook execution for {WorkflowId}", request.WorkflowID);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error starting execution");
            }
        }

        [HttpPost]
        public IActionResult SaveStep([FromBody] ExecutionStepModel step)
        {
            try
            {
                _logger.LogInformation("Saving execution step {ExecutionId}:{NodeId}", step.ExecutionID, step.NodeID);
                executionSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "ExecutionSaveStep", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: step.ExecutionID.ToString(), NodeID: step.NodeID.ToString(), Status: step.Status ?? string.Empty, OutputKey: step.SelectedOutputKey ?? string.Empty, OutputJson: step.OutputJson ?? string.Empty, ErrorMessage: step.ErrorMessage ?? string.Empty);
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error saving execution step for execution {ExecutionId}", step.ExecutionID);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving execution step");
            }
        }

        [HttpPost]
        public IActionResult MarkStatus([FromBody] ExecutionStatusModel status)
        {
            try
            {
                _logger.LogInformation("Marking execution {ExecutionId} with status {Status}", status.ExecutionID, status.Status);
                executionSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "ExecutionMarkStatus", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: status.ExecutionID.ToString(), Status: status.Status ?? string.Empty, ErrorMessage: status.ErrorMessage ?? string.Empty);
                return Ok();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error marking execution status for {ExecutionId}", status.ExecutionID);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error marking execution status");
            }
        }

        [HttpGet]
        public IActionResult Retrieve(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Retrieving execution list page {PageNumber} size {PageSize}", pageNumber, pageSize);
                var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "Retrieve", currentUser?.UserID?.ToString() ?? string.Empty, PageNumber: pageNumber.ToString(), PageSize: pageSize.ToString());
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving executions page {PageNumber}", pageNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving executions");
            }
        }

        [HttpGet]
        [Route("{executionId}")]
        public IActionResult RetrieveDetails(long executionId)
        {
            try
            {
                _logger.LogInformation("Retrieving execution details {ExecutionId}", executionId);
                var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "RetrieveDetails", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: executionId.ToString());
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving execution details {ExecutionId}", executionId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving execution details");
            }
        }

        [HttpGet]
        public IActionResult Dequeue()
        {
            try
            {
                _logger.LogInformation("Dequeuing execution for user {UserId}", currentUser?.UserID);
                var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "ExecutionDequeue", currentUser?.UserID?.ToString() ?? string.Empty);
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error dequeuing execution");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error dequeuing execution");
            }
        }
    }
}
