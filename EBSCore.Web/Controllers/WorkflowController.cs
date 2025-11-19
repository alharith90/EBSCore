using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WorkflowController : ControllerBase
    {
        private readonly DBWorkflowSP _workflowSP;
        private readonly Common _common;
        private readonly User? _currentUser;

        public WorkflowController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _workflowSP = new DBWorkflowSP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1", string? PageSize = "20", string? Search = "")
        {
            try
            {
                var dataSet = (DataSet)_workflowSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "rtvWorkflows",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    PageNumber: PageNumber,
                    PageSize: PageSize,
                    SearchQuery: Search
                );

                var list = new List<WorkflowSummary>();
                if (dataSet.Tables.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        list.Add(new WorkflowSummary
                        {
                            WorkflowID = Convert.ToInt32(row[nameof(WorkflowSummary.WorkflowID)]),
                            WorkflowCode = row["WorkflowCode"].ToString(),
                            WorkflowName = row["Name"].ToString(),
                            Status = row["Status"].ToString(),
                            Priority = row["Priority"].ToString(),
                            Frequency = row["Frequency"].ToString(),
                            IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }

                var totalCount = dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0
                    ? Convert.ToInt32(dataSet.Tables[1].Rows[0]["TotalCount"])
                    : list.Count;

                return Ok(new { Data = list, TotalCount = totalCount });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving workflows");
            }
        }

        [HttpGet("{workflowId}")]
        public async Task<object> GetById(int workflowId)
        {
            try
            {
                var dataSet = (DataSet)_workflowSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "rtvWorkflow",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    WorkflowID: workflowId.ToString()
                );

                if (_common.IsEmptyDataSet(dataSet))
                {
                    return NotFound();
                }

                var workflowRow = dataSet.Tables[0].Rows[0];
                var workflow = new WorkflowDefinition
                {
                    WorkflowID = workflowId,
                    CompanyID = workflowRow["CompanyID"] == DBNull.Value ? null : Convert.ToInt32(workflowRow["CompanyID"]),
                    UnitID = workflowRow["UnitID"] == DBNull.Value ? null : Convert.ToInt64(workflowRow["UnitID"]),
                    WorkflowCode = workflowRow["WorkflowCode"].ToString(),
                    WorkflowName = workflowRow["Name"].ToString(),
                    WorkflowDescription = workflowRow["Description"].ToString(),
                    Status = workflowRow["Status"].ToString(),
                    Priority = workflowRow["Priority"].ToString(),
                    Frequency = workflowRow["Frequency"].ToString(),
                    Notes = workflowRow["Notes"].ToString(),
                    IsActive = workflowRow["IsActive"] != DBNull.Value && Convert.ToBoolean(workflowRow["IsActive"])
                };

                if (dataSet.Tables.Count > 1)
                {
                    foreach (DataRow nodeRow in dataSet.Tables[1].Rows)
                    {
                        workflow.Nodes.Add(new WorkflowNodeModel
                        {
                            NodeID = Convert.ToInt32(nodeRow["NodeID"]),
                            NodeKey = $"node-{nodeRow["NodeID"]}",
                            Name = nodeRow["Name"].ToString(),
                            NodeType = nodeRow["NodeType"].ToString(),
                            ConfigJson = nodeRow["ConfigJson"].ToString(),
                            PositionX = nodeRow["PositionX"] == DBNull.Value ? 0 : Convert.ToDecimal(nodeRow["PositionX"]),
                            PositionY = nodeRow["PositionY"] == DBNull.Value ? 0 : Convert.ToDecimal(nodeRow["PositionY"]),
                            CredentialID = nodeRow["CredentialID"] == DBNull.Value ? null : Convert.ToInt32(nodeRow["CredentialID"]),
                            RetryCount = nodeRow["RetryCount"] == DBNull.Value ? 0 : Convert.ToInt32(nodeRow["RetryCount"])
                        });
                    }
                }

                if (dataSet.Tables.Count > 2)
                {
                    foreach (DataRow connectionRow in dataSet.Tables[2].Rows)
                    {
                        workflow.Connections.Add(new WorkflowConnectionModel
                        {
                            NodeConnectionID = Convert.ToInt32(connectionRow["NodeConnectionID"]),
                            SourceNodeID = Convert.ToInt32(connectionRow["SourceNodeID"]),
                            TargetNodeID = Convert.ToInt32(connectionRow["TargetNodeID"]),
                            SourceNodeKey = $"node-{connectionRow["SourceNodeID"]}",
                            TargetNodeKey = $"node-{connectionRow["TargetNodeID"]}",
                            SourceOutputKey = connectionRow["SourceOutputKey"].ToString(),
                            TargetInputKey = connectionRow["TargetInputKey"].ToString()
                        });
                    }
                }

                if (dataSet.Tables.Count > 3)
                {
                    foreach (DataRow triggerRow in dataSet.Tables[3].Rows)
                    {
                        workflow.Triggers.Add(new WorkflowTriggerModel
                        {
                            WorkflowTriggerID = Convert.ToInt32(triggerRow["WorkflowTriggerID"]),
                            TriggerNodeID = triggerRow["TriggerNodeID"] == DBNull.Value ? null : Convert.ToInt32(triggerRow["TriggerNodeID"]),
                            TriggerType = triggerRow["TriggerType"].ToString(),
                            Secret = triggerRow["Secret"].ToString(),
                            CronExpression = triggerRow["CronExpression"].ToString(),
                            ConfigurationJson = triggerRow["ConfigurationJson"].ToString()
                        });
                    }
                }

                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving workflow");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] WorkflowDefinition workflow)
        {
            try
            {
                if (_currentUser == null || _currentUser.UserType != UserType.Manager)
                {
                    return Unauthorized("Insufficient privileges");
                }

                var payloadNodes = JsonConvert.SerializeObject(workflow.Nodes ?? new List<WorkflowNodeModel>());
                var payloadConnections = JsonConvert.SerializeObject(workflow.Connections ?? new List<WorkflowConnectionModel>());
                var payloadTriggers = JsonConvert.SerializeObject(workflow.Triggers ?? new List<WorkflowTriggerModel>());

                var workflowId = _workflowSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                    Operation: "SaveWorkflow",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    UnitID: workflow.UnitID?.ToString(),
                    WorkflowID: workflow.WorkflowID?.ToString(),
                    WorkflowCode: workflow.WorkflowCode,
                    WorkflowName: workflow.WorkflowName,
                    WorkflowDescription: workflow.WorkflowDescription,
                    Status: workflow.Status,
                    Priority: workflow.Priority,
                    Frequency: workflow.Frequency,
                    Notes: workflow.Notes,
                    IsActive: workflow.IsActive ? "1" : "0",
                    NodesJson: payloadNodes,
                    ConnectionsJson: payloadConnections,
                    TriggersJson: payloadTriggers
                );

                return Ok(new { WorkflowID = workflowId });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving workflow");
            }
        }

        [HttpDelete("{workflowId}")]
        public async Task<object> Delete(int workflowId)
        {
            try
            {
                if (_currentUser == null || _currentUser.UserType != UserType.Manager)
                {
                    return Unauthorized("Insufficient privileges");
                }

                _workflowSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteWorkflow",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    WorkflowID: workflowId.ToString()
                );

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error deleting workflow");
            }
        }
    }
}
