using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Application.Execution.Strategies;

public class ConditionalNodeExecutor : INodeExecutorStrategy
{
    public bool CanExecute(string nodeType) => string.Equals(nodeType, NodeType.Conditional, StringComparison.OrdinalIgnoreCase);

    public Task<NodeExecutionResult> ExecuteAsync(NodeExecutionContext context, CancellationToken cancellationToken)
    {
        var config = JsonSerializer.Deserialize<ConditionalConfig>(context.Node.ConfigJson ?? "{}") ?? new ConditionalConfig();
        var leftValue = TemplateEvaluator.ResolveValue(config.Left ?? string.Empty, context);
        var rightValue = TemplateEvaluator.ResolveValue(config.Right ?? string.Empty, context);

        var comparison = string.IsNullOrWhiteSpace(config.Operator)
            ? string.Equals(Convert.ToString(leftValue), Convert.ToString(rightValue), StringComparison.OrdinalIgnoreCase)
            : EvaluateOperator(config.Operator!, leftValue, rightValue);

        var outputs = new Dictionary<string, object?>
        {
            ["result"] = comparison,
            ["left"] = leftValue,
            ["right"] = rightValue
        };

        var selectedKey = comparison ? config.TrueOutputKey : config.FalseOutputKey;

        return Task.FromResult(new NodeExecutionResult
        {
            Success = true,
            Outputs = outputs,
            SelectedOutputKey = selectedKey
        });
    }

    private static bool EvaluateOperator(string op, object? left, object? right)
    {
        var leftString = Convert.ToString(left);
        var rightString = Convert.ToString(right);

        return op switch
        {
            "Equals" => string.Equals(leftString, rightString, StringComparison.OrdinalIgnoreCase),
            "NotEquals" => !string.Equals(leftString, rightString, StringComparison.OrdinalIgnoreCase),
            "GreaterThan" => CompareAsDouble(leftString, rightString) > 0,
            "LessThan" => CompareAsDouble(leftString, rightString) < 0,
            _ => string.Equals(leftString, rightString, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static int CompareAsDouble(string? left, string? right)
    {
        if (double.TryParse(left, out var leftDouble) && double.TryParse(right, out var rightDouble))
        {
            return leftDouble.CompareTo(rightDouble);
        }

        return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ConditionalConfig
    {
        public string? Left { get; set; }
        public string? Right { get; set; }
        public string? Operator { get; set; }
        public string? TrueOutputKey { get; set; } = "true";
        public string? FalseOutputKey { get; set; } = "false";
    }
}
