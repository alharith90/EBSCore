using System.Collections.Generic;

namespace EBSCore.Web.WorkflowEngine.Application.DTOs;

public class WorkflowDto
{
    public int WorkflowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public IList<NodeDto> Nodes { get; set; } = new List<NodeDto>();
    public IList<NodeConnectionDto> Connections { get; set; } = new List<NodeConnectionDto>();
    public IList<WorkflowTriggerDto> Triggers { get; set; } = new List<WorkflowTriggerDto>();
}

public class NodeDto
{
    public int NodeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public string? ConfigJson { get; set; }
    public decimal PositionX { get; set; }
    public decimal PositionY { get; set; }
    public int? CredentialId { get; set; }
    public int RetryCount { get; set; }
}

public class NodeConnectionDto
{
    public int NodeConnectionId { get; set; }
    public int SourceNodeId { get; set; }
    public string? SourceOutputKey { get; set; }
    public int TargetNodeId { get; set; }
    public string? TargetInputKey { get; set; }
}

public class WorkflowTriggerDto
{
    public int WorkflowTriggerId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public int? TriggerNodeId { get; set; }
    public string? Secret { get; set; }
    public string? CronExpression { get; set; }
    public string? ConfigurationJson { get; set; }
    public bool IsActive { get; set; }
}

public class WorkflowQueryDto
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
