namespace EBSCore.Web.Models.Workflow
{
    public class ExecutionStartModel
    {
        public int WorkflowID { get; set; }
        public string? PayloadJson { get; set; }
    }

    public class ExecutionStepModel
    {
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
}
