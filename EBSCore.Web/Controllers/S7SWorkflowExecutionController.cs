using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SWorkflowExecutionController : ControllerBase
    {
        private readonly DBS7SWorkflowExecutionSP executionSp;
        private readonly Common common;
        private readonly User? currentUser;

        public S7SWorkflowExecutionController(DBS7SWorkflowExecutionSP executionSp, IHttpContextAccessor httpContextAccessor)
        {
            this.executionSp = executionSp;
            common = new Common();
            currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpPost]
        public IActionResult StartManual([FromBody] ExecutionStartModel request)
        {
            var result = executionSp.QueryDatabase(SqlQueryType.ExecuteScalar, "ExecutionStartManual", currentUser?.UserID?.ToString() ?? string.Empty, WorkflowID: request.WorkflowID.ToString(), PayloadJson: request.PayloadJson ?? string.Empty);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult StartWebhook([FromBody] ExecutionStartModel request)
        {
            var result = executionSp.QueryDatabase(SqlQueryType.ExecuteScalar, "ExecutionStartWebhook", currentUser?.UserID?.ToString() ?? string.Empty, WorkflowID: request.WorkflowID.ToString(), PayloadJson: request.PayloadJson ?? string.Empty, RequestJson: request.PayloadJson ?? string.Empty);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveStep([FromBody] ExecutionStepModel step)
        {
            executionSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "ExecutionSaveStep", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: step.ExecutionID.ToString(), NodeID: step.NodeID.ToString(), Status: step.Status ?? string.Empty, OutputKey: step.SelectedOutputKey ?? string.Empty, OutputJson: step.OutputJson ?? string.Empty, ErrorMessage: step.ErrorMessage ?? string.Empty);
            return Ok();
        }

        [HttpPost]
        public IActionResult MarkStatus([FromBody] ExecutionStatusModel status)
        {
            executionSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "ExecutionMarkStatus", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: status.ExecutionID.ToString(), Status: status.Status ?? string.Empty, ErrorMessage: status.ErrorMessage ?? string.Empty);
            return Ok();
        }

        [HttpGet]
        public IActionResult Retrieve(int pageNumber = 1, int pageSize = 20)
        {
            var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "Retrieve", currentUser?.UserID?.ToString() ?? string.Empty, PageNumber: pageNumber.ToString(), PageSize: pageSize.ToString());
            return Ok(data);
        }

        [HttpGet]
        [Route("{executionId}")]
        public IActionResult RetrieveDetails(long executionId)
        {
            var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "RetrieveDetails", currentUser?.UserID?.ToString() ?? string.Empty, ExecutionID: executionId.ToString());
            return Ok(data);
        }

        [HttpGet]
        public IActionResult Dequeue()
        {
            var data = executionSp.QueryDatabase(SqlQueryType.FillDataset, "ExecutionDequeue", currentUser?.UserID?.ToString() ?? string.Empty);
            return Ok(data);
        }
    }
}
