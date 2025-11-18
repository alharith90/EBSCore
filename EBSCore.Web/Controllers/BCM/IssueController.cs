using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IssueController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBIssueSP IssueSP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public IssueController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            IssueSP = new DBIssueSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchFields = "", string? SearchQuery = "")
        {
            try
            {
                DataSet DSResult = (DataSet)IssueSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvIssues",
                    CurrentUserID: CurrentUser?.UserID.ToString(),
                    PageNumber: PageNumber,
                    PageSize: PageSize,
                    SearchFields: SearchFields,
                    SearchQuery: SearchQuery,
                    SortColumn: SortColumn,
                    SortDirection: SortDirection
                );

                var Result = new
                {
                    Data = DSResult.Tables[0],
                    PageCount = DSResult.Tables.Count > 1 ? DSResult.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(Result));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving issues");
            }
        }

        [HttpGet("{IssueID}")]
        public async Task<object> Get(int IssueID)
        {
            try
            {
                DataSet DSResult = (DataSet)IssueSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvIssue",
                    CurrentUserID: CurrentUser?.UserID.ToString(),
                    IssueID: IssueID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving issue");
            }
        }

        [HttpPost]
        public async Task<object> Save(Issue Issue)
        {
            try
            {
                object Result = IssueSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    IssueID: Issue.IssueID?.ToString(),
                    Description: Issue.Description,
                    Category: Issue.Category,
                    Source: Issue.Source,
                    Impact: Issue.Impact,
                    DateIdentified: Issue.DateIdentified?.ToString("yyyy-MM-dd"),
                    Owner: Issue.Owner,
                    Status: Issue.Status,
                    RootCause: Issue.RootCause,
                    CorrectiveAction: Issue.CorrectiveAction,
                    ActionOwner: Issue.ActionOwner,
                    ActionDueDate: Issue.ActionDueDate?.ToString("yyyy-MM-dd"),
                    ActionCompletionDate: Issue.ActionCompletionDate?.ToString("yyyy-MM-dd"),
                    VerificationOfEffectiveness: Issue.VerificationOfEffectiveness,
                    RelatedProcess: Issue.RelatedProcess,
                    AuditReference: Issue.AuditReference,
                    ReviewDate: Issue.ReviewDate?.ToString("yyyy-MM-dd"),
                    RiskCategory: Issue.RiskCategory,
                    Likelihood: Issue.Likelihood?.ToString(),
                    Consequence: Issue.Consequence?.ToString(),
                    DetectionMethod: Issue.DetectionMethod,
                    IssueType: Issue.IssueType,
                    LinkedBCP: Issue.LinkedBCP,
                    LessonsLearned: Issue.LessonsLearned,
                    IsRecurring: Issue.IsRecurring?.ToString(),
                    MitigationActions: Issue.MitigationActions,
                    EscalationLevel: Issue.EscalationLevel,
                    ClosureApprovedBy: Issue.ClosureApprovedBy,
                    ClosureComments: Issue.ClosureComments,
                    CurrentUserID: CurrentUser?.UserID.ToString()
                );

                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving issue");
            }
        }

        [HttpDelete("{IssueID}")]
        public async Task<object> Delete(int IssueID)
        {
            try
            {
                object Result = IssueSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    IssueID: IssueID.ToString(),
                    CurrentUserID: CurrentUser?.UserID.ToString()
                );

                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting issue");
            }
        }
    }
}
