using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface IWorkflowExecutor
{
    Task RunAsync(Workflow workflow, Execution execution, IDictionary<string, object?>? payload, CancellationToken cancellationToken);
}
