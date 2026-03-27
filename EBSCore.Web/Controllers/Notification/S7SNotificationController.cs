using EBSCore.AdoClass;
using EBSCore.AdoClass.Notification;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using NotificationTemplate = EBSCore.Web.Models.S7SNotificationTemplate;
using NotificationStatus = EBSCore.Web.Models.S7SNotificationStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.Notification
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SNotificationCanceledController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DBS7SNotificationSP notificationSP;
        private readonly Common common;
        private readonly User currentUser;
          private readonly ILogger<S7SNotificationController> _logger;

        public S7SNotificationCanceledController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<S7SNotificationController> logger)
        {
            this.configuration = configuration;
            notificationSP = new DBS7SNotificationSP(configuration);
            currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            common = new Common();
             _logger = logger;
        }

        [HttpGet]
        public async Task<object> Index(string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchQuery = "")
        {
            try
            {
                DataSet ds = (DataSet)notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "GetStatus",
                    PageNumber: PageNumber ?? "1",
                    PageSize: PageSize ?? "10",
                    SortColumn: SortColumn ?? string.Empty,
                    SortDirection: SortDirection ?? string.Empty,
                    SearchQuery: SearchQuery ?? string.Empty,
                    CompanyID: currentUser?.CompanyID ?? string.Empty);
                var result = new
                {
                    Data = ds.Tables.Count > 0 ? ds.Tables[0] : null,
                    PageCount = ds.Tables.Count > 1 ? ds.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return BadRequest("Error retrieving notifications");
            }
        }

        [HttpGet]
        public async Task<object> Templates()
        {
            try
            {
                var ds = (DataSet)notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplates",
                    CompanyID: currentUser?.CompanyID ?? string.Empty);
                var table = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving templates");
                return BadRequest("Error retrieving templates");
            }
        }

        [HttpGet("{NotificationTemplateID}")]
        public async Task<object> Template(int NotificationTemplateID)
        {
            try
            {
                var ds = (DataSet)notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RtvTemplate",
                    NotificationTemplateID: NotificationTemplateID.ToString(),
                    CompanyID: currentUser?.CompanyID ?? string.Empty);
                return Ok(JsonConvert.SerializeObject(ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving template");
                return BadRequest("Error retrieving template");
            }
        }

        [HttpPost]
        public async Task<object> SaveTemplate(NotificationTemplate model)
        {
            try
            {
                var ds = (DataSet)notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveTemplate",
                    NotificationTemplateID: model.NotificationTemplateID?.ToString() ?? string.Empty,
                    TemplateKey: model.TemplateKey ?? string.Empty,
                    Name: model.Name ?? string.Empty,
                    ChannelID: model.ChannelID?.ToString() ?? string.Empty,
                    Subject: model.Subject ?? string.Empty,
                    Body: model.Body ?? string.Empty,
                    UseDesign: model.UseDesign.ToString(),
                    Attachments: model.Attachments ?? string.Empty,
                    Description: model.Description ?? string.Empty,
                    CompanyID: model.CompanyID?.ToString() ?? currentUser?.CompanyID ?? string.Empty,
                    IsActive: model.IsActive.ToString(),
                    CreatedBy: currentUser?.UserID?.ToString() ?? string.Empty);

                return Ok(JsonConvert.SerializeObject(ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving template");
                return BadRequest("Error saving template");
            }
        }

        [HttpDelete("{NotificationTemplateID}")]
        public async Task<object> DeleteTemplate(int NotificationTemplateID)
        {
            try
            {
                notificationSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteTemplate",
                    NotificationTemplateID: NotificationTemplateID.ToString(),
                    UpdatedBy: currentUser?.UserID?.ToString() ?? string.Empty);
                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template");
                return BadRequest("Error deleting template");
            }
        }

        [HttpPost]
        public async Task<object> SaveStatus(NotificationStatus status)
        {
            try
            {
                var ds = (DataSet)notificationSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "SaveStatus",
                    NotificationTemplateID: status.NotificationTemplateID?.ToString() ?? string.Empty,
                    Email: status.Email ?? string.Empty,
                    CCEmails: status.CCEmails ?? string.Empty,
                    BCCEmails: status.BCCEmails ?? string.Empty,
                    CountryCode: status.CountryCode?.ToString() ?? string.Empty,
                    MobileNo: status.MobileNo ?? string.Empty,
                    ToUserID: status.ToUserID?.ToString() ?? string.Empty,
                    ChannelID: status.ChannelID?.ToString() ?? string.Empty,
                    ConnectionID: status.ConnectionID?.ToString() ?? string.Empty,
                    Priority: status.Priority?.ToString() ?? string.Empty,
                    ScheduledAt: status.ScheduledAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                    ExceptionID: status.ExceptionID?.ToString() ?? string.Empty,
                    ErrorMessage: status.ErrorMessage ?? string.Empty,
                    ErrorStack: status.ErrorStack ?? string.Empty,
                    CreatedBy: currentUser?.UserID ?? string.Empty);

                return Ok(JsonConvert.SerializeObject(ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving notification");
                return BadRequest("Error saving notification");
            }
        }

        [HttpGet]
        public async Task<object> Channels()
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();
                const string sql = "SELECT NotificationChannelID, Name, IsActive FROM S7SNotificationChannel WHERE IsActive = 1 ORDER BY Name";
                var channels = new List<S7SNotificationChannel>();
                using var command = new SqlCommand(sql, conn);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    channels.Add(new S7SNotificationChannel
                    {
                        NotificationChannelID = reader["NotificationChannelID"] == DBNull.Value ? null : Convert.ToInt32(reader["NotificationChannelID"]),
                        Name = reader["Name"] == DBNull.Value ? null : reader["Name"].ToString(),
                        IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                    });
                }
                return Ok(JsonConvert.SerializeObject(channels));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving channels");
                return BadRequest("Error retrieving channels");
            }
        }

        [HttpGet]
        public async Task<object> Connections()
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();
                const string sql = @"SELECT NotificationConnectionID, ChannelID, Name, ProviderType, ConfigurationJson,
                                            IsDefault, IsActive, CompanyID, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                                     FROM S7SNotificationConnection
                                     WHERE IsActive = 1
                                     ORDER BY Name";
                var connections = new List<S7SNotificationConnection>();
                using var command = new SqlCommand(sql, conn);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    connections.Add(new S7SNotificationConnection
                    {
                        NotificationConnectionID = reader["NotificationConnectionID"] == DBNull.Value ? null : Convert.ToInt32(reader["NotificationConnectionID"]),
                        ChannelID = reader["ChannelID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ChannelID"]),
                        Name = reader["Name"] == DBNull.Value ? null : reader["Name"].ToString(),
                        ProviderType = reader["ProviderType"] == DBNull.Value ? null : reader["ProviderType"].ToString(),
                        ConfigurationJson = reader["ConfigurationJson"] == DBNull.Value ? null : reader["ConfigurationJson"].ToString(),
                        IsDefault = reader["IsDefault"] != DBNull.Value && Convert.ToBoolean(reader["IsDefault"]),
                        IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                        CompanyID = reader["CompanyID"] == DBNull.Value ? null : Convert.ToInt32(reader["CompanyID"]),
                        CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]),
                        CreatedAt = reader["CreatedAt"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"]),
                        UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
                return Ok(JsonConvert.SerializeObject(connections));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving connections");
                return BadRequest("Error retrieving connections");
            }
        }

        [HttpPost]
        public async Task<object> SaveConnection(S7SNotificationConnection connection)
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();
                if (connection.NotificationConnectionID == null)
                {
                    var insertSql = @"INSERT INTO S7SNotificationConnection (ChannelID, Name, ProviderType, ConfigurationJson, IsDefault, IsActive, CompanyID, CreatedBy)
                                      VALUES (@ChannelID, @Name, @ProviderType, @ConfigurationJson, ISNULL(@IsDefault,0), ISNULL(@IsActive,1), @CompanyID, @CreatedBy);
                                      SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    using var insertCommand = new SqlCommand(insertSql, conn);
                    insertCommand.Parameters.AddWithValue("@ChannelID", (object?)connection.ChannelID ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@Name", (object?)connection.Name ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@ProviderType", (object?)connection.ProviderType ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@ConfigurationJson", (object?)connection.ConfigurationJson ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@IsDefault", (object?)connection.IsDefault ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@IsActive", (object?)connection.IsActive ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@CompanyID", (object?)connection.CompanyID ?? DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@CreatedBy", (object?)currentUser?.UserID ?? DBNull.Value);
                    var id = (int)(await insertCommand.ExecuteScalarAsync() ?? 0);
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
                    using var updateCommand = new SqlCommand(updateSql, conn);
                    updateCommand.Parameters.AddWithValue("@ChannelID", (object?)connection.ChannelID ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@Name", (object?)connection.Name ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@ProviderType", (object?)connection.ProviderType ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@ConfigurationJson", (object?)connection.ConfigurationJson ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@IsDefault", (object?)connection.IsDefault ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@IsActive", (object?)connection.IsActive ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@CompanyID", (object?)connection.CompanyID ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@NotificationConnectionID", connection.NotificationConnectionID);
                    updateCommand.Parameters.AddWithValue("@UpdatedBy", (object?)currentUser?.UserID ?? DBNull.Value);
                    await updateCommand.ExecuteNonQueryAsync();
                }

                return Ok(JsonConvert.SerializeObject(connection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving connection");
                return BadRequest("Error saving connection");
            }
        }

        [HttpDelete("{NotificationConnectionID}")]
        public async Task<object> DeleteConnection(int NotificationConnectionID)
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();
                const string sql = "UPDATE S7SNotificationConnection SET IsActive = 0, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UpdatedBy WHERE NotificationConnectionID = @NotificationConnectionID";
                using var command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)currentUser?.UserID ?? DBNull.Value);
                command.Parameters.AddWithValue("@NotificationConnectionID", NotificationConnectionID);
                await command.ExecuteNonQueryAsync();
                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting connection");
                return BadRequest("Error deleting connection");
            }
        }

        [HttpGet]
        public async Task<object> History(int? NotificationStatusID = null, string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchQuery = "")
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();
                using var command = new SqlCommand("S7SNotificationHistorySP", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Operation", "List");
                command.Parameters.AddWithValue("@NotificationStatusID", (object?)NotificationStatusID ?? DBNull.Value);
                command.Parameters.AddWithValue("@PageNumber", (object?)PageNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@PageSize", (object?)PageSize ?? DBNull.Value);
                command.Parameters.AddWithValue("@SortColumn", (object?)SortColumn ?? DBNull.Value);
                command.Parameters.AddWithValue("@SortDirection", (object?)SortDirection ?? DBNull.Value);
                command.Parameters.AddWithValue("@SearchQuery", (object?)SearchQuery ?? DBNull.Value);

                using var adapter = new SqlDataAdapter(command);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                var data = dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
                var page = dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0 ? dataSet.Tables[1].Rows[0] : null;
                var result = new
                {
                    Data = data,
                    PageCount = page == null ? 1 : page["PageCount"]
                };
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history");
                return BadRequest("Error retrieving history");
            }
        }
    }
}
