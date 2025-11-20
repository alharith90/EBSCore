using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowService(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<PagedResult<WorkflowDto>> GetWorkflowsAsync(WorkflowQueryDto query, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await _workflowRepository.GetWorkflowsAsync(query.Search, skip, pageSize, cancellationToken);
        var total = await _workflowRepository.CountWorkflowsAsync(query.Search, cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<WorkflowDto>(dtos, total);
    }

    public async Task<WorkflowDto?> GetWorkflowAsync(int workflowId, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetWorkflowAsync(workflowId, cancellationToken);
        return workflow == null ? null : MapToDetailedDto(workflow);
    }

    public async Task<int> CreateWorkflowAsync(WorkflowDto dto, string userName, CancellationToken cancellationToken)
    {
        var workflow = new Workflow
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        };

        var nodes = dto.Nodes.Select(n => new Node
        {
            Name = n.Name,
            NodeType = n.NodeType,
            ConfigJson = n.ConfigJson,
            PositionX = n.PositionX,
            PositionY = n.PositionY,
            CredentialId = n.CredentialId,
            RetryCount = n.RetryCount,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        var connections = dto.Connections.Select(c => new NodeConnection
        {
            SourceNodeId = c.SourceNodeId,
            SourceOutputKey = c.SourceOutputKey,
            TargetNodeId = c.TargetNodeId,
            TargetInputKey = c.TargetInputKey,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        var triggers = dto.Triggers.Select(t => new WorkflowTrigger
        {
            TriggerType = t.TriggerType,
            TriggerNodeId = t.TriggerNodeId,
            Secret = t.Secret,
            CronExpression = t.CronExpression,
            ConfigurationJson = t.ConfigurationJson,
            IsActive = t.IsActive,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        return await _workflowRepository.CreateWorkflowAsync(workflow, nodes.ToList(), connections.ToList(), triggers.ToList(), cancellationToken);
    }

    public async Task UpdateWorkflowAsync(int workflowId, WorkflowDto dto, string userName, CancellationToken cancellationToken)
    {
        var workflow = new Workflow
        {
            WorkflowId = workflowId,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        };

        var nodes = dto.Nodes.Select(n => new Node
        {
            WorkflowId = workflowId,
            Name = n.Name,
            NodeType = n.NodeType,
            ConfigJson = n.ConfigJson,
            PositionX = n.PositionX,
            PositionY = n.PositionY,
            CredentialId = n.CredentialId,
            RetryCount = n.RetryCount,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        var connections = dto.Connections.Select(c => new NodeConnection
        {
            WorkflowId = workflowId,
            SourceNodeId = c.SourceNodeId,
            SourceOutputKey = c.SourceOutputKey,
            TargetNodeId = c.TargetNodeId,
            TargetInputKey = c.TargetInputKey,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        var triggers = dto.Triggers.Select(t => new WorkflowTrigger
        {
            WorkflowId = workflowId,
            TriggerType = t.TriggerType,
            TriggerNodeId = t.TriggerNodeId,
            Secret = t.Secret,
            CronExpression = t.CronExpression,
            ConfigurationJson = t.ConfigurationJson,
            IsActive = t.IsActive,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        });

        await _workflowRepository.UpdateWorkflowAsync(workflow, nodes.ToList(), connections.ToList(), triggers.ToList(), cancellationToken);
    }

    public async Task SoftDeleteWorkflowAsync(int workflowId, string userName, CancellationToken cancellationToken)
    {
        await _workflowRepository.SoftDeleteWorkflowAsync(workflowId, userName, cancellationToken);
    }

    private static WorkflowDto MapToDto(Workflow workflow)
    {
        return new WorkflowDto
        {
            WorkflowId = workflow.WorkflowId,
            Name = workflow.Name,
            Description = workflow.Description,
            IsActive = workflow.IsActive
        };
    }

    private static WorkflowDto MapToDetailedDto(Workflow workflow)
    {
        var dto = MapToDto(workflow);
        dto.Nodes = workflow.Nodes.Select(n => new NodeDto
        {
            NodeId = n.NodeId,
            Name = n.Name,
            NodeType = n.NodeType,
            ConfigJson = n.ConfigJson,
            PositionX = n.PositionX,
            PositionY = n.PositionY,
            CredentialId = n.CredentialId,
            RetryCount = n.RetryCount
        }).ToList();

        dto.Connections = workflow.Connections.Select(c => new NodeConnectionDto
        {
            NodeConnectionId = c.NodeConnectionId,
            SourceNodeId = c.SourceNodeId,
            SourceOutputKey = c.SourceOutputKey,
            TargetNodeId = c.TargetNodeId,
            TargetInputKey = c.TargetInputKey
        }).ToList();

        dto.Triggers = workflow.Triggers.Select(t => new WorkflowTriggerDto
        {
            WorkflowTriggerId = t.WorkflowTriggerId,
            TriggerType = t.TriggerType,
            TriggerNodeId = t.TriggerNodeId,
            Secret = t.Secret,
            CronExpression = t.CronExpression,
            ConfigurationJson = t.ConfigurationJson,
            IsActive = t.IsActive
        }).ToList();

        return dto;
    }
}
