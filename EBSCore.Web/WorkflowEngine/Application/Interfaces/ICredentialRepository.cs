using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface ICredentialRepository
{
    Task<IReadOnlyList<Credential>> GetCredentialsAsync(CancellationToken cancellationToken);
    Task<Credential?> GetCredentialAsync(int credentialId, CancellationToken cancellationToken);
    Task<int> CreateCredentialAsync(Credential credential, CancellationToken cancellationToken);
    Task UpdateCredentialAsync(Credential credential, CancellationToken cancellationToken);
    Task SoftDeleteCredentialAsync(int credentialId, string updatedBy, CancellationToken cancellationToken);
}
