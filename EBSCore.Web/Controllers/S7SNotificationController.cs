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
                var ds = _notificationSP.ListStatus(ChannelID, NotificationTemplateID, Sent, Success, Convert.ToInt32(_currentUser?.UserID));
                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
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
                var ds = _notificationSP.RtvTemplates(ChannelID, IsActive, Convert.ToInt32(_currentUser?.UserID));
                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
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
                var ds = _notificationSP.RtvTemplate(TemplateID, Convert.ToInt32(_currentUser?.UserID));
                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
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
                var ds = _notificationSP.SaveTemplate(template, Convert.ToInt32(_currentUser?.UserID));
                return Ok(JsonConvert.SerializeObject(ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable()));
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
                _notificationSP.DeleteTemplate(TemplateID, Convert.ToInt32(_currentUser?.UserID));
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
                var ds = _notificationSP.RtvChannels();
                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
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
                var ds = _notificationSP.RtvConnections(ChannelID);
                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
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
                var ds = _notificationSP.SaveStatus(status, Convert.ToInt32(_currentUser?.UserID));
                return Ok(JsonConvert.SerializeObject(ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable()));
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
                _notificationSP.MarkSent(NotificationStatusID, Convert.ToInt32(_currentUser?.UserID));
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
                _notificationSP.MarkFailed(NotificationStatusID, request?.ErrorMessage ?? string.Empty, request?.ErrorStack ?? string.Empty, Convert.ToInt32(_currentUser?.UserID));
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
