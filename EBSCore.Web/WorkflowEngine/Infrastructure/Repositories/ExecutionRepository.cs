using System.Data;
using Dapper;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;
using EBSCore.Web.WorkflowEngine.Domain.Enums;

namespace EBSCore.Web.WorkflowEngine.Infrastructure.Repositories;

public class ExecutionRepository : IExecutionRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ExecutionRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> InsertExecutionAsync(Execution execution, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SExecution (WorkflowID, Status, TriggerType, TriggerDataJson, WebhookRequestJson, ErrorMessage, StartedAt, CompletedAt, RetryCount, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@WorkflowId, @Status, @TriggerType, @TriggerDataJson, @WebhookRequestJson, @ErrorMessage, @StartedAt, @CompletedAt, @RetryCount, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0);
                             SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
        {
            execution.WorkflowId,
            execution.Status,
            execution.TriggerType,
            execution.TriggerDataJson,
            execution.WebhookRequestJson,
            execution.ErrorMessage,
            execution.StartedAt,
            execution.CompletedAt,
            execution.RetryCount,
            execution.CreatedBy,
            execution.CreatedAt,
            execution.UpdatedBy,
            execution.UpdatedAt
        }, cancellationToken: cancellationToken));
    }

    public async Task UpdateExecutionStatusAsync(long executionId, ExecutionStatus status, string? errorMessage, DateTime? completedAt, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE S7SExecution
                             SET Status = @Status,
                                 ErrorMessage = @ErrorMessage,
                                 CompletedAt = @CompletedAt,
                                 UpdatedBy = 'system',
                                 UpdatedAt = SYSUTCDATETIME()
                             WHERE ExecutionID = @ExecutionId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            ExecutionId = executionId,
            Status = status.ToString(),
            ErrorMessage = errorMessage,
            CompletedAt = completedAt
        }, cancellationToken: cancellationToken));
    }

    public async Task<long> InsertExecutionStepAsync(ExecutionStep step, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SExecutionStep (ExecutionID, NodeID, Status, StartedAt, CompletedAt, RetryCount, OutputJson, ErrorMessage, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@ExecutionId, @NodeId, @Status, @StartedAt, @CompletedAt, @RetryCount, @OutputJson, @ErrorMessage, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0);
                             SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
        {
            step.ExecutionId,
            step.NodeId,
            step.Status,
            step.StartedAt,
            step.CompletedAt,
            step.RetryCount,
            step.OutputJson,
            step.ErrorMessage,
            step.CreatedBy,
            step.CreatedAt,
            step.UpdatedBy,
            step.UpdatedAt
        }, cancellationToken: cancellationToken));
    }

    public async Task UpdateExecutionStepAsync(ExecutionStep step, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE S7SExecutionStep
                             SET Status = @Status,
                                 CompletedAt = @CompletedAt,
                                 RetryCount = @RetryCount,
                                 OutputJson = @OutputJson,
                                 ErrorMessage = @ErrorMessage,
                                 UpdatedBy = @UpdatedBy,
                                 UpdatedAt = @UpdatedAt
                             WHERE ExecutionStepID = @ExecutionStepId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            step.Status,
            step.CompletedAt,
            step.RetryCount,
            step.OutputJson,
            step.ErrorMessage,
            step.UpdatedBy,
            step.UpdatedAt,
            step.ExecutionStepId
        }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Execution>> GetExecutionsAsync(int workflowId, int skip, int take, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT ExecutionID   AS ExecutionId,
                                   WorkflowID    AS WorkflowId,
                                   Status,
                                   TriggerType,
                                   TriggerDataJson,
                                   WebhookRequestJson,
                                   ErrorMessage,
                                   StartedAt,
                                   CompletedAt,
                                   RetryCount,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SExecution
                            WHERE WorkflowID = @WorkflowId AND IsDeleted = 0
                            ORDER BY ExecutionID DESC
                            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

        await using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<Execution>(new CommandDefinition(sql, new { WorkflowId = workflowId, Skip = skip, Take = take }, cancellationToken: cancellationToken));
        return result.ToList();
    }

    public async Task<int> CountExecutionsAsync(int workflowId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM S7SExecution WHERE WorkflowID = @WorkflowId AND IsDeleted = 0";
        await using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { WorkflowId = workflowId }, cancellationToken: cancellationToken));
    }

    public async Task<Execution?> GetExecutionAsync(long executionId, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT ExecutionID   AS ExecutionId,
                                   WorkflowID    AS WorkflowId,
                                   Status,
                                   TriggerType,
                                   TriggerDataJson,
                                   WebhookRequestJson,
                                   ErrorMessage,
                                   StartedAt,
                                   CompletedAt,
                                   RetryCount,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SExecution
                            WHERE ExecutionID = @ExecutionId AND IsDeleted = 0";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Execution>(new CommandDefinition(sql, new { ExecutionId = executionId }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<ExecutionStep>> GetExecutionStepsAsync(long executionId, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT ExecutionStepID AS ExecutionStepId,
                                   ExecutionID     AS ExecutionId,
                                   NodeID          AS NodeId,
                                   Status,
                                   StartedAt,
                                   CompletedAt,
                                   RetryCount,
                                   OutputJson,
                                   ErrorMessage,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SExecutionStep
                            WHERE ExecutionID = @ExecutionId AND IsDeleted = 0
                            ORDER BY ExecutionStepID";

        await using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<ExecutionStep>(new CommandDefinition(sql, new { ExecutionId = executionId }, cancellationToken: cancellationToken));
        return result.ToList();
    }
}
