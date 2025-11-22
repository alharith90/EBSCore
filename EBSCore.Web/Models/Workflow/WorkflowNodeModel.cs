namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowNodeModel
    {
        public int? NodeID { get; set; }
        public int? WorkflowID { get; set; }
        public string? NodeKey { get; set; }
        public string? Name { get; set; }
        public string? NodeType { get; set; }
        public string? ConfigJson { get; set; }
        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
        public int? CredentialID { get; set; }
        public int RetryCount { get; set; }
    }
}
