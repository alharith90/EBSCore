using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EBSCore.Web.Controllers;

[ApiController]
[Route("api/executions")]
public class ExecutionController : ControllerBase
{
    private readonly IExecutionService _executionService;

    public ExecutionController(IExecutionService executionService)
    {
        _executionService = executionService;
    }

    [HttpGet("{executionId:long}")]
    public async Task<ActionResult<ExecutionDetailDto>> GetExecution(long executionId, CancellationToken cancellationToken)
    {
        var execution = await _executionService.GetExecutionAsync(executionId, cancellationToken);
        if (execution == null)
        {
            return NotFound();
        }

        return Ok(execution);
    }
}
