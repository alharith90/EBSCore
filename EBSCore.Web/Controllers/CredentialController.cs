using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBSCore.Web.Controllers;

[ApiController]
[Route("api/credentials")]
public class CredentialController : ControllerBase
{
    private readonly ICredentialService _credentialService;

    public CredentialController(ICredentialService credentialService)
    {
        _credentialService = credentialService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CredentialDto>>> GetCredentials(CancellationToken cancellationToken)
    {
        var credentials = await _credentialService.GetCredentialsAsync(cancellationToken);
        return Ok(credentials);
    }

    [HttpGet("{credentialId:int}")]
    public async Task<ActionResult<CredentialDto>> GetCredential(int credentialId, CancellationToken cancellationToken)
    {
        var credential = await _credentialService.GetCredentialAsync(credentialId, cancellationToken);
        if (credential == null)
        {
            return NotFound();
        }

        return Ok(credential);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCredential([FromBody] CredentialRequestDto request, CancellationToken cancellationToken)
    {
        var credentialId = await _credentialService.CreateCredentialAsync(request, GetUserName(), cancellationToken);
        return CreatedAtAction(nameof(GetCredential), new { credentialId }, null);
    }

    [HttpPut("{credentialId:int}")]
    public async Task<IActionResult> UpdateCredential(int credentialId, [FromBody] CredentialRequestDto request, CancellationToken cancellationToken)
    {
        await _credentialService.UpdateCredentialAsync(credentialId, request, GetUserName(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{credentialId:int}")]
    public async Task<IActionResult> DeleteCredential(int credentialId, CancellationToken cancellationToken)
    {
        await _credentialService.DeleteCredentialAsync(credentialId, GetUserName(), cancellationToken);
        return NoContent();
    }

    private string GetUserName()
    {
        return User?.Identity?.Name ?? HttpContext?.Session?.GetString("User") ?? "system";
    }
}
