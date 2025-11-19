using EBSCore.Web.WorkflowEngine.Application.DTOs;

namespace EBSCore.Web.WorkflowEngine.Application.Interfaces;

public interface ICredentialService
{
    Task<IReadOnlyList<CredentialDto>> GetCredentialsAsync(CancellationToken cancellationToken);
    Task<CredentialDto?> GetCredentialAsync(int credentialId, CancellationToken cancellationToken);
    Task<int> CreateCredentialAsync(CredentialRequestDto request, string userName, CancellationToken cancellationToken);
    Task UpdateCredentialAsync(int credentialId, CredentialRequestDto request, string userName, CancellationToken cancellationToken);
    Task DeleteCredentialAsync(int credentialId, string userName, CancellationToken cancellationToken);
}
