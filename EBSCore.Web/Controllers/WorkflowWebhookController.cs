using System.IO;
using System.Text;
using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBSCore.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/workflows/webhook")]
public class WorkflowWebhookController : ControllerBase
{
    private readonly IExecutionService _executionService;

    public WorkflowWebhookController(IExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpPost("{workflowId:int}/{secret}")]
    public async Task<ActionResult<object>> InvokeWebhook(int workflowId, string secret, CancellationToken cancellationToken)
    {
        Request.EnableBuffering();
        string body;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync(cancellationToken);
            Request.Body.Position = 0;
        }

        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

        var requestDto = new WebhookRequestDto
        {
            Headers = headers,
            Query = query,
            Body = body
        };

        var executionId = await _executionService.HandleWebhookAsync(workflowId, secret, requestDto, cancellationToken);
        return Ok(new { executionId });
    }
}
