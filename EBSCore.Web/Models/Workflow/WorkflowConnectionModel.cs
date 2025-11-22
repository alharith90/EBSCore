namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowConnectionModel
    {
        public int? ConnectionID { get; set; }
        public int? WorkflowID { get; set; }
        public int? SourceNodeID { get; set; }
        public string? SourceOutputKey { get; set; }
        public int? TargetNodeID { get; set; }
        public string? TargetInputKey { get; set; }
    }
}
