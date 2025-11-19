using System.Collections.Generic;

namespace EBSCore.Web.WorkflowEngine.Application.Execution;

public class NodeExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IDictionary<string, object?> Outputs { get; set; } = new Dictionary<string, object?>();
    public string? SelectedOutputKey { get; set; }
}
