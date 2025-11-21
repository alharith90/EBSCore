using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SNotificationController : ControllerBase
    {
        private readonly DBS7SNotificationSP _notificationSP;
        private readonly Common _common;
        private readonly User? _currentUser;

        public S7SNotificationController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _notificationSP = new DBS7SNotificationSP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("S7S Notification engine API ready");
        }

        [HttpGet]
        public async Task<object> RtvStatusList(int? ChannelID = null, int? NotificationTemplateID = null, bool? Sent = null, bool? Success = null)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "GetStatus",
                    ChannelID: ChannelID?.ToString() ?? string.Empty,
                    NotificationTemplateID: NotificationTemplateID?.ToString() ?? string.Empty,
                    PageNumber: "1",
                    PageSize: "100");

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving notifications");
            }
        }

        [HttpGet]
        public async Task<object> RtvTemplates(int? ChannelID = null, bool? IsActive = true)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplates",
                    ChannelID: ChannelID?.ToString() ?? string.Empty,
                    CompanyID: _currentUser?.CompanyID ?? string.Empty,
                    IsActive: IsActive.HasValue ? IsActive.Value.ToString() : string.Empty);

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving templates");
            }
        }

        [HttpGet("{TemplateID}")]
        public async Task<object> RtvTemplate(int TemplateID)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplate",
                    NotificationTemplateID: TemplateID.ToString());

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving template");
            }
        }

        [HttpPost]
        public async Task<object> SaveTemplate([FromBody] S7SNotificationTemplate template)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveTemplate",
                    NotificationTemplateID: template.NotificationTemplateID?.ToString() ?? string.Empty,
                    TemplateKey: template.TemplateKey,
                    Name: template.Name,
                    ChannelID: template.ChannelID.ToString(),
                    Subject: template.Subject,
                    Body: template.Body,
                    UseDesign: template.UseDesign.ToString(),
                    Attachments: template.Attachments,
                    Description: string.Empty,
                    CompanyID: template.CompanyID?.ToString() ?? string.Empty,
                    IsActive: template.IsActive.ToString(),
                    CreatedBy: _currentUser?.UserID ?? string.Empty,
                    UpdatedBy: _currentUser?.UserID ?? string.Empty);

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving template");
            }
        }

        [HttpDelete("{TemplateID}")]
        public async Task<object> DeleteTemplate(int TemplateID)
        {
            try
            {
                _notificationSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteTemplate",
                    NotificationTemplateID: TemplateID.ToString(),
                    UpdatedBy: _currentUser?.UserID ?? string.Empty);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error deleting template");
            }
        }

        [HttpGet]
        public async Task<object> RtvChannels()
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplates",
                    IsActive: true.ToString());

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving channels");
            }
        }

        [HttpGet]
        public async Task<object> RtvConnections(int? ChannelID = null)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplates",
                    ChannelID: ChannelID?.ToString() ?? string.Empty);

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving connections");
            }
        }

        [HttpPost]
        public async Task<object> SaveStatus([FromBody] S7SNotificationStatus status)
        {
            try
            {
                var ds = (DataSet)_notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveStatus",
                    NotificationTemplateID: status.NotificationTemplateID.ToString(),
                    Email: status.Email,
                    CCEmails: status.CCEmails,
                    BCCEmails: status.BCCEmails,
                    CountryCode: status.CountryCode,
                    MobileNo: status.MobileNo,
                    ToUserID: status.ToUserID?.ToString() ?? string.Empty,
                    ChannelID: status.ChannelID.ToString(),
                    ConnectionID: status.ConnectionID?.ToString() ?? string.Empty,
                    TryDate: status.TryDate?.ToString() ?? string.Empty,
                    MaxTry: status.MaxTry.ToString(),
                    Priority: status.Priority.ToString(),
                    ScheduledAt: status.ScheduledAt?.ToString() ?? string.Empty,
                    ExceptionID: status.ExceptionID?.ToString() ?? string.Empty,
                    ErrorMessage: status.ErrorMessage,
                    ErrorStack: status.ErrorStack,
                    CreatedBy: _currentUser?.UserID ?? string.Empty,
                    PayloadJson: status.PayloadJson ?? string.Empty);

                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving status");
            }
        }

        [HttpPost("{NotificationStatusID}/sent")]
        public async Task<object> MarkSent(int NotificationStatusID)
        {
            try
            {
                _notificationSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "MarkSent",
                    NotificationStatusID: NotificationStatusID.ToString(),
                    UpdatedBy: _currentUser?.UserID ?? string.Empty);
                return Ok("Notification marked as sent");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error marking sent");
            }
        }

        public class FailureRequest
        {
            public string? ErrorMessage { get; set; }
            public string? ErrorStack { get; set; }
        }

        [HttpPost("{NotificationStatusID}/failed")]
        public async Task<object> MarkFailed(int NotificationStatusID, [FromBody] FailureRequest request)
        {
            try
            {
                _notificationSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "MarkFailed",
                    NotificationStatusID: NotificationStatusID.ToString(),
                    ErrorMessage: request?.ErrorMessage ?? string.Empty,
                    ErrorStack: request?.ErrorStack ?? string.Empty,
                    UpdatedBy: _currentUser?.UserID ?? string.Empty);
                return Ok("Notification marked as failed");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error marking failed");
            }
        }
    }
}
