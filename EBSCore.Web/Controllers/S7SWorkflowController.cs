using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.Data;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.AdoClass;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using EBSCore.Web.Models.Workflow;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SWorkflowController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBS7SWorkflowSP WorkflowSP;
        private readonly Common Common;
        private readonly User CurrentUser;
        private readonly ILogger<S7SWorkflowController> _logger;

        public S7SWorkflowController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<S7SWorkflowController> logger)
        {
            Configuration = configuration;
            WorkflowSP = new DBS7SWorkflowSP(configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
            _logger = logger;
        }

        // --------------------------------------------------------------------
        // Retrieve all workflows (Paged)
        // --------------------------------------------------------------------
        [HttpGet]
        public async Task<object> Get(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                DataSet dsResult = (DataSet)WorkflowSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvS7SWorkflows",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: CurrentUser.UnitID,
                    PageNumber: pageNumber.ToString(),
                    PageSize: pageSize.ToString()
                );

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving S7S Workflows");
                return BadRequest("Error retrieving S7S Workflows");
            }
        }

        // --------------------------------------------------------------------
        // Retrieve a single workflow
        // --------------------------------------------------------------------
        [HttpGet]
        public object GetOne(long workflowID)
        {
            try
            {
                DataSet dsResult = (DataSet)WorkflowSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvS7SWorkflow",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: CurrentUser.UnitID,
                    WorkflowID: workflowID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(dsResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving S7S Workflow");
                return BadRequest("Error retrieving S7S Workflow");
            }
        }

        // --------------------------------------------------------------------
        // Save Workflow
        // --------------------------------------------------------------------
        [HttpPost]
        public async Task<object> Save(WorkflowDefinition workflow)
        {
            try
            {
                string nodesJson = JsonConvert.SerializeObject(workflow.Nodes);
                string connectionsJson = JsonConvert.SerializeObject(workflow.Connections);
                string triggersJson = JsonConvert.SerializeObject(workflow.Triggers);

                WorkflowSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveS7SWorkflow",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: CurrentUser.UnitID,

                    WorkflowID: workflow.WorkflowID.ToString(),
                    WorkflowCode: workflow.WorkflowCode,
                    WorkflowName: workflow.WorkflowName,
                    WorkflowDescription: workflow.WorkflowDescription,
                    Status: workflow.Status,
                    Priority: workflow.Priority,
                    Frequency: workflow.Frequency,
                    Notes: workflow.Notes,
                    IsActive: workflow.IsActive.ToString(),

                    NodesJson: nodesJson,
                    ConnectionsJson: connectionsJson,
                    TriggersJson: triggersJson

                   // CreatedBy: CurrentUser.UserID,
                   // ModifiedBy: CurrentUser.UserID
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving S7S Workflow");
                return BadRequest("Error saving S7S Workflow");
            }
        }

        // --------------------------------------------------------------------
        // Delete Workflow
        // --------------------------------------------------------------------
        [HttpDelete]
        public object Delete(long workflowID)
        {
            try
            {
                WorkflowSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    WorkflowID: workflowID.ToString()
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting S7S Workflow");
                return BadRequest("Error deleting S7S Workflow");
            }
        }
    }
}
