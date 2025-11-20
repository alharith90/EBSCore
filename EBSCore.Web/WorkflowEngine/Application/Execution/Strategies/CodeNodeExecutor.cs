using System.Text.Json;
using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Application.Execution.Strategies;

public class CodeNodeExecutor : INodeExecutorStrategy
{
    public bool CanExecute(string nodeType) => string.Equals(nodeType, NodeType.Code, StringComparison.OrdinalIgnoreCase);

    public Task<NodeExecutionResult> ExecuteAsync(NodeExecutionContext context, CancellationToken cancellationToken)
    {
        var config = JsonSerializer.Deserialize<CodeNodeConfig>(context.Node.ConfigJson ?? "{}") ?? new CodeNodeConfig();
        var outputs = new Dictionary<string, object?>();

        if (config.MergeInputs)
        {
            foreach (var input in context.InputValues)
            {
                outputs[input.Key] = input.Value;
            }
        }

        if (config.Outputs != null)
        {
            foreach (var output in config.Outputs)
            {
                outputs[output.Key] = TemplateEvaluator.Render(output.Value, context);
            }
        }

        if (!string.IsNullOrWhiteSpace(config.FunctionName) && string.Equals(config.FunctionName, "Concat", StringComparison.OrdinalIgnoreCase))
        {
            var separator = config.Parameters?.GetValueOrDefault("separator") ?? string.Empty;
            var values = context.InputValues.Values.Select(v => v?.ToString() ?? string.Empty);
            outputs["result"] = string.Join(separator, values);
        }
        else if (!outputs.ContainsKey("result"))
        {
            outputs["result"] = outputs.Count > 0 ? outputs.Values.Last() : context.InputValues.FirstOrDefault().Value;
        }

        return Task.FromResult(new NodeExecutionResult
        {
            Success = true,
            Outputs = outputs
        });
    }

    private sealed class CodeNodeConfig
    {
        public string? FunctionName { get; set; }
        public bool MergeInputs { get; set; }
        public Dictionary<string, string>? Outputs { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
    }
}
