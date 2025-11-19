using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using EBSCore.Web.WorkflowEngine.Domain.Entities;

namespace EBSCore.Web.WorkflowEngine.Application.Services;

public class CredentialService : ICredentialService
{
    private readonly ICredentialRepository _credentialRepository;

    public CredentialService(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public async Task<IReadOnlyList<CredentialDto>> GetCredentialsAsync(CancellationToken cancellationToken)
    {
        var credentials = await _credentialRepository.GetCredentialsAsync(cancellationToken);
        return credentials.Select(Map).ToList();
    }

    public async Task<CredentialDto?> GetCredentialAsync(int credentialId, CancellationToken cancellationToken)
    {
        var credential = await _credentialRepository.GetCredentialAsync(credentialId, cancellationToken);
        return credential == null ? null : Map(credential);
    }

    public async Task<int> CreateCredentialAsync(CredentialRequestDto request, string userName, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var credential = new Credential
        {
            Name = request.Name,
            CredentialType = request.CredentialType,
            DataJson = request.DataJson,
            CreatedBy = userName,
            CreatedAt = now,
            UpdatedBy = userName,
            UpdatedAt = now
        };

        return await _credentialRepository.CreateCredentialAsync(credential, cancellationToken);
    }

    public async Task UpdateCredentialAsync(int credentialId, CredentialRequestDto request, string userName, CancellationToken cancellationToken)
    {
        var credential = new Credential
        {
            CredentialId = credentialId,
            Name = request.Name,
            CredentialType = request.CredentialType,
            DataJson = request.DataJson,
            UpdatedBy = userName,
            UpdatedAt = DateTime.UtcNow
        };

        await _credentialRepository.UpdateCredentialAsync(credential, cancellationToken);
    }

    public async Task DeleteCredentialAsync(int credentialId, string userName, CancellationToken cancellationToken)
    {
        await _credentialRepository.SoftDeleteCredentialAsync(credentialId, userName, cancellationToken);
    }

    private static CredentialDto Map(Credential credential)
    {
        return new CredentialDto
        {
            CredentialId = credential.CredentialId,
            Name = credential.Name,
            CredentialType = credential.CredentialType,
            DataJson = credential.DataJson
        };
    }
}
