using EBSCore.Web.WorkflowEngine.Domain.Entities;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface IExecutionRepository
{
    Task<long> InsertExecutionAsync(Execution execution, CancellationToken cancellationToken);
    Task UpdateExecutionStatusAsync(long executionId, ExecutionStatus status, string? errorMessage, DateTime? completedAt, CancellationToken cancellationToken);
    Task<long> InsertExecutionStepAsync(ExecutionStep step, CancellationToken cancellationToken);
    Task UpdateExecutionStepAsync(ExecutionStep step, CancellationToken cancellationToken);
    Task<IReadOnlyList<Execution>> GetExecutionsAsync(int workflowId, int skip, int take, CancellationToken cancellationToken);
    Task<int> CountExecutionsAsync(int workflowId, CancellationToken cancellationToken);
    Task<Execution?> GetExecutionAsync(long executionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExecutionStep>> GetExecutionStepsAsync(long executionId, CancellationToken cancellationToken);
}
