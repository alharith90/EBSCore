using System.Data;
using Dapper;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public WorkflowRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Workflow>> GetWorkflowsAsync(string? search, int skip, int take, CancellationToken cancellationToken)
    {
        var sql = @"SELECT WorkflowID        AS WorkflowId,
                           Name,
                           Description,
                           IsActive,
                           CreatedBy,
                           CreatedAt,
                           UpdatedBy,
                           UpdatedAt,
                           IsDeleted
                    FROM S7SWorkflow
                    WHERE IsDeleted = 0
                      AND (@Search IS NULL OR Name LIKE @Search)
                    ORDER BY WorkflowID DESC
                    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

        var parameter = new
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%",
            Skip = skip,
            Take = take
        };

        await using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<Workflow>(new CommandDefinition(sql, parameter, cancellationToken: cancellationToken));
        return items.ToList();
    }

    public async Task<int> CountWorkflowsAsync(string? search, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM S7SWorkflow WHERE IsDeleted = 0 AND (@Search IS NULL OR Name LIKE @Search)";
        var parameter = new { Search = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%" };
        await using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, parameter, cancellationToken: cancellationToken));
    }

    public async Task<Workflow?> GetWorkflowAsync(int workflowId, CancellationToken cancellationToken)
    {
        var sql = @"SELECT WorkflowID        AS WorkflowId,
                           Name,
                           Description,
                           IsActive,
                           CreatedBy,
                           CreatedAt,
                           UpdatedBy,
                           UpdatedAt,
                           IsDeleted
                    FROM S7SWorkflow
                    WHERE WorkflowID = @WorkflowId AND IsDeleted = 0;

                    SELECT NodeID          AS NodeId,
                           WorkflowID      AS WorkflowId,
                           Name,
                           NodeType,
                           ConfigJson,
                           PositionX,
                           PositionY,
                           CredentialID    AS CredentialId,
                           RetryCount,
                           CreatedBy,
                           CreatedAt,
                           UpdatedBy,
                           UpdatedAt,
                           IsDeleted
                    FROM S7SNode
                    WHERE WorkflowID = @WorkflowId AND IsDeleted = 0;

                    SELECT NodeConnectionID AS NodeConnectionId,
                           WorkflowID       AS WorkflowId,
                           SourceNodeID     AS SourceNodeId,
                           SourceOutputKey,
                           TargetNodeID     AS TargetNodeId,
                           TargetInputKey,
                           CreatedBy,
                           CreatedAt,
                           UpdatedBy,
                           UpdatedAt,
                           IsDeleted
                    FROM S7SNodeConnection
                    WHERE WorkflowID = @WorkflowId AND IsDeleted = 0;

                    SELECT WorkflowTriggerID AS WorkflowTriggerId,
                           WorkflowID        AS WorkflowId,
                           TriggerType,
                           TriggerNodeID     AS TriggerNodeId,
                           Secret,
                           CronExpression,
                           ConfigurationJson,
                           IsActive,
                           CreatedBy,
                           CreatedAt,
                           UpdatedBy,
                           UpdatedAt,
                           IsDeleted
                    FROM S7SWorkflowTrigger
                    WHERE WorkflowID = @WorkflowId AND IsDeleted = 0;";

        await using var connection = _connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, new { WorkflowId = workflowId }, cancellationToken: cancellationToken));

        var workflow = await multi.ReadSingleOrDefaultAsync<Workflow>();
        if (workflow == null)
        {
            return null;
        }

        var nodes = (await multi.ReadAsync<Node>()).ToList();
        var connections = (await multi.ReadAsync<NodeConnection>()).ToList();
        var triggers = (await multi.ReadAsync<WorkflowTrigger>()).ToList();

        foreach (var node in nodes)
        {
            workflow.Nodes.Add(node);
        }

        foreach (var connectionRow in connections)
        {
            workflow.Connections.Add(connectionRow);
        }

        foreach (var trigger in triggers)
        {
            workflow.Triggers.Add(trigger);
        }

        return workflow;
    }

    public async Task<int> CreateWorkflowAsync(Workflow workflow, IEnumerable<Node> nodes, IEnumerable<NodeConnection> connections, IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken)
    {
        const string workflowSql = @"INSERT INTO S7SWorkflow (Name, Description, IsActive, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                                     VALUES (@Name, @Description, @IsActive, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = connection.BeginTransaction();

        try
        {
            var workflowId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(workflowSql, new
            {
                workflow.Name,
                workflow.Description,
                workflow.IsActive,
                workflow.CreatedBy,
                workflow.CreatedAt,
                workflow.UpdatedBy,
                workflow.UpdatedAt
            }, transaction, cancellationToken: cancellationToken));

            await InsertNodesAsync(connection, transaction, workflowId, nodes, cancellationToken);
            await InsertConnectionsAsync(connection, transaction, workflowId, connections, cancellationToken);
            await InsertTriggersAsync(connection, transaction, workflowId, triggers, cancellationToken);

            transaction.Commit();
            return workflowId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateWorkflowAsync(Workflow workflow, IEnumerable<Node> nodes, IEnumerable<NodeConnection> connections, IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken)
    {
        const string updateSql = @"UPDATE S7SWorkflow
                                    SET Name = @Name,
                                        Description = @Description,
                                        IsActive = @IsActive,
                                        UpdatedBy = @UpdatedBy,
                                        UpdatedAt = @UpdatedAt
                                    WHERE WorkflowID = @WorkflowId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(new CommandDefinition(updateSql, new
            {
                WorkflowId = workflow.WorkflowId,
                workflow.Name,
                workflow.Description,
                workflow.IsActive,
                workflow.UpdatedBy,
                workflow.UpdatedAt
            }, transaction, cancellationToken: cancellationToken));

            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM S7SNodeConnection WHERE WorkflowID = @WorkflowId", new { WorkflowId = workflow.WorkflowId }, transaction, cancellationToken: cancellationToken));
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM S7SWorkflowTrigger WHERE WorkflowID = @WorkflowId", new { WorkflowId = workflow.WorkflowId }, transaction, cancellationToken: cancellationToken));
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM S7SNode WHERE WorkflowID = @WorkflowId", new { WorkflowId = workflow.WorkflowId }, transaction, cancellationToken: cancellationToken));

            await InsertNodesAsync(connection, transaction, workflow.WorkflowId, nodes, cancellationToken);
            await InsertConnectionsAsync(connection, transaction, workflow.WorkflowId, connections, cancellationToken);
            await InsertTriggersAsync(connection, transaction, workflow.WorkflowId, triggers, cancellationToken);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task SoftDeleteWorkflowAsync(int workflowId, string updatedBy, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE S7SWorkflow
                             SET IsDeleted = 1,
                                 UpdatedBy = @UpdatedBy,
                                 UpdatedAt = SYSUTCDATETIME()
                             WHERE WorkflowID = @WorkflowId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { WorkflowId = workflowId, UpdatedBy = updatedBy }, cancellationToken: cancellationToken));
    }

    public async Task<WorkflowTrigger?> GetActiveWebhookTriggerAsync(int workflowId, string secret, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT WorkflowTriggerID AS WorkflowTriggerId,
                                   WorkflowID        AS WorkflowId,
                                   TriggerType,
                                   TriggerNodeID     AS TriggerNodeId,
                                   Secret,
                                   CronExpression,
                                   ConfigurationJson,
                                   IsActive,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SWorkflowTrigger
                            WHERE WorkflowID = @WorkflowId
                              AND Secret = @Secret
                              AND TriggerType = 'Webhook'
                              AND IsActive = 1
                              AND IsDeleted = 0";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<WorkflowTrigger>(new CommandDefinition(sql, new { WorkflowId = workflowId, Secret = secret }, cancellationToken: cancellationToken));
    }

    private static async Task InsertNodesAsync(IDbConnection connection, IDbTransaction transaction, int workflowId, IEnumerable<Node> nodes, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SNode (WorkflowID, Name, NodeType, ConfigJson, PositionX, PositionY, CredentialID, RetryCount, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@WorkflowId, @Name, @NodeType, @ConfigJson, @PositionX, @PositionY, @CredentialId, @RetryCount, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0)";

        foreach (var node in nodes)
        {
            var parameters = new
            {
                WorkflowId = workflowId,
                node.Name,
                node.NodeType,
                node.ConfigJson,
                node.PositionX,
                node.PositionY,
                node.CredentialId,
                node.RetryCount,
                node.CreatedBy,
                node.CreatedAt,
                node.UpdatedBy,
                node.UpdatedAt
            };
            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
        }
    }

    private static async Task InsertConnectionsAsync(IDbConnection connection, IDbTransaction transaction, int workflowId, IEnumerable<NodeConnection> connections, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SNodeConnection (WorkflowID, SourceNodeID, SourceOutputKey, TargetNodeID, TargetInputKey, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@WorkflowId, @SourceNodeId, @SourceOutputKey, @TargetNodeId, @TargetInputKey, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0)";

        foreach (var connectionRow in connections)
        {
            var parameters = new
            {
                WorkflowId = workflowId,
                connectionRow.SourceNodeId,
                connectionRow.SourceOutputKey,
                connectionRow.TargetNodeId,
                connectionRow.TargetInputKey,
                connectionRow.CreatedBy,
                connectionRow.CreatedAt,
                connectionRow.UpdatedBy,
                connectionRow.UpdatedAt
            };
            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
        }
    }

    private static async Task InsertTriggersAsync(IDbConnection connection, IDbTransaction transaction, int workflowId, IEnumerable<WorkflowTrigger> triggers, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SWorkflowTrigger (WorkflowID, TriggerType, TriggerNodeID, Secret, CronExpression, ConfigurationJson, IsActive, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@WorkflowId, @TriggerType, @TriggerNodeId, @Secret, @CronExpression, @ConfigurationJson, @IsActive, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0)";

        foreach (var trigger in triggers)
        {
            var parameters = new
            {
                WorkflowId = workflowId,
                trigger.TriggerType,
                trigger.TriggerNodeId,
                trigger.Secret,
                trigger.CronExpression,
                trigger.ConfigurationJson,
                trigger.IsActive,
                trigger.CreatedBy,
                trigger.CreatedAt,
                trigger.UpdatedBy,
                trigger.UpdatedAt
            };
            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
        }
    }
}
