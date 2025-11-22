using System.Collections.Generic;

namespace EBSCore.Web.Models.Workflow
{
    public class ExecutionStartModel
    {
        public int WorkflowID { get; set; }
        public string? PayloadJson { get; set; }
    }

    public class ExecutionStepModel
    {
        public long? ExecutionStepID { get; set; }
        public long ExecutionID { get; set; }
        public int NodeID { get; set; }
        public string? Status { get; set; }
        public string? SelectedOutputKey { get; set; }
        public string? OutputJson { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ExecutionStatusModel
    {
        public long ExecutionID { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
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
        public List<ExecutionStepModel> Steps { get; set; } = new List<ExecutionStepModel>();
    }
}
