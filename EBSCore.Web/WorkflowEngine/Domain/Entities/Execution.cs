namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class Execution
{
    public long ExecutionId { get; set; }
    public int WorkflowId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public string? TriggerDataJson { get; set; }
    public string? WebhookRequestJson { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RetryCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
