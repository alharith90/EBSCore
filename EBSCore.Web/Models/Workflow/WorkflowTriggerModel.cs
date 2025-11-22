namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowTriggerModel
    {
        public int? TriggerID { get; set; }
        public int? WorkflowID { get; set; }
        public int? TriggerNodeID { get; set; }
        public string? TriggerType { get; set; }
        public string? Secret { get; set; }
        public string? CronExpression { get; set; }
        public string? ConfigurationJson { get; set; }
        public bool IsActive { get; set; }
    }
}
