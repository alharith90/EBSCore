using System.Collections.Concurrent;
using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;
using EBSCore.Web.WorkflowEngine.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EBSCore.Web.WorkflowEngine.Application.Services;

public class WorkflowExecutor : IWorkflowExecutor
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IEnumerable<INodeExecutorStrategy> _strategies;
    private readonly IExecutionRepository _executionRepository;
    private readonly ILogger<WorkflowExecutor> _logger;

    public WorkflowExecutor(IEnumerable<INodeExecutorStrategy> strategies, IExecutionRepository executionRepository, ILogger<WorkflowExecutor> logger)
    {
        _strategies = strategies;
        _executionRepository = executionRepository;
        _logger = logger;
    }

    public async Task RunAsync(Workflow workflow, Execution execution, IDictionary<string, object?>? payload, CancellationToken cancellationToken)
    {
        await _executionRepository.UpdateExecutionStatusAsync(execution.ExecutionId, ExecutionStatus.Running, null, null, cancellationToken);

        var queue = new Queue<Node>(SelectStartNodes(workflow));
        if (queue.Count == 0)
        {
            _logger.LogWarning("Workflow {WorkflowId} has no nodes to execute.", workflow.WorkflowId);
            await _executionRepository.UpdateExecutionStatusAsync(execution.ExecutionId, ExecutionStatus.Succeeded, null, DateTime.UtcNow, cancellationToken);
            return;
        }

        var completedResults = new ConcurrentDictionary<int, NodeExecutionResult>();
        var visited = new HashSet<int>();

        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var node = queue.Dequeue();
            if (node == null || visited.Contains(node.NodeId))
            {
                continue;
            }

            visited.Add(node.NodeId);
            var now = DateTime.UtcNow;
            var step = new ExecutionStep
            {
                ExecutionId = execution.ExecutionId,
                NodeId = node.NodeId,
                Status = ExecutionStatus.Running.ToString(),
                StartedAt = now,
                CreatedAt = now,
                CreatedBy = execution.CreatedBy,
                UpdatedAt = now,
                UpdatedBy = execution.CreatedBy
            };
            step.ExecutionStepId = await _executionRepository.InsertExecutionStepAsync(step, cancellationToken);

            NodeExecutionResult result;
            try
            {
                var strategy = _strategies.FirstOrDefault(s => s.CanExecute(node.NodeType));
                if (strategy == null)
                {
                    result = new NodeExecutionResult
                    {
                        Success = false,
                        ErrorMessage = $"No executor registered for node type {node.NodeType}.",
                        Outputs = new Dictionary<string, object?>()
                    };
                }
                else
                {
                    var inputValues = BuildInputValues(node, workflow, completedResults);
                    var context = new NodeExecutionContext(workflow, node, execution.ExecutionId, completedResults, payload, inputValues);
                    result = await strategy.ExecuteAsync(context, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing node {NodeId} in workflow {WorkflowId}", node.NodeId, workflow.WorkflowId);
                result = new NodeExecutionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Outputs = new Dictionary<string, object?>()
                };
            }

            step.Status = result.Success ? ExecutionStatus.Succeeded.ToString() : ExecutionStatus.Failed.ToString();
            step.CompletedAt = DateTime.UtcNow;
            step.OutputJson = JsonSerializer.Serialize(result.Outputs, SerializerOptions);
            step.ErrorMessage = result.ErrorMessage;
            step.UpdatedAt = DateTime.UtcNow;
            step.UpdatedBy = execution.CreatedBy;
            await _executionRepository.UpdateExecutionStepAsync(step, cancellationToken);

            if (!result.Success)
            {
                await _executionRepository.UpdateExecutionStatusAsync(execution.ExecutionId, ExecutionStatus.Failed, result.ErrorMessage, DateTime.UtcNow, cancellationToken);
                return;
            }

            completedResults[node.NodeId] = result;

            foreach (var connection in workflow.Connections.Where(c => c.SourceNodeId == node.NodeId))
            {
                if (!string.IsNullOrWhiteSpace(result.SelectedOutputKey) && !string.Equals(connection.SourceOutputKey, result.SelectedOutputKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var targetNode = workflow.Nodes.FirstOrDefault(n => n.NodeId == connection.TargetNodeId);
                if (targetNode != null && !visited.Contains(targetNode.NodeId))
                {
                    queue.Enqueue(targetNode);
                }
            }
        }

        await _executionRepository.UpdateExecutionStatusAsync(execution.ExecutionId, ExecutionStatus.Succeeded, null, DateTime.UtcNow, cancellationToken);
    }

    private static IEnumerable<Node> SelectStartNodes(Workflow workflow)
    {
        var triggerNodeIds = workflow.Triggers.Where(t => t.IsActive && t.TriggerNodeId.HasValue).Select(t => t.TriggerNodeId!.Value).ToHashSet();
        var triggerNodes = workflow.Nodes.Where(n => triggerNodeIds.Contains(n.NodeId) || string.Equals(n.NodeType, NodeType.Trigger, StringComparison.OrdinalIgnoreCase)).ToList();
        if (triggerNodes.Count > 0)
        {
            return triggerNodes;
        }

        var nodesWithIncoming = workflow.Connections.Select(c => c.TargetNodeId).ToHashSet();
        var roots = workflow.Nodes.Where(n => !nodesWithIncoming.Contains(n.NodeId)).ToList();
        return roots.Count > 0 ? roots : workflow.Nodes;
    }

    private static IDictionary<string, object?> BuildInputValues(Node node, Workflow workflow, IReadOnlyDictionary<int, NodeExecutionResult> completedResults)
    {
        var inputs = new Dictionary<string, object?>();
        var inbound = workflow.Connections.Where(c => c.TargetNodeId == node.NodeId).ToList();
        var index = 0;
        foreach (var connection in inbound)
        {
            if (!completedResults.TryGetValue(connection.SourceNodeId, out var result))
            {
                continue;
            }

            var key = !string.IsNullOrWhiteSpace(connection.TargetInputKey)
                ? connection.TargetInputKey!
                : (!string.IsNullOrWhiteSpace(connection.SourceOutputKey) ? connection.SourceOutputKey! : $"input{index}");

            object? valueToStore = null;
            if (!string.IsNullOrWhiteSpace(connection.SourceOutputKey) && result.Outputs.TryGetValue(connection.SourceOutputKey, out var value))
            {
                valueToStore = value;
            }
            else if (result.Outputs.TryGetValue("result", out var defaultValue))
            {
                valueToStore = defaultValue;
            }
            else if (result.Outputs.Count > 0)
            {
                valueToStore = result.Outputs.First().Value;
            }

            if (valueToStore != null)
            {
                inputs[key] = valueToStore;
                var aliasKey = $"node{connection.SourceNodeId}.{connection.SourceOutputKey ?? "result"}";
                inputs[aliasKey] = valueToStore;
            }

            index++;
        }

        return inputs;
    }
}
