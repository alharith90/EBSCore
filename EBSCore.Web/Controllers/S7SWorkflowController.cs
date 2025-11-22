using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SWorkflowController : ControllerBase
    {
        private readonly DBS7SWorkflowSP workflowSp;
        private readonly Common common;
        private readonly User? currentUser;

        public S7SWorkflowController(DBS7SWorkflowSP workflowSp, IHttpContextAccessor httpContextAccessor)
        {
            this.workflowSp = workflowSp;
            common = new Common();
            currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        public IActionResult Retrieve(int pageNumber = 1, int pageSize = 20)
        {
            var data = workflowSp.QueryDatabase(SqlQueryType.FillDataset, "Retrieve", currentUser?.UserID?.ToString() ?? string.Empty, currentUser?.CompanyID?.ToString() ?? string.Empty, currentUser?.UnitID?.ToString() ?? string.Empty, PageNumber: pageNumber.ToString(), PageSize: pageSize.ToString());
            return Ok(data);
        }

        [HttpGet]
        [Route("{workflowId}")]
        public IActionResult RetrieveDetails(int workflowId)
        {
            var data = workflowSp.QueryDatabase(SqlQueryType.FillDataset, "RetrieveDetails", currentUser?.UserID?.ToString() ?? string.Empty, currentUser?.CompanyID?.ToString() ?? string.Empty, currentUser?.UnitID?.ToString() ?? string.Empty, WorkflowID: workflowId.ToString());
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Save([FromBody] WorkflowDefinitionModel workflow)
        {
            var nodes = System.Text.Json.JsonSerializer.Serialize(workflow.Nodes ?? new List<WorkflowNodeModel>());
            var connections = System.Text.Json.JsonSerializer.Serialize(workflow.Connections ?? new List<WorkflowConnectionModel>());
            var triggers = System.Text.Json.JsonSerializer.Serialize(workflow.Triggers ?? new List<WorkflowTriggerModel>());

            var result = workflowSp.QueryDatabase(
                SqlQueryType.ExecuteScalar,
                "Save",
                currentUser?.UserID?.ToString() ?? string.Empty,
                currentUser?.CompanyID?.ToString() ?? string.Empty,
                currentUser?.UnitID?.ToString() ?? string.Empty,
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

        [HttpDelete]
        [Route("{workflowId}")]
        public IActionResult Delete(int workflowId)
        {
            workflowSp.QueryDatabase(SqlQueryType.ExecuteNonQuery, "Delete", currentUser?.UserID?.ToString() ?? string.Empty, currentUser?.CompanyID?.ToString() ?? string.Empty, currentUser?.UnitID?.ToString() ?? string.Empty, WorkflowID: workflowId.ToString());
            return Ok();
        }
    }
}
