using EBSCore.AdoClass;
using EBSCore.AdoClass.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EBSCore.Web.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly EBSCore.Web.AppCode.Common _common = new EBSCore.Web.AppCode.Common();

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _common.LogInfo("NotificationBackgroundService starting", "Background worker starting");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _common.LogInfo("NotificationBackgroundService tick", "Polling notifications");
                    using var scope = _scopeFactory.CreateScope();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var dispatcher = new DBNotificationDispatchSP(configuration);

                    var dataSet = (DataSet)dispatcher.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset, Operation: "DequeueOutbox");
                    if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    {
                        _common.LogInfo("No pending notifications", "Dispatch queue empty");
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                        continue;
                    }

                    var row = dataSet.Tables[0].Rows[0];
                    var item = NotificationDispatchItem.FromDataRow(row);
                    _common.LogInfo("Dispatching notification", $"Outbox:{item.OutboxID} Channel:{item.ChannelID} Template:{item.TemplateID} Notification:{item.NotificationID}");
                    _logger.LogInformation("[Notification] Dispatching outbox item {OutboxID} via {ChannelType} to {RecipientID}", item.OutboxID, item.ChannelType, item.RecipientID);

                    try
                    {
                        var response = await SendAsync(item, stoppingToken);
                        dispatcher.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                            Operation: "CompleteOutbox",
                            OutboxID: item.OutboxID.ToString(),
                            Status: "Succeeded",
                            ResponseJson: response ?? string.Empty);
                        _common.LogInfo("Notification sent", $"Outbox:{item.OutboxID} Channel:{item.ChannelID} Response:{response}");
                        _logger.LogInformation("[Notification] Outbox {OutboxID} delivered", item.OutboxID);
                    }
                    catch (Exception ex)
                    {
                        _common.LogError(ex, $"Notification send failure Outbox:{item.OutboxID} Channel:{item.ChannelID} Template:{item.TemplateID}");
                        _logger.LogError(ex, "[Notification] Failed sending Outbox {OutboxID}", item.OutboxID);
                        dispatcher.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                            Operation: "FailOutbox",
                            OutboxID: item.OutboxID.ToString(),
                            ErrorMessage: ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    _common.LogError(ex, "Notification background service error");
                    _logger.LogError(ex, "Notification background service error");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            _common.LogInfo("NotificationBackgroundService stopping", "Background worker stopping");
        }

        private async Task<string?> SendAsync(NotificationDispatchItem item, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            switch (item.ChannelType.ToLowerInvariant())
            {
                case "email":
                    _logger.LogInformation("[Notification] Email to {Target}: {Subject}", item.TargetAddress, item.Subject);
                    break;
                case "sms":
                    _logger.LogInformation("[Notification] SMS to {Target}", item.TargetAddress);
                    break;
                case "whatsapp":
                    _logger.LogInformation("[Notification] WhatsApp message to {Target}", item.TargetAddress);
                    break;
                case "webhook":
                    if (!string.IsNullOrWhiteSpace(item.TargetAddress))
                    {
                        var content = new StringContent(item.PayloadJson ?? "{}", Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(item.TargetAddress, content, cancellationToken);
                        response.EnsureSuccessStatusCode();
                        return JsonSerializer.Serialize(new
                        {
                            StatusCode = (int)response.StatusCode,
                            Reason = response.ReasonPhrase
                        });
                    }
                    break;
                default:
                    _logger.LogWarning("[Notification] Unknown channel type {ChannelType}. Marking as sent to avoid blocking.", item.ChannelType);
                    break;
            }

            return JsonSerializer.Serialize(new
            {
                ProcessedAt = DateTimeOffset.UtcNow,
                item.ChannelType,
                item.TargetAddress
            });
        }

        private class NotificationDispatchItem
        {
            public long OutboxID { get; set; }
            public long NotificationID { get; set; }
            public long RecipientID { get; set; }
            public int? ChannelID { get; set; }
            public string ChannelType { get; set; } = "";
            public int? TemplateID { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public string? PayloadJson { get; set; }
            public int RetryCount { get; set; }
            public int MaxRetryCount { get; set; }
            public int BackoffSeconds { get; set; }
            public string TargetAddress { get; set; } = string.Empty;

            public static NotificationDispatchItem FromDataRow(DataRow row)
            {
                return new NotificationDispatchItem
                {
                    OutboxID = Convert.ToInt64(row[nameof(OutboxID)]),
                    NotificationID = Convert.ToInt64(row[nameof(NotificationID)]),
                    RecipientID = Convert.ToInt64(row[nameof(RecipientID)]),
                    ChannelID = row.Table.Columns.Contains(nameof(ChannelID)) && row[nameof(ChannelID)] != DBNull.Value ? Convert.ToInt32(row[nameof(ChannelID)]) : null,
                    ChannelType = row[nameof(ChannelType)].ToString() ?? string.Empty,
                    TemplateID = row.Table.Columns.Contains(nameof(TemplateID)) && row[nameof(TemplateID)] != DBNull.Value ? Convert.ToInt32(row[nameof(TemplateID)]) : null,
                    Subject = row.Table.Columns.Contains(nameof(Subject)) ? row[nameof(Subject)].ToString() : null,
                    Body = row.Table.Columns.Contains(nameof(Body)) ? row[nameof(Body)].ToString() : null,
                    PayloadJson = row.Table.Columns.Contains(nameof(PayloadJson)) ? row[nameof(PayloadJson)].ToString() : null,
                    RetryCount = row.Table.Columns.Contains(nameof(RetryCount)) ? Convert.ToInt32(row[nameof(RetryCount)]) : 0,
                    MaxRetryCount = row.Table.Columns.Contains(nameof(MaxRetryCount)) ? Convert.ToInt32(row[nameof(MaxRetryCount)]) : 0,
                    BackoffSeconds = row.Table.Columns.Contains(nameof(BackoffSeconds)) ? Convert.ToInt32(row[nameof(BackoffSeconds)]) : 30,
                    TargetAddress = row.Table.Columns.Contains("TargetAddress") ? row["TargetAddress"].ToString() ?? string.Empty : string.Empty
                };
            }
        }
    }
}
