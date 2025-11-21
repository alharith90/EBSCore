using Dapper;
using EBSCore.AdoClass;
using EBSCore.AdoClass.Notification;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using NotificationTemplate = EBSCore.Web.Models.Notification.S7SNotificationTemplate;
using NotificationStatus = EBSCore.Web.Models.Notification.S7SNotificationStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.Notification
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SNotificationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DBS7SNotificationSP notificationSP;
        private readonly Common common;
        private readonly User currentUser;

        public S7SNotificationController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            notificationSP = new DBS7SNotificationSP(configuration);
            currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            common = new Common();
        }

        [HttpGet]
        public async Task<object> Index(string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchQuery = "")
        {
            try
            {
                DataSet ds = notificationSP.GetStatus(PageNumber ?? "1", PageSize ?? "10", SortColumn ?? string.Empty, SortDirection ?? string.Empty, SearchQuery ?? string.Empty);
                var result = new
                {
                    Data = ds.Tables.Count > 0 ? ds.Tables[0] : null,
                    PageCount = ds.Tables.Count > 1 ? ds.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving notifications");
            }
        }

        [HttpGet]
        public async Task<object> Templates()
        {
            try
            {
                var table = notificationSP.RtvTemplates();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving templates");
            }
        }

        [HttpGet("{NotificationTemplateID}")]
        public async Task<object> Template(int NotificationTemplateID)
        {
            try
            {
                var row = notificationSP.RtvTemplate(NotificationTemplateID.ToString());
                return Ok(JsonConvert.SerializeObject(row?.Table));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving template");
            }
        }

        [HttpPost]
        public async Task<object> SaveTemplate(NotificationTemplate model)
        {
            try
            {
                var row = notificationSP.SaveTemplate(
                    model.NotificationTemplateID?.ToString() ?? string.Empty,
                    model.TemplateKey ?? string.Empty,
                    model.Name ?? string.Empty,
                    model.ChannelID?.ToString() ?? string.Empty,
                    model.Subject ?? string.Empty,
                    model.Body ?? string.Empty,
                    model.UseDesign?.ToString() ?? string.Empty,
                    model.Attachments ?? string.Empty,
                    model.Description ?? string.Empty,
                    model.CompanyID?.ToString() ?? string.Empty,
                    model.IsActive?.ToString() ?? string.Empty,
                    currentUser?.UserID?.ToString() ?? string.Empty
                );

                return Ok(JsonConvert.SerializeObject(row?.Table ?? new DataTable()));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving template");
            }
        }

        [HttpDelete("{NotificationTemplateID}")]
        public async Task<object> DeleteTemplate(int NotificationTemplateID)
        {
            try
            {
                notificationSP.DeleteTemplate(NotificationTemplateID.ToString(), currentUser?.UserID?.ToString() ?? string.Empty);
                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error deleting template");
            }
        }

        [HttpPost]
        public async Task<object> SaveStatus(NotificationStatus status)
        {
            try
            {
                var row = notificationSP.SaveStatus(
                    status.NotificationTemplateID?.ToString() ?? string.Empty,
                    status.Email ?? string.Empty,
                    status.CCEmails ?? string.Empty,
                    status.BCCEmails ?? string.Empty,
                    status.CountryCode?.ToString() ?? string.Empty,
                    status.MobileNo ?? string.Empty,
                    status.ToUserID?.ToString() ?? string.Empty,
                    status.ChannelID?.ToString() ?? string.Empty,
                    status.ConnectionID?.ToString() ?? string.Empty,
                    status.Priority?.ToString() ?? string.Empty,
                    status.ScheduledAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                    status.ExceptionID?.ToString() ?? string.Empty,
                    status.ErrorMessage ?? string.Empty,
                    status.ErrorStack ?? string.Empty
                );

                return Ok(JsonConvert.SerializeObject(row?.Table ?? new DataTable()));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving notification");
            }
        }

        [HttpGet]
        public async Task<object> Channels()
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                var channels = await conn.QueryAsync<S7SNotificationChannel>("SELECT * FROM S7SNotificationChannel WHERE IsActive = 1 ORDER BY Name");
                return Ok(JsonConvert.SerializeObject(channels));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving channels");
            }
        }

        [HttpGet]
        public async Task<object> Connections()
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                var connections = await conn.QueryAsync<S7SNotificationConnection>("SELECT * FROM S7SNotificationConnection WHERE IsActive = 1 ORDER BY Name");
                return Ok(JsonConvert.SerializeObject(connections));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving connections");
            }
        }

        [HttpPost]
        public async Task<object> SaveConnection(S7SNotificationConnection connection)
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                if (connection.NotificationConnectionID == null)
                {
                    var insertSql = @"INSERT INTO S7SNotificationConnection (ChannelID, Name, ProviderType, ConfigurationJson, IsDefault, IsActive, CompanyID, CreatedBy)
                                      VALUES (@ChannelID, @Name, @ProviderType, @ConfigurationJson, ISNULL(@IsDefault,0), ISNULL(@IsActive,1), @CompanyID, @CreatedBy);
                                      SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    var id = await conn.ExecuteScalarAsync<int>(insertSql, new
                    {
                        connection.ChannelID,
                        connection.Name,
                        connection.ProviderType,
                        connection.ConfigurationJson,
                        connection.IsDefault,
                        connection.IsActive,
                        connection.CompanyID,
                        CreatedBy = currentUser?.UserID
                    });
                    connection.NotificationConnectionID = id;
                }
                else
                {
                    var updateSql = @"UPDATE S7SNotificationConnection
                                      SET ChannelID = @ChannelID,
                                          Name = @Name,
                                          ProviderType = @ProviderType,
                                          ConfigurationJson = @ConfigurationJson,
                                          IsDefault = ISNULL(@IsDefault, IsDefault),
                                          IsActive = ISNULL(@IsActive, IsActive),
                                          CompanyID = @CompanyID,
                                          UpdatedBy = @UpdatedBy,
                                          UpdatedAt = SYSUTCDATETIME()
                                      WHERE NotificationConnectionID = @NotificationConnectionID";
                    await conn.ExecuteAsync(updateSql, new
                    {
                        connection.ChannelID,
                        connection.Name,
                        connection.ProviderType,
                        connection.ConfigurationJson,
                        connection.IsDefault,
                        connection.IsActive,
                        connection.CompanyID,
                        connection.NotificationConnectionID,
                        UpdatedBy = currentUser?.UserID
                    });
                }

                return Ok(JsonConvert.SerializeObject(connection));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving connection");
            }
        }

        [HttpDelete("{NotificationConnectionID}")]
        public async Task<object> DeleteConnection(int NotificationConnectionID)
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.ExecuteAsync("UPDATE S7SNotificationConnection SET IsActive = 0, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UpdatedBy WHERE NotificationConnectionID = @NotificationConnectionID", new { NotificationConnectionID, UpdatedBy = currentUser?.UserID });
                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error deleting connection");
            }
        }

        [HttpGet]
        public async Task<object> History(int? NotificationStatusID = null, string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchQuery = "")
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@Operation", "List");
                parameters.Add("@NotificationStatusID", NotificationStatusID);
                parameters.Add("@PageNumber", PageNumber);
                parameters.Add("@PageSize", PageSize);
                parameters.Add("@SortColumn", SortColumn);
                parameters.Add("@SortDirection", SortDirection);
                parameters.Add("@SearchQuery", SearchQuery);
                using var reader = await conn.QueryMultipleAsync("S7SNotificationHistorySP", parameters, commandType: CommandType.StoredProcedure);
                var data = await reader.ReadAsync();
                var page = await reader.ReadFirstOrDefaultAsync();
                var result = new
                {
                    Data = data,
                    PageCount = page == null ? 1 : page.PageCount
                };
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving history");
            }
        }
    }
}
