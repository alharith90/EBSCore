namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class WorkflowTrigger
{
    public int WorkflowTriggerId { get; set; }
    public int WorkflowId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public int? TriggerNodeId { get; set; }
    public string? Secret { get; set; }
    public string? CronExpression { get; set; }
    public string? ConfigurationJson { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
