using EBSCore.AdoClass;
<<<<<<< ours
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Net.Http;
=======
using EBSCore.Web.AppCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Net.Http;
using System.Net.Mail;
>>>>>>> theirs
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EBSCore.Web.Services
{
    public class S7SNotificationBackgroundService : BackgroundService
    {
<<<<<<< ours
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<S7SNotificationBackgroundService> logger;

        public S7SNotificationBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<S7SNotificationBackgroundService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
=======
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<S7SNotificationBackgroundService> _logger;
        private readonly Common _common = new Common();

        public S7SNotificationBackgroundService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, ILogger<S7SNotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
>>>>>>> theirs
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
<<<<<<< ours
=======
            _common.LogInfo("S7SNotificationBackgroundService starting", "S7SNotificationBackgroundService");
>>>>>>> theirs
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
<<<<<<< ours
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
=======
                    using var scope = _scopeFactory.CreateScope();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var sp = new DBS7SNotificationSP(configuration);

                    var pending = (DataSet)sp.RtvPendingNotifications(25, 0);
                    if (pending.Tables.Count == 0 || pending.Tables[0].Rows.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        continue;
                    }

                    foreach (DataRow row in pending.Tables[0].Rows)
                    {
                        var item = PendingNotification.FromDataRow(row);
                        try
                        {
                            var response = await SendAsync(item, stoppingToken);
                            sp.MarkSent(item.NotificationStatusID, 0);
                            _logger.LogInformation("Notification {Id} sent via {Channel}", item.NotificationStatusID, item.ChannelCode);
                            _common.LogInfo($"Notification sent {item.NotificationStatusID}", response ?? string.Empty);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Notification {Id} failed", item.NotificationStatusID);
                            _common.LogError(ex, $"Notification failed {item.NotificationStatusID}");
                            sp.MarkFailed(item.NotificationStatusID, ex.Message, ex.StackTrace ?? string.Empty, 0);
>>>>>>> theirs
                        }
                    }
                }
                catch (Exception ex)
                {
<<<<<<< ours
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
=======
                    _logger.LogError(ex, "S7SNotificationBackgroundService loop error");
                    _common.LogError(ex, "S7SNotificationBackgroundService loop error");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task<string?> SendAsync(PendingNotification item, CancellationToken cancellationToken)
        {
            switch (item.ChannelCode.ToLowerInvariant())
            {
                case "email":
                case "exchange":
                    return await SendEmailAsync(item);
                case "sms":
                case "whatsapp":
                    _logger.LogInformation("Simulated {Channel} send to {Target}", item.ChannelCode, item.TargetAddress);
                    return JsonSerializer.Serialize(new { Channel = item.ChannelCode, Target = item.TargetAddress, Status = "Queued" });
                case "webhook":
                    return await SendWebhookAsync(item, cancellationToken);
                default:
                    _logger.LogWarning("Unknown notification channel {Channel}", item.ChannelCode);
                    return JsonSerializer.Serialize(new { Channel = item.ChannelCode, Status = "Unknown" });
            }
        }

        private async Task<string?> SendEmailAsync(PendingNotification item)
        {
            try
            {
                var config = item.ConfigurationJson ?? string.Empty;
                var smtpHost = "";
                int smtpPort = 25;
                string? user = null;
                string? password = null;

                if (!string.IsNullOrWhiteSpace(config))
                {
                    using var json = JsonDocument.Parse(config);
                    var root = json.RootElement;
                    smtpHost = root.TryGetProperty("Host", out var host) ? host.GetString() ?? string.Empty : smtpHost;
                    smtpPort = root.TryGetProperty("Port", out var port) ? port.GetInt32() : smtpPort;
                    user = root.TryGetProperty("User", out var usr) ? usr.GetString() : null;
                    password = root.TryGetProperty("Password", out var pwd) ? pwd.GetString() : null;
                }

                using var smtp = new SmtpClient(string.IsNullOrWhiteSpace(smtpHost) ? "localhost" : smtpHost, smtpPort);
                if (!string.IsNullOrEmpty(user))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(user, password);
                }

                var mail = new MailMessage
                {
                    Subject = item.Subject ?? "Notification",
                    Body = item.Body ?? string.Empty,
                    IsBodyHtml = true
                };
                mail.To.Add(item.Email ?? item.TargetAddress ?? string.Empty);

                if (!string.IsNullOrWhiteSpace(item.CC))
                {
                    mail.CC.Add(item.CC);
                }
                if (!string.IsNullOrWhiteSpace(item.BCC))
                {
                    mail.Bcc.Add(item.BCC);
                }

                await smtp.SendMailAsync(mail);
                return JsonSerializer.Serialize(new { Status = "Sent", Host = smtpHost, Port = smtpPort });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email send failed for {Id}", item.NotificationStatusID);
                throw;
            }
        }

        private async Task<string?> SendWebhookAsync(PendingNotification item, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            var url = item.TargetAddress;
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("Webhook target is missing");
            }

            var payload = item.PayloadJson ?? "{}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Serialize(new { StatusCode = (int)response.StatusCode, Reason = response.ReasonPhrase });
        }

        private class PendingNotification
        {
            public int NotificationStatusID { get; set; }
            public string ChannelCode { get; set; } = string.Empty;
            public string? ProviderType { get; set; }
            public string? ConfigurationJson { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
            public string? Email { get; set; }
            public string? CC { get; set; }
            public string? BCC { get; set; }
            public string? TargetAddress { get; set; }
            public string? PayloadJson { get; set; }

            public static PendingNotification FromDataRow(DataRow row)
            {
                return new PendingNotification
                {
                    NotificationStatusID = Convert.ToInt32(row[nameof(NotificationStatusID)]),
                    ChannelCode = row.Table.Columns.Contains("ChannelCode") ? row["ChannelCode"].ToString() ?? string.Empty : string.Empty,
                    ProviderType = row.Table.Columns.Contains("ProviderType") ? row["ProviderType"].ToString() : null,
                    ConfigurationJson = row.Table.Columns.Contains("ConfigurationJson") ? row["ConfigurationJson"].ToString() : null,
                    Subject = row.Table.Columns.Contains("Subject") ? row["Subject"].ToString() : null,
                    Body = row.Table.Columns.Contains("Body") ? row["Body"].ToString() : null,
                    Email = row.Table.Columns.Contains("Email") ? row["Email"].ToString() : null,
                    CC = row.Table.Columns.Contains("CCEmails") ? row["CCEmails"].ToString() : null,
                    BCC = row.Table.Columns.Contains("BCCEmails") ? row["BCCEmails"].ToString() : null,
                    TargetAddress = row.Table.Columns.Contains("MobileNo") ? row["MobileNo"].ToString() : null,
                    PayloadJson = row.Table.Columns.Contains("PayloadJson") ? row["PayloadJson"].ToString() : null
                };
>>>>>>> theirs
            }
        }
    }
}
