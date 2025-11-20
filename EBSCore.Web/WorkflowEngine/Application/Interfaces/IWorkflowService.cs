using EBSCore.Web.WorkflowEngine.Application.DTOs;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface IWorkflowService
{
    Task<PagedResult<WorkflowDto>> GetWorkflowsAsync(WorkflowQueryDto query, CancellationToken cancellationToken);
    Task<WorkflowDto?> GetWorkflowAsync(int workflowId, CancellationToken cancellationToken);
    Task<int> CreateWorkflowAsync(WorkflowDto dto, string userName, CancellationToken cancellationToken);
    Task UpdateWorkflowAsync(int workflowId, WorkflowDto dto, string userName, CancellationToken cancellationToken);
    Task SoftDeleteWorkflowAsync(int workflowId, string userName, CancellationToken cancellationToken);
}
