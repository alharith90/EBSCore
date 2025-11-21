using EBSCore.AdoClass;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EBSCore.Web.Services
{
    public class S7SNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<S7SNotificationBackgroundService> logger;

        public S7SNotificationBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<S7SNotificationBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var sp = new DBS7SNotificationSP(configuration);
                    var table = sp.RtvPendingNotifications("50");

                    foreach (DataRow row in table.Rows)
                    {
                        if (stoppingToken.IsCancellationRequested) break;
                        var statusId = row.Field<int>("NotificationStatusID");
                        var channelCode = row.Field<string>("ChannelCode") ?? string.Empty;
                        try
                        {
                            await SendAsync(row, channelCode, stoppingToken);
                            sp.MarkSent(statusId.ToString(), string.Empty, row.Table.Columns.Contains("ExceptionID") ? row["ExceptionID"].ToString() : string.Empty);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to send notification {NotificationStatusID}", statusId);
                            sp.MarkFailed(statusId.ToString(), string.Empty, ex.Message, ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "S7S notification background service error");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task SendAsync(DataRow row, string channelCode, CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient();
            switch (channelCode.ToLowerInvariant())
            {
                case "email":
                case "exchange":
                    await SendEmailAsync(row, client, cancellationToken);
                    break;
                case "sms":
                    await SendSmsAsync(row, client, cancellationToken);
                    break;
                case "whatsapp":
                    await SendWhatsAppAsync(row, client, cancellationToken);
                    break;
                case "webhook":
                    await SendWebHookAsync(row, client, cancellationToken);
                    break;
                default:
                    logger.LogWarning("Unknown channel {ChannelCode}, marking as sent", channelCode);
                    break;
            }
        }

        private Task SendEmailAsync(DataRow row, HttpClient client, CancellationToken cancellationToken)
        {
            logger.LogInformation("[S7SNotification] Sending email to {Email}", row.Field<string>("Email"));
            return Task.CompletedTask;
        }

        private Task SendSmsAsync(DataRow row, HttpClient client, CancellationToken cancellationToken)
        {
            logger.LogInformation("[S7SNotification] Sending sms to {Mobile}", row.Field<string>("MobileNo"));
            return Task.CompletedTask;
        }

        private Task SendWhatsAppAsync(DataRow row, HttpClient client, CancellationToken cancellationToken)
        {
            logger.LogInformation("[S7SNotification] Sending WhatsApp to {Mobile}", row.Field<string>("MobileNo"));
            return Task.CompletedTask;
        }

        private async Task SendWebHookAsync(DataRow row, HttpClient client, CancellationToken cancellationToken)
        {
            var payload = new
            {
                NotificationStatusID = row.Field<int>("NotificationStatusID"),
                Template = row.Field<string>("TemplateKey"),
                Body = row.Field<string>("Body"),
                Email = row.Field<string>("Email"),
                Mobile = row.Field<string>("MobileNo")
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
            }
        }
    }
}
