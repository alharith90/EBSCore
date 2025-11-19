using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Application.Services;

public class ExecutionService : IExecutionService
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IWorkflowRepository _workflowRepository;
    private readonly IExecutionRepository _executionRepository;
    private readonly IWorkflowExecutor _workflowExecutor;

    public ExecutionService(IWorkflowRepository workflowRepository, IExecutionRepository executionRepository, IWorkflowExecutor workflowExecutor)
    {
        _workflowRepository = workflowRepository;
        _executionRepository = executionRepository;
        _workflowExecutor = workflowExecutor;
    }

    public async Task<long> StartManualExecutionAsync(int workflowId, IDictionary<string, object?>? payload, string userName, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetWorkflowAsync(workflowId, cancellationToken) ?? throw new InvalidOperationException($"Workflow {workflowId} was not found.");
        var execution = CreateExecution(workflowId, TriggerType.Manual, payload, null, userName);
        execution.ExecutionId = await _executionRepository.InsertExecutionAsync(execution, cancellationToken);
        await _workflowExecutor.RunAsync(workflow, execution, payload, cancellationToken);
        return execution.ExecutionId;
    }

    public async Task<PagedResult<ExecutionDto>> GetExecutionsAsync(int workflowId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var normalizedPage = page <= 0 ? 1 : page;
        var normalizedSize = pageSize <= 0 ? 20 : pageSize;
        var skip = (normalizedPage - 1) * normalizedSize;

        var executions = await _executionRepository.GetExecutionsAsync(workflowId, skip, normalizedSize, cancellationToken);
        var total = await _executionRepository.CountExecutionsAsync(workflowId, cancellationToken);

        var dtos = executions.Select(MapExecutionDto).ToList();
        return new PagedResult<ExecutionDto>(dtos, total);
    }

    public async Task<ExecutionDetailDto?> GetExecutionAsync(long executionId, CancellationToken cancellationToken)
    {
        var execution = await _executionRepository.GetExecutionAsync(executionId, cancellationToken);
        if (execution == null)
        {
            return null;
        }

        var steps = await _executionRepository.GetExecutionStepsAsync(executionId, cancellationToken);
        var detail = MapExecutionDetailDto(execution);
        detail.Steps = steps.Select(step => new ExecutionStepDto
        {
            ExecutionStepId = step.ExecutionStepId,
            NodeId = step.NodeId,
            Status = step.Status,
            StartedAt = step.StartedAt,
            CompletedAt = step.CompletedAt,
            OutputJson = step.OutputJson,
            ErrorMessage = step.ErrorMessage
        }).ToList();

        return detail;
    }

    public async Task<long> HandleWebhookAsync(int workflowId, string secret, WebhookRequestDto request, CancellationToken cancellationToken)
    {
        var trigger = await _workflowRepository.GetActiveWebhookTriggerAsync(workflowId, secret, cancellationToken);
        if (trigger == null)
        {
            throw new InvalidOperationException("Invalid webhook secret or workflow not enabled for webhooks.");
        }

        var workflow = await _workflowRepository.GetWorkflowAsync(workflowId, cancellationToken) ?? throw new InvalidOperationException($"Workflow {workflowId} was not found.");
        var payload = new Dictionary<string, object?>
        {
            ["workflowTriggerId"] = trigger.WorkflowTriggerId,
            ["secret"] = trigger.Secret
        };

        var serializedRequest = JsonSerializer.Serialize(request, SerializerOptions);
        var webhookUser = "webhook";
        if (request.Headers != null && request.Headers.TryGetValue("x-user", out var headerUser) && !string.IsNullOrWhiteSpace(headerUser))
        {
            webhookUser = headerUser;
        }

        var execution = CreateExecution(workflowId, TriggerType.Webhook, payload, serializedRequest, webhookUser);
        execution.ExecutionId = await _executionRepository.InsertExecutionAsync(execution, cancellationToken);
        await _workflowExecutor.RunAsync(workflow, execution, payload, cancellationToken);
        return execution.ExecutionId;
    }

    private static Execution CreateExecution(int workflowId, TriggerType triggerType, IDictionary<string, object?>? payload, string? webhookRequestJson, string userName)
    {
        var now = DateTime.UtcNow;
        return new Execution
        {
            WorkflowId = workflowId,
            Status = ExecutionStatus.Pending.ToString(),
            TriggerType = triggerType.ToString(),
            TriggerDataJson = payload == null ? null : JsonSerializer.Serialize(payload, SerializerOptions),
            WebhookRequestJson = webhookRequestJson,
            StartedAt = now,
            CreatedAt = now,
            CreatedBy = userName,
            UpdatedAt = now,
            UpdatedBy = userName
        };
    }

    private static ExecutionDto MapExecutionDto(Execution execution)
    {
        return new ExecutionDto
        {
            ExecutionId = execution.ExecutionId,
            WorkflowId = execution.WorkflowId,
            Status = execution.Status,
            TriggerType = execution.TriggerType,
            StartedAt = execution.StartedAt,
            CompletedAt = execution.CompletedAt,
            ErrorMessage = execution.ErrorMessage
        };
    }

    private static ExecutionDetailDto MapExecutionDetailDto(Execution execution)
    {
        var dto = new ExecutionDetailDto
        {
            ExecutionId = execution.ExecutionId,
            WorkflowId = execution.WorkflowId,
            Status = execution.Status,
            TriggerType = execution.TriggerType,
            StartedAt = execution.StartedAt,
            CompletedAt = execution.CompletedAt,
            ErrorMessage = execution.ErrorMessage,
            TriggerDataJson = execution.TriggerDataJson,
            WebhookRequestJson = execution.WebhookRequestJson
        };
        return dto;
    }
}
