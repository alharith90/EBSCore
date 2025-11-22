namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowConnectionModel
    {
        public int? NodeConnectionID { get; set; }
        public int? WorkflowID { get; set; }
        public int? SourceNodeID { get; set; }
        public string? SourceNodeKey { get; set; }
        public string? SourceOutputKey { get; set; }
        public int? TargetNodeID { get; set; }
        public string? TargetNodeKey { get; set; }
        public string? TargetInputKey { get; set; }
    }
}
