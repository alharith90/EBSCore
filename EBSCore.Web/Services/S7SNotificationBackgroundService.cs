using EBSCore.AdoClass;
using EBSCore.AdoClass.Notification;
using EBSCore.Web.AppCode;
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
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Services
{
    public class S7SNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<S7SNotificationBackgroundService> logger;
        private readonly Common common = new Common();

        public S7SNotificationBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<S7SNotificationBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            common.LogInfo("S7SNotificationBackgroundService starting", "S7SNotificationBackgroundService");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var sp = new DBS7SNotificationSP(configuration);

                    var pendingDs = (DataSet)sp.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                        Operation: "RtvPendingNotifications",
                        BatchSize: "25");
                    var pending = pendingDs?.Tables.Count > 0 ? pendingDs.Tables[0] : new DataTable();
                    if (pending.Rows.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                        continue;
                    }

                    foreach (DataRow row in pending.Rows)
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }

                        var statusId = Convert.ToInt32(row["NotificationStatusID"]);
                        try
                        {
                            var response = await SendAsync(row, stoppingToken);
                            sp.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                                Operation: "MarkSent",
                                NotificationStatusID: statusId.ToString());
                            logger.LogInformation("Notification {Id} sent", statusId);
                            common.LogInfo($"Notification sent {statusId}", response ?? string.Empty);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Notification {Id} failed", statusId);
                            common.LogError(ex, $"Notification failed {statusId}");
                            sp.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                                Operation: "MarkFailed",
                                NotificationStatusID: statusId.ToString(),
                                ErrorMessage: ex.Message,
                                ErrorStack: ex.StackTrace ?? string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "S7SNotificationBackgroundService loop error");
                    common.LogError(ex, "S7SNotificationBackgroundService loop error");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task<string?> SendAsync(DataRow row, CancellationToken cancellationToken)
        {
            var channelCode = row.Table.Columns.Contains("ChannelCode") ? row["ChannelCode"].ToString() ?? string.Empty : string.Empty;
            switch (channelCode.ToLowerInvariant())
            {
                case "webhook":
                    return await SendWebHookAsync(row, cancellationToken);
                default:
                    logger.LogInformation("Simulated send for channel {Channel}", channelCode);
                    return JsonSerializer.Serialize(new { Channel = channelCode, Status = "Queued" });
            }
        }

        private async Task<string?> SendWebHookAsync(DataRow row, CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient();
            var payload = new
            {
                NotificationStatusID = row.Field<int>("NotificationStatusID"),
                Template = row.Table.Columns.Contains("TemplateKey") ? row.Field<string>("TemplateKey") : string.Empty,
                Body = row.Table.Columns.Contains("Body") ? row.Field<string>("Body") : string.Empty,
                Email = row.Table.Columns.Contains("Email") ? row.Field<string>("Email") : string.Empty,
                Mobile = row.Table.Columns.Contains("MobileNo") ? row.Field<string>("MobileNo") : string.Empty
            };

            var json = JsonSerializer.Serialize(payload);
            string? url = null;
            if (row.Table.Columns.Contains("ConfigurationJson"))
            {
                var configJson = row.Field<string>("ConfigurationJson");
                if (!string.IsNullOrWhiteSpace(configJson))
                {
                    using var doc = JsonDocument.Parse(configJson);
                    if (doc.RootElement.TryGetProperty("Url", out var urlElement))
                    {
                        url = urlElement.GetString();
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            logger.LogWarning("Webhook URL missing for notification {Id}", row.Field<int>("NotificationStatusID"));
            return null;
        }
    }
}
