using EBSCore.Web.WorkflowEngine.Application.DTOs;
using EBSCore.Web.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBSCore.Web.Controllers;

[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly IExecutionService _executionService;

    public WorkflowController(IWorkflowService workflowService, IExecutionService executionService)
    {
        _workflowService = workflowService;
        _executionService = executionService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<WorkflowDto>>> GetWorkflows([FromQuery] WorkflowQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _workflowService.GetWorkflowsAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{workflowId:int}")]
    public async Task<ActionResult<WorkflowDto>> GetWorkflow(int workflowId, CancellationToken cancellationToken)
    {
        var workflow = await _workflowService.GetWorkflowAsync(workflowId, cancellationToken);
        if (workflow == null)
        {
            return NotFound();
        }

        return Ok(workflow);
    }

    [HttpPost]
    public async Task<ActionResult> CreateWorkflow([FromBody] WorkflowDto workflowDto, CancellationToken cancellationToken)
    {
        var workflowId = await _workflowService.CreateWorkflowAsync(workflowDto, GetUserName(), cancellationToken);
        return CreatedAtAction(nameof(GetWorkflow), new { workflowId }, null);
    }

    [HttpPut("{workflowId:int}")]
    public async Task<IActionResult> UpdateWorkflow(int workflowId, [FromBody] WorkflowDto workflowDto, CancellationToken cancellationToken)
    {
        await _workflowService.UpdateWorkflowAsync(workflowId, workflowDto, GetUserName(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{workflowId:int}")]
    public async Task<IActionResult> DeleteWorkflow(int workflowId, CancellationToken cancellationToken)
    {
        await _workflowService.SoftDeleteWorkflowAsync(workflowId, GetUserName(), cancellationToken);
        return NoContent();
    }

    [HttpPost("{workflowId:int}/execute")]
    public async Task<ActionResult<object>> ExecuteWorkflow(int workflowId, [FromBody] ExecutionRequestDto request, CancellationToken cancellationToken)
    {
        var executionId = await _executionService.StartManualExecutionAsync(workflowId, request.Payload, GetUserName(), cancellationToken);
        return Ok(new { executionId });
    }

    [HttpGet("{workflowId:int}/executions")]
    public async Task<ActionResult<PagedResult<ExecutionDto>>> GetExecutions(int workflowId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _executionService.GetExecutionsAsync(workflowId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    private string GetUserName()
    {
        return User?.Identity?.Name ?? HttpContext?.Session?.GetString("User") ?? "system";
    }
}
