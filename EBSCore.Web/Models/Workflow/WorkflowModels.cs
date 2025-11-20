using System.Collections.Generic;

namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowDefinition
    {
        public int? WorkflowID { get; set; }
        public int? CompanyID { get; set; }
        public long? UnitID { get; set; }
        public string? WorkflowCode { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowDescription { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Frequency { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public List<WorkflowNodeModel> Nodes { get; set; } = new();
        public List<WorkflowConnectionModel> Connections { get; set; } = new();
        public List<WorkflowTriggerModel> Triggers { get; set; } = new();
    }

    public class WorkflowSummary
    {
        public int WorkflowID { get; set; }
        public string? WorkflowCode { get; set; }
        public string? WorkflowName { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Frequency { get; set; }
        public bool IsActive { get; set; }
    }

    public class WorkflowNodeModel
    {
        public int? NodeID { get; set; }
        public string? NodeKey { get; set; }
        public string? Name { get; set; }
        public string? NodeType { get; set; }
        public string? ConfigJson { get; set; }
        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
        public int? CredentialID { get; set; }
        public int RetryCount { get; set; }
    }

    public class WorkflowConnectionModel
    {
        public int? NodeConnectionID { get; set; }
        public int? SourceNodeID { get; set; }
        public string? SourceNodeKey { get; set; }
        public string? SourceOutputKey { get; set; }
        public int? TargetNodeID { get; set; }
        public string? TargetNodeKey { get; set; }
        public string? TargetInputKey { get; set; }
    }

    public class WorkflowTriggerModel
    {
        public int? WorkflowTriggerID { get; set; }
        public int? TriggerNodeID { get; set; }
        public string? TriggerType { get; set; }
        public string? Secret { get; set; }
        public string? CronExpression { get; set; }
        public string? ConfigurationJson { get; set; }
    }

    public class WorkflowExecutionRequest
    {
        public string? PayloadJson { get; set; }
    }

    public class WorkflowExecutionModel
    {
        public long ExecutionID { get; set; }
        public int WorkflowID { get; set; }
        public string? Status { get; set; }
        public string? TriggerType { get; set; }
        public string? TriggerDataJson { get; set; }
        public string? WebhookRequestJson { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class WorkflowExecutionDetail
    {
        public WorkflowExecutionModel? Execution { get; set; }
        public List<WorkflowExecutionStepModel> Steps { get; set; } = new();
    }

    public class WorkflowExecutionStepModel
    {
        public long ExecutionStepID { get; set; }
        public long ExecutionID { get; set; }
        public int NodeID { get; set; }
        public string? Status { get; set; }
        public string? SelectedOutputKey { get; set; }
        public string? OutputJson { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class WorkflowCredentialModel
    {
        public int? CredentialID { get; set; }
        public string? CredentialName { get; set; }
        public string? CredentialType { get; set; }
        public string? DataJson { get; set; }
        public string? Notes { get; set; }
    }
}
