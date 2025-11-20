using System.Text.RegularExpressions;

namespace EBSCore.Web.WorkflowEngine.Application.Execution;

public static class TemplateEvaluator
{
    private static readonly Regex PlaceholderRegex = new("{{(.*?)}}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string Render(string? template, NodeExecutionContext context)
    {
        if (template is null)
        {
            return string.Empty;
        }

        return PlaceholderRegex.Replace(template, match =>
        {
            var key = match.Groups[1].Value.Trim();
            var value = ResolveValue(key, context);
            return value?.ToString() ?? string.Empty;
        });
    }

    public static object? ResolveValue(string key, NodeExecutionContext context)
    {
        if (context.InputValues.TryGetValue(key, out var value))
        {
            return value;
        }

        if (context.TriggerPayload.TryGetValue(key, out var triggerValue))
        {
            return triggerValue;
        }

        if (key.StartsWith("node", StringComparison.OrdinalIgnoreCase))
        {
            var segments = key.Split('.', 2);
            if (segments.Length == 2)
            {
                var nodePart = segments[0].AsSpan(4);
                if (int.TryParse(nodePart, out var nodeId))
                {
                    var property = segments[1];
                    var nodeValue = context.GetNodeOutput(nodeId, property);
                    if (nodeValue != null)
                    {
                        return nodeValue;
                    }
                }
            }
        }

        return key;
    }
}
