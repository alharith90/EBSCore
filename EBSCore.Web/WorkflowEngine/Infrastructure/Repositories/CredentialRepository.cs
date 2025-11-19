using Dapper;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Infrastructure.Repositories;

public class CredentialRepository : ICredentialRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CredentialRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsAsync(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT CredentialID   AS CredentialId,
                                   Name,
                                   CredentialType,
                                   DataJson,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SCredential
                            WHERE IsDeleted = 0
                            ORDER BY CredentialID DESC";

        await using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<Credential>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result.ToList();
    }

    public async Task<Credential?> GetCredentialAsync(int credentialId, CancellationToken cancellationToken)
    {
        const string sql = @"SELECT CredentialID   AS CredentialId,
                                   Name,
                                   CredentialType,
                                   DataJson,
                                   CreatedBy,
                                   CreatedAt,
                                   UpdatedBy,
                                   UpdatedAt,
                                   IsDeleted
                            FROM S7SCredential
                            WHERE CredentialID = @CredentialId AND IsDeleted = 0";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Credential>(new CommandDefinition(sql, new { CredentialId = credentialId }, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateCredentialAsync(Credential credential, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO S7SCredential (Name, CredentialType, DataJson, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted)
                             VALUES (@Name, @CredentialType, @DataJson, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 0);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

        await using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
        {
            credential.Name,
            credential.CredentialType,
            credential.DataJson,
            credential.CreatedBy,
            credential.CreatedAt,
            credential.UpdatedBy,
            credential.UpdatedAt
        }, cancellationToken: cancellationToken));
    }

    public async Task UpdateCredentialAsync(Credential credential, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE S7SCredential
                             SET Name = @Name,
                                 CredentialType = @CredentialType,
                                 DataJson = @DataJson,
                                 UpdatedBy = @UpdatedBy,
                                 UpdatedAt = @UpdatedAt
                             WHERE CredentialID = @CredentialId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            credential.CredentialId,
            credential.Name,
            credential.CredentialType,
            credential.DataJson,
            credential.UpdatedBy,
            credential.UpdatedAt
        }, cancellationToken: cancellationToken));
    }

    public async Task SoftDeleteCredentialAsync(int credentialId, string updatedBy, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE S7SCredential
                             SET IsDeleted = 1,
                                 UpdatedBy = @UpdatedBy,
                                 UpdatedAt = SYSUTCDATETIME()
                             WHERE CredentialID = @CredentialId";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { CredentialId = credentialId, UpdatedBy = updatedBy }, cancellationToken: cancellationToken));
    }
}
