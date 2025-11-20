namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class ExecutionStep
{
    public long ExecutionStepId { get; set; }
    public long ExecutionId { get; set; }
    public int NodeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RetryCount { get; set; }
    public string? OutputJson { get; set; }
    public string? ErrorMessage { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
