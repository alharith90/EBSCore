using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Application.Execution.Strategies;

public class DelayNodeExecutor : INodeExecutorStrategy
{
    public bool CanExecute(string nodeType) => string.Equals(nodeType, NodeType.Delay, StringComparison.OrdinalIgnoreCase);

    public async Task<NodeExecutionResult> ExecuteAsync(NodeExecutionContext context, CancellationToken cancellationToken)
    {
        var config = JsonSerializer.Deserialize<DelayConfig>(context.Node.ConfigJson ?? "{}") ?? new DelayConfig();
        var delaySeconds = config.Seconds <= 0 ? 1 : config.Seconds;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);

        return new NodeExecutionResult
        {
            Success = true,
            Outputs = new Dictionary<string, object?>
            {
                ["result"] = $"Waited {delaySeconds} seconds",
                ["seconds"] = delaySeconds
            }
        };
    }

    private sealed class DelayConfig
    {
        public int Seconds { get; set; } = 1;
    }
}
