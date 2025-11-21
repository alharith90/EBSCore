using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Notification
{
    public class DBS7SNotificationSP : DBProcessSP
    {
        private readonly IConfiguration configuration;

        private readonly TableField operation = new TableField("Operation", SqlDbType.NVarChar);
        private readonly TableField notificationStatusId = new TableField("NotificationStatusID", SqlDbType.Int);
        private readonly TableField notificationTemplateId = new TableField("NotificationTemplateID", SqlDbType.Int);
        private readonly TableField email = new TableField("Email", SqlDbType.NVarChar);
        private readonly TableField ccEmails = new TableField("CCEmails", SqlDbType.NVarChar);
        private readonly TableField bccEmails = new TableField("BCCEmails", SqlDbType.NVarChar);
        private readonly TableField countryCode = new TableField("CountryCode", SqlDbType.NVarChar);
        private readonly TableField mobileNo = new TableField("MobileNo", SqlDbType.NVarChar);
        private readonly TableField channelId = new TableField("ChannelID", SqlDbType.Int);
        private readonly TableField connectionId = new TableField("ConnectionID", SqlDbType.Int);
        private readonly TableField toUserId = new TableField("ToUserID", SqlDbType.Int);
        private readonly TableField sent = new TableField("Sent", SqlDbType.Bit);
        private readonly TableField success = new TableField("Success", SqlDbType.Bit);
        private readonly TableField tryAgain = new TableField("TryAgain", SqlDbType.Bit);
        private readonly TableField tryDate = new TableField("TryDate", SqlDbType.DateTime);
        private readonly TableField lastTryDate = new TableField("LastTryDate", SqlDbType.DateTime);
        private readonly TableField noOfTry = new TableField("NoOfTry", SqlDbType.Int);
        private readonly TableField maxTry = new TableField("MaxTry", SqlDbType.Int);
        private readonly TableField priority = new TableField("Priority", SqlDbType.Int);
        private readonly TableField scheduledAt = new TableField("ScheduledAt", SqlDbType.DateTime);
        private readonly TableField errorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        private readonly TableField errorStack = new TableField("ErrorStack", SqlDbType.NVarChar);
        private readonly TableField payloadJson = new TableField("PayloadJson", SqlDbType.NVarChar);
        private readonly TableField batchSize = new TableField("BatchSize", SqlDbType.Int);
        private readonly TableField currentUserId = new TableField("CurrentUserID", SqlDbType.Int);

        public DBS7SNotificationSP(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
            SPName = "S7SNotificationSP";
        }

        public DataTable RtvPendingNotifications(int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvPendingNotifications");
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvPendingNotifications");
                return new DataTable();
            }
        }

        public DataTable RtvPendingNotifications(int batchSizeValue, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvPendingNotifications");
                AddParameter(batchSize, batchSizeValue.ToString());
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvPendingNotifications");
                return new DataTable();
            }
        }

        public int MarkSent(int notificationStatusID, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "MarkSent");
                AddParameter(notificationStatusId, notificationStatusID.ToString());
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SNotificationSP.MarkSent {notificationStatusID}");
                return -1;
            }
        }

        public int MarkFailed(int notificationStatusID, string errorMessageValue, string errorStackValue, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "MarkFailed");
                AddParameter(notificationStatusId, notificationStatusID.ToString());
                AddParameter(errorMessage, errorMessageValue ?? string.Empty);
                AddParameter(errorStack, errorStackValue ?? string.Empty);
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SNotificationSP.MarkFailed {notificationStatusID}");
                return -1;
            }
        }

        public DataTable SaveTemplate(Models.Notification.S7SNotificationTemplate template, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "SaveTemplate");
                AddParameter(notificationTemplateId, template.NotificationTemplateID?.ToString() ?? string.Empty);
                AddParameter(new TableField("TemplateKey", SqlDbType.NVarChar), template.TemplateKey ?? string.Empty);
                AddParameter(new TableField("Name", SqlDbType.NVarChar), template.Name ?? string.Empty);
                AddParameter(channelId, template.ChannelID.ToString());
                AddParameter(new TableField("Subject", SqlDbType.NVarChar), template.Subject ?? string.Empty);
                AddParameter(new TableField("Body", SqlDbType.NVarChar), template.Body ?? string.Empty);
                AddParameter(new TableField("UseDesign", SqlDbType.Bit), template.UseDesign ? "1" : "0");
                AddParameter(new TableField("Attachments", SqlDbType.NVarChar), template.Attachments ?? string.Empty);
                AddParameter(new TableField("CompanyID", SqlDbType.Int), template.CompanyID?.ToString() ?? string.Empty);
                AddParameter(new TableField("IsActive", SqlDbType.Bit), template.IsActive ? "1" : "0");
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.SaveTemplate");
                return new DataTable();
            }
        }

        public DataTable RtvTemplate(int templateID, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvTemplate");
                AddParameter(notificationTemplateId, templateID.ToString());
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvTemplate");
                return new DataTable();
            }
        }

        public DataTable RtvTemplates(int? channelID, bool? isActive, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvTemplates");
                if (channelID.HasValue)
                {
                    AddParameter(channelId, channelID.Value.ToString());
                }

                if (isActive.HasValue)
                {
                    AddParameter(new TableField("IsActive", SqlDbType.Bit), isActive.Value ? "1" : "0");
                }

                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvTemplates");
                return new DataTable();
            }
        }

        public DataTable RtvChannels()
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvChannels");
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvChannels");
                return new DataTable();
            }
        }

        public DataTable RtvConnections(int? channelID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "rtvConnections");
                if (channelID.HasValue)
                {
                    AddParameter(channelId, channelID.Value.ToString());
                }
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.RtvConnections");
                return new DataTable();
            }
        }

        public DataTable SaveAll(Models.Notification.S7SNotificationStatus status, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "SaveAll");
                AddParameter(notificationStatusId, status.S7SNotificationStatusID?.ToString() ?? string.Empty);
                AddParameter(notificationTemplateId, status.NotificationTemplateID.ToString());
                AddParameter(email, status.Email ?? string.Empty);
                AddParameter(ccEmails, status.CCEmails ?? string.Empty);
                AddParameter(bccEmails, status.BCCEmails ?? string.Empty);
                AddParameter(countryCode, status.CountryCode ?? string.Empty);
                AddParameter(mobileNo, status.MobileNo ?? string.Empty);
                AddParameter(channelId, status.ChannelID.ToString());
                AddParameter(connectionId, status.ConnectionID?.ToString() ?? string.Empty);
                AddParameter(toUserId, status.ToUserID?.ToString() ?? string.Empty);
                AddParameter(sent, status.Sent ? "1" : "0");
                AddParameter(success, status.Success ? "1" : "0");
                AddParameter(priority, status.Priority.ToString());
                AddParameter(scheduledAt, status.ScheduledAt?.ToString("s") ?? string.Empty);
                AddParameter(payloadJson, status.PayloadJson ?? string.Empty);
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.SaveAll");
                return new DataTable();
            }
        }

        public int DeleteTemplate(int templateID, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "DeleteTemplate");
                AddParameter(notificationTemplateId, templateID.ToString());
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.DeleteTemplate");
                return -1;
            }
        }

        public DataTable SaveStatus(Models.Notification.S7SNotificationStatus status, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "SaveStatus");
                AddParameter(notificationStatusId, status.S7SNotificationStatusID?.ToString() ?? string.Empty);
                AddParameter(notificationTemplateId, status.NotificationTemplateID.ToString());
                AddParameter(email, status.Email ?? string.Empty);
                AddParameter(ccEmails, status.CCEmails ?? string.Empty);
                AddParameter(bccEmails, status.BCCEmails ?? string.Empty);
                AddParameter(countryCode, status.CountryCode ?? string.Empty);
                AddParameter(mobileNo, status.MobileNo ?? string.Empty);
                AddParameter(channelId, status.ChannelID.ToString());
                AddParameter(connectionId, status.ConnectionID?.ToString() ?? string.Empty);
                AddParameter(toUserId, status.ToUserID?.ToString() ?? string.Empty);
                AddParameter(sent, status.Sent ? "1" : "0");
                AddParameter(success, status.Success ? "1" : "0");
                AddParameter(tryAgain, status.TryAgain ? "1" : "0");
                AddParameter(tryDate, status.TryDate?.ToString("s") ?? string.Empty);
                AddParameter(lastTryDate, status.LastTryDate?.ToString("s") ?? string.Empty);
                AddParameter(noOfTry, status.NoOfTry.ToString());
                AddParameter(maxTry, status.MaxTry.ToString());
                AddParameter(priority, status.Priority.ToString());
                AddParameter(scheduledAt, status.ScheduledAt?.ToString("s") ?? string.Empty);
                AddParameter(errorMessage, status.ErrorMessage ?? string.Empty);
                AddParameter(errorStack, status.ErrorStack ?? string.Empty);
                AddParameter(payloadJson, status.PayloadJson ?? string.Empty);
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.SaveStatus");
                return new DataTable();
            }
        }

        public DataTable GetStatus(int notificationStatusID, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "GetStatus");
                AddParameter(notificationStatusId, notificationStatusID.ToString());
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.GetStatus");
                return new DataTable();
            }
        }

        public DataTable ListStatus(int? channelID, int? templateID, bool? sentValue, bool? successValue, int currentUserID)
        {
            try
            {
                ClearParameters();
                AddParameter(operation, "ListStatus");
                if (channelID.HasValue)
                {
                    AddParameter(channelId, channelID.Value.ToString());
                }
                if (templateID.HasValue)
                {
                    AddParameter(notificationTemplateId, templateID.Value.ToString());
                }
                if (sentValue.HasValue)
                {
                    AddParameter(sent, sentValue.Value ? "1" : "0");
                }
                if (successValue.HasValue)
                {
                    AddParameter(success, successValue.Value ? "1" : "0");
                }
                AddParameter(currentUserId, currentUserID.ToString());
                return ExecuteDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, "DBS7SNotificationSP.ListStatus");
                return new DataTable();
            }
        }

        private void ClearParameters()
        {
            FieldsArrayList = new ArrayList();
        }

        private void AddParameter(TableField field, string value)
        {
            field.SetValue(value ?? string.Empty, ref FieldsArrayList);
        }

        private DataTable ExecuteDataTable()
        {
            var result = QueryDatabase(SqlQueryType.FillDataset) as DataSet;
            return result != null && result.Tables.Count > 0 ? result.Tables[0] : new DataTable();
        }

        private int ExecuteNonQuery()
        {
            var result = QueryDatabase(SqlQueryType.ExecuteNonQuery);
            return result is int code ? code : -1;
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(configuration);
                handler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: ex.Message,
                    Form: context,
                    Source: ex.Source,
                    TargetSite: ex.TargetSite?.Name,
                    StackTrace: ex.StackTrace);
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
