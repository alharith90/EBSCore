using EBSCore.Web.WorkflowEngine.Application.DTOs;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface IExecutionService
{
    Task<long> StartManualExecutionAsync(int workflowId, IDictionary<string, object?>? payload, string userName, CancellationToken cancellationToken);
    Task<PagedResult<ExecutionDto>> GetExecutionsAsync(int workflowId, int page, int pageSize, CancellationToken cancellationToken);
    Task<ExecutionDetailDto?> GetExecutionAsync(long executionId, CancellationToken cancellationToken);
    Task<long> HandleWebhookAsync(int workflowId, string secret, WebhookRequestDto request, CancellationToken cancellationToken);
}
