using EBSCore.Web.WorkflowEngine.Application.Execution;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface INodeExecutorStrategy
{
    bool CanExecute(string nodeType);
    Task<NodeExecutionResult> ExecuteAsync(NodeExecutionContext context, CancellationToken cancellationToken);
}
