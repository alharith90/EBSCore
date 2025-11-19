using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface IWorkflowRepository
{
    Task<IReadOnlyList<Workflow>> GetWorkflowsAsync(string? search, int skip, int take, CancellationToken cancellationToken);
    Task<int> CountWorkflowsAsync(string? search, CancellationToken cancellationToken);
    Task<Workflow?> GetWorkflowAsync(int workflowId, CancellationToken cancellationToken);
    Task<int> CreateWorkflowAsync(Workflow workflow, IEnumerable<Node> nodes, IEnumerable<NodeConnection> connections, IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken);
    Task UpdateWorkflowAsync(Workflow workflow, IEnumerable<Node> nodes, IEnumerable<NodeConnection> connections, IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken);
    Task SoftDeleteWorkflowAsync(int workflowId, string updatedBy, CancellationToken cancellationToken);
    Task<WorkflowTrigger?> GetActiveWebhookTriggerAsync(int workflowId, string secret, CancellationToken cancellationToken);
}
