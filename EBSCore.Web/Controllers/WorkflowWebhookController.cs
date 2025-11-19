using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/workflows/webhook")]
    public class WorkflowWebhookController : ControllerBase
    {
        private readonly DBWorkflowExecutionSP _executionSP;
        private readonly Common _common;

        public WorkflowWebhookController(IConfiguration configuration)
        {
            _executionSP = new DBWorkflowExecutionSP(configuration);
            _common = new Common();
        }

        [HttpPost("{workflowId:int}/{secret}")]
        public async Task<IActionResult> Invoke(int workflowId, string secret)
        {
            try
            {
                Request.EnableBuffering();
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                }

                var payload = new
                {
                    Headers = Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    Query = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    Body = body
                };

                var executionId = _executionSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                    Operation: "StartWebhookExecution",
                    WorkflowID: workflowId.ToString(),
                    WebhookSecret: secret,
                    RequestJson: JsonConvert.SerializeObject(payload)
                );

                return Ok(new { ExecutionID = executionId });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Webhook trigger failed");
            }
        }
    }
}
