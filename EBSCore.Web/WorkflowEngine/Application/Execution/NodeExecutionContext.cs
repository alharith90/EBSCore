using System.Collections.Generic;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Execution;

public class NodeExecutionContext
{
    private readonly IReadOnlyDictionary<int, NodeExecutionResult> _previousResults;

    public NodeExecutionContext(Workflow workflow, Node node, long executionId, IReadOnlyDictionary<int, NodeExecutionResult> previousResults, IDictionary<string, object?>? triggerPayload, IDictionary<string, object?> inputValues)
    {
        Workflow = workflow;
        Node = node;
        ExecutionId = executionId;
        _previousResults = previousResults;
        TriggerPayload = triggerPayload ?? new Dictionary<string, object?>();
        InputValues = inputValues;
    }

    public Workflow Workflow { get; }
    public Node Node { get; }
    public long ExecutionId { get; }
    public IDictionary<string, object?> TriggerPayload { get; }
    public IDictionary<string, object?> InputValues { get; }

    public object? GetNodeOutput(int nodeId, string key)
    {
        if (_previousResults.TryGetValue(nodeId, out var result) && result.Outputs.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }
}
