using EBSCore.AdoClass;
using EBSCore.Web.Models.BCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class S7SPlanController : ControllerBase
    {
        private readonly DBS7SPlan_SP _planSp;
        private readonly ILogger<S7SPlanController> _logger;

        public S7SPlanController(DBS7SPlan_SP planSp, ILogger<S7SPlanController> logger)
        {
            _planSp = planSp;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int? planId)
        {
            _logger.LogInformation("Fetching BCM plans {PlanId}", planId);
            var data = _planSp.QueryDatabase(SqlQueryType.FillDataset, "Get", UserID: User?.Identity?.Name ?? string.Empty, PlanID: planId?.ToString() ?? string.Empty);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Upsert([FromBody] S7SPlan plan)
        {
            _logger.LogInformation("Upserting BCM plan {Plan}", plan.PlanName);
            var result = _planSp.QueryDatabase(
                SqlQueryType.ExecuteNonQuery,
                "Upsert",
                UserID: User?.Identity?.Name ?? string.Empty,
                CompanyID: plan.CompanyID.ToString(),
                UnitID: plan.UnitID.ToString(),
                PlanID: plan.PlanID.ToString(),
                PlanCode: plan.PlanCode,
                PlanName: plan.PlanName,
                NextReviewDate: plan.NextReviewDate.ToString("O"),
                FrequencyMonths: plan.FrequencyMonths.ToString());

            return Ok(result);
        }
    }
}
