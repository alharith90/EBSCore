namespace EBSCore.Web.Models.Workflow
{
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
}
