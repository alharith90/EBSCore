namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class Node
{
    public int NodeId { get; set; }
    public int WorkflowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public string? ConfigJson { get; set; }
    public decimal PositionX { get; set; }
    public decimal PositionY { get; set; }
    public int? CredentialId { get; set; }
    public int RetryCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
