namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class NodeConnection
{
    public int NodeConnectionId { get; set; }
    public int WorkflowId { get; set; }
    public int SourceNodeId { get; set; }
    public string? SourceOutputKey { get; set; }
    public int TargetNodeId { get; set; }
    public string? TargetInputKey { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
