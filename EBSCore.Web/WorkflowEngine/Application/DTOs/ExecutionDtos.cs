using System.Collections.Generic;

namespace EBSCore.Web.WorkflowEngine.Application.DTOs;

public class ExecutionDto
{
    public long ExecutionId { get; set; }
    public int WorkflowId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ExecutionDetailDto : ExecutionDto
{
    public string? TriggerDataJson { get; set; }
    public string? WebhookRequestJson { get; set; }
    public IList<ExecutionStepDto> Steps { get; set; } = new List<ExecutionStepDto>();
}

public class ExecutionStepDto
{
    public long ExecutionStepId { get; set; }
    public int NodeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? OutputJson { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ExecutionRequestDto
{
    public IDictionary<string, object?>? Payload { get; set; }
}

public class WebhookRequestDto
{
    public IDictionary<string, string>? Headers { get; set; }
    public IDictionary<string, string>? Query { get; set; }
    public string? Body { get; set; }
}
