using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Data;
<<<<<<< ours
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
=======
>>>>>>> theirs

namespace EBSCore.AdoClass
{
    public class DBS7SNotificationSP : DBParentStoredProcedureClass
    {
<<<<<<< ours
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField NotificationStatusID = new TableField("NotificationStatusID", SqlDbType.Int);
        public TableField NotificationTemplateID = new TableField("NotificationTemplateID", SqlDbType.Int);
        public TableField TemplateKey = new TableField("TemplateKey", SqlDbType.NVarChar);
        public TableField Name = new TableField("Name", SqlDbType.NVarChar);
        public TableField ChannelID = new TableField("ChannelID", SqlDbType.Int);
        public TableField ConnectionID = new TableField("ConnectionID", SqlDbType.Int);
        public TableField Subject = new TableField("Subject", SqlDbType.NVarChar);
        public TableField Body = new TableField("Body", SqlDbType.NVarChar, true);
        public TableField UseDesign = new TableField("UseDesign", SqlDbType.Bit);
        public TableField Attachments = new TableField("Attachments", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField IsActive = new TableField("IsActive", SqlDbType.Bit);
        public TableField Email = new TableField("Email", SqlDbType.NVarChar);
        public TableField CCEmails = new TableField("CCEmails", SqlDbType.NVarChar);
        public TableField BCCEmails = new TableField("BCCEmails", SqlDbType.NVarChar);
        public TableField CountryCode = new TableField("CountryCode", SqlDbType.Int);
        public TableField MobileNo = new TableField("MobileNo", SqlDbType.NVarChar);
        public TableField ToUserID = new TableField("ToUserID", SqlDbType.Int);
        public TableField TryDate = new TableField("TryDate", SqlDbType.DateTime2);
        public TableField MaxTry = new TableField("MaxTry", SqlDbType.Int);
        public TableField Priority = new TableField("Priority", SqlDbType.Int);
        public TableField ScheduledAt = new TableField("ScheduledAt", SqlDbType.DateTime2);
        public TableField ExceptionID = new TableField("ExceptionID", SqlDbType.Int);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField ErrorStack = new TableField("ErrorStack", SqlDbType.NVarChar, true);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public TableField BatchSize = new TableField("BatchSize", SqlDbType.Int);

        public DBS7SNotificationSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SNotificationSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string NotificationStatusID = "",
            string NotificationTemplateID = "",
            string TemplateKey = "",
            string Name = "",
            string ChannelID = "",
            string ConnectionID = "",
            string Subject = "",
            string Body = "",
            string UseDesign = "",
            string Attachments = "",
            string Description = "",
            string CompanyID = "",
            string IsActive = "",
            string Email = "",
            string CCEmails = "",
            string BCCEmails = "",
            string CountryCode = "",
            string MobileNo = "",
            string ToUserID = "",
            string TryDate = "",
            string MaxTry = "",
            string Priority = "",
            string ScheduledAt = "",
            string ExceptionID = "",
            string ErrorMessage = "",
            string ErrorStack = "",
            string UpdatedBy = "",
            string CreatedBy = "",
            string PageNumber = "",
            string PageSize = "",
            string SortColumn = "",
            string SortDirection = "",
            string SearchQuery = "",
            string BatchSize = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.NotificationStatusID.SetValue(NotificationStatusID, ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(NotificationTemplateID, ref FieldsArrayList);
            this.TemplateKey.SetValue(TemplateKey, ref FieldsArrayList);
            this.Name.SetValue(Name, ref FieldsArrayList);
            this.ChannelID.SetValue(ChannelID, ref FieldsArrayList);
            this.ConnectionID.SetValue(ConnectionID, ref FieldsArrayList);
            this.Subject.SetValue(Subject, ref FieldsArrayList);
            this.Body.SetValue(Body, ref FieldsArrayList);
            this.UseDesign.SetValue(UseDesign, ref FieldsArrayList);
            this.Attachments.SetValue(Attachments, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.IsActive.SetValue(IsActive, ref FieldsArrayList);
            this.Email.SetValue(Email, ref FieldsArrayList);
            this.CCEmails.SetValue(CCEmails, ref FieldsArrayList);
            this.BCCEmails.SetValue(BCCEmails, ref FieldsArrayList);
            this.CountryCode.SetValue(CountryCode, ref FieldsArrayList);
            this.MobileNo.SetValue(MobileNo, ref FieldsArrayList);
            this.ToUserID.SetValue(ToUserID, ref FieldsArrayList);
            this.TryDate.SetValue(TryDate, ref FieldsArrayList);
            this.MaxTry.SetValue(MaxTry, ref FieldsArrayList);
            this.Priority.SetValue(Priority, ref FieldsArrayList);
            this.ScheduledAt.SetValue(ScheduledAt, ref FieldsArrayList);
            this.ExceptionID.SetValue(ExceptionID, ref FieldsArrayList);
            this.ErrorMessage.SetValue(ErrorMessage, ref FieldsArrayList);
            this.ErrorStack.SetValue(ErrorStack, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);
            this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
            this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);
            this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);
            this.BatchSize.SetValue(BatchSize, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }

        public DataTable RtvPendingNotifications(string batchSize = "10")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("rtvPendingNotifications", ref FieldsArrayList);
            this.BatchSize.SetValue(batchSize, ref FieldsArrayList);
            DataSet ds = (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public DataRow? SaveTemplate(
            string notificationTemplateID,
            string templateKey,
            string name,
            string channelID,
            string subject,
            string body,
            string useDesign,
            string attachments,
            string description,
            string companyID,
            string isActive,
            string userID)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("SaveTemplate", ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(notificationTemplateID, ref FieldsArrayList);
            this.TemplateKey.SetValue(templateKey, ref FieldsArrayList);
            this.Name.SetValue(name, ref FieldsArrayList);
            this.ChannelID.SetValue(channelID, ref FieldsArrayList);
            this.Subject.SetValue(subject, ref FieldsArrayList);
            this.Body.SetValue(body, ref FieldsArrayList);
            this.UseDesign.SetValue(useDesign, ref FieldsArrayList);
            this.Attachments.SetValue(attachments, ref FieldsArrayList);
            this.Description.SetValue(description, ref FieldsArrayList);
            this.CompanyID.SetValue(companyID, ref FieldsArrayList);
            this.IsActive.SetValue(isActive, ref FieldsArrayList);
            this.CreatedBy.SetValue(userID, ref FieldsArrayList);
            DataSet ds = (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
            return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
        }

        public DataRow? RtvTemplate(string notificationTemplateID)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("RtvTemplate", ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(notificationTemplateID, ref FieldsArrayList);
            DataSet ds = (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
            return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
        }

        public DataTable RtvTemplates(string companyID = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("RtvTemplates", ref FieldsArrayList);
            this.CompanyID.SetValue(companyID, ref FieldsArrayList);
            DataSet ds = (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public int DeleteTemplate(string notificationTemplateID, string updatedBy)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("DeleteTemplate", ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(notificationTemplateID, ref FieldsArrayList);
            this.UpdatedBy.SetValue(updatedBy, ref FieldsArrayList);
            return (int)base.QueryDatabase(SqlQueryType.ExecuteNonQuery);
        }

        public DataRow? SaveStatus(
            string notificationTemplateID,
            string email,
            string ccEmails,
            string bccEmails,
            string countryCode,
            string mobileNo,
            string toUserID,
            string channelID,
            string connectionID,
            string priority,
            string scheduledAt,
            string exceptionID,
            string errorMessage,
            string errorStack)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("SaveStatus", ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(notificationTemplateID, ref FieldsArrayList);
            this.Email.SetValue(email, ref FieldsArrayList);
            this.CCEmails.SetValue(ccEmails, ref FieldsArrayList);
            this.BCCEmails.SetValue(bccEmails, ref FieldsArrayList);
            this.CountryCode.SetValue(countryCode, ref FieldsArrayList);
            this.MobileNo.SetValue(mobileNo, ref FieldsArrayList);
            this.ToUserID.SetValue(toUserID, ref FieldsArrayList);
            this.ChannelID.SetValue(channelID, ref FieldsArrayList);
            this.ConnectionID.SetValue(connectionID, ref FieldsArrayList);
            this.Priority.SetValue(priority, ref FieldsArrayList);
            this.ScheduledAt.SetValue(scheduledAt, ref FieldsArrayList);
            this.ExceptionID.SetValue(exceptionID, ref FieldsArrayList);
            this.ErrorMessage.SetValue(errorMessage, ref FieldsArrayList);
            this.ErrorStack.SetValue(errorStack, ref FieldsArrayList);
            DataSet ds = (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
            return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
        }

        public int MarkSent(string notificationStatusID, string updatedBy, string exceptionID = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("MarkSent", ref FieldsArrayList);
            this.NotificationStatusID.SetValue(notificationStatusID, ref FieldsArrayList);
            this.ExceptionID.SetValue(exceptionID, ref FieldsArrayList);
            this.UpdatedBy.SetValue(updatedBy, ref FieldsArrayList);
            return (int)base.QueryDatabase(SqlQueryType.ExecuteNonQuery);
        }

        public int MarkFailed(string notificationStatusID, string updatedBy, string errorMessage, string errorStack, string exceptionID = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("MarkFailed", ref FieldsArrayList);
            this.NotificationStatusID.SetValue(notificationStatusID, ref FieldsArrayList);
            this.ErrorMessage.SetValue(errorMessage, ref FieldsArrayList);
            this.ErrorStack.SetValue(errorStack, ref FieldsArrayList);
            this.ExceptionID.SetValue(exceptionID, ref FieldsArrayList);
            this.UpdatedBy.SetValue(updatedBy, ref FieldsArrayList);
            return (int)base.QueryDatabase(SqlQueryType.ExecuteNonQuery);
        }

        public DataSet GetStatus(string pageNumber, string pageSize, string sortColumn, string sortDirection, string searchQuery)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("GetStatus", ref FieldsArrayList);
            this.PageNumber.SetValue(pageNumber, ref FieldsArrayList);
            this.PageSize.SetValue(pageSize, ref FieldsArrayList);
            this.SortColumn.SetValue(sortColumn, ref FieldsArrayList);
            this.SortDirection.SetValue(sortDirection, ref FieldsArrayList);
            this.SearchQuery.SetValue(searchQuery, ref FieldsArrayList);
            return (DataSet)base.QueryDatabase(SqlQueryType.FillDataset);
        }

        public int ResetForRetry(string notificationStatusID, string scheduledAt)
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue("ResetForRetry", ref FieldsArrayList);
            this.NotificationStatusID.SetValue(notificationStatusID, ref FieldsArrayList);
            this.ScheduledAt.SetValue(scheduledAt, ref FieldsArrayList);
            return (int)base.QueryDatabase(SqlQueryType.ExecuteNonQuery);
=======
        private readonly IConfiguration _configuration;
        private readonly Common.ErrorHandler _errorHandler;

        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField NotificationStatusID = new TableField("NotificationStatusID", SqlDbType.Int);
        public TableField NotificationTemplateID = new TableField("NotificationTemplateID", SqlDbType.Int);
        public TableField Email = new TableField("Email", SqlDbType.NVarChar);
        public TableField CCEmails = new TableField("CCEmails", SqlDbType.NVarChar);
        public TableField BCCEmails = new TableField("BCCEmails", SqlDbType.NVarChar);
        public TableField CountryCode = new TableField("CountryCode", SqlDbType.NVarChar);
        public TableField MobileNo = new TableField("MobileNo", SqlDbType.NVarChar);
        public TableField ChannelID = new TableField("ChannelID", SqlDbType.Int);
        public TableField ConnectionID = new TableField("ConnectionID", SqlDbType.Int);
        public TableField ToUserID = new TableField("ToUserID", SqlDbType.Int);
        public TableField Sent = new TableField("Sent", SqlDbType.Bit);
        public TableField Success = new TableField("Success", SqlDbType.Bit);
        public TableField TryAgain = new TableField("TryAgain", SqlDbType.Bit);
        public TableField TryDate = new TableField("TryDate", SqlDbType.DateTime);
        public TableField LastTryDate = new TableField("LastTryDate", SqlDbType.DateTime);
        public TableField NoOfTry = new TableField("NoOfTry", SqlDbType.Int);
        public TableField MaxTry = new TableField("MaxTry", SqlDbType.Int);
        public TableField Priority = new TableField("Priority", SqlDbType.Int);
        public TableField ScheduledAt = new TableField("ScheduledAt", SqlDbType.DateTime);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField ErrorStack = new TableField("ErrorStack", SqlDbType.NVarChar);
        public TableField PayloadJson = new TableField("PayloadJson", SqlDbType.NVarChar);
        public TableField BatchSize = new TableField("BatchSize", SqlDbType.Int);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);

        public DBS7SNotificationSP(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            _errorHandler = new Common.ErrorHandler(configuration);
            SPName = "S7SNotificationSP";
        }

        public DataSet RtvPendingNotifications(int batchSize, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("rtvPendingNotifications", ref FieldsArrayList);
                BatchSize.SetValue(batchSize.ToString(), ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "RtvPendingNotifications");
                return new DataSet();
            }
        }

        public int MarkSent(int notificationStatusID, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("MarkSent", ref FieldsArrayList);
                NotificationStatusID.SetValue(notificationStatusID.ToString(), ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (int)QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"MarkSent {notificationStatusID}");
                return -1;
            }
        }

        public int MarkFailed(int notificationStatusID, string errorMessage, string errorStack, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("MarkFailed", ref FieldsArrayList);
                NotificationStatusID.SetValue(notificationStatusID.ToString(), ref FieldsArrayList);
                ErrorMessage.SetValue(errorMessage ?? string.Empty, ref FieldsArrayList);
                ErrorStack.SetValue(errorStack ?? string.Empty, ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (int)QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"MarkFailed {notificationStatusID}");
                return -1;
            }
        }

        public DataSet SaveTemplate(Models.Notification.S7SNotificationTemplate template, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("SaveTemplate", ref FieldsArrayList);
                NotificationTemplateID.SetValue(template.NotificationTemplateID?.ToString() ?? string.Empty, ref FieldsArrayList);
                new TableField("TemplateKey", SqlDbType.NVarChar).SetValue(template.TemplateKey ?? string.Empty, ref FieldsArrayList);
                new TableField("Name", SqlDbType.NVarChar).SetValue(template.Name ?? string.Empty, ref FieldsArrayList);
                ChannelID.SetValue(template.ChannelID.ToString(), ref FieldsArrayList);
                new TableField("Subject", SqlDbType.NVarChar).SetValue(template.Subject ?? string.Empty, ref FieldsArrayList);
                new TableField("Body", SqlDbType.NVarChar).SetValue(template.Body ?? string.Empty, ref FieldsArrayList);
                new TableField("UseDesign", SqlDbType.Bit).SetValue(template.UseDesign ? "1" : "0", ref FieldsArrayList);
                new TableField("Attachments", SqlDbType.NVarChar).SetValue(template.Attachments ?? string.Empty, ref FieldsArrayList);
                new TableField("CompanyID", SqlDbType.Int).SetValue(template.CompanyID?.ToString() ?? string.Empty, ref FieldsArrayList);
                new TableField("IsActive", SqlDbType.Bit).SetValue(template.IsActive ? "1" : "0", ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveTemplate");
                return new DataSet();
            }
        }

        public DataSet RtvTemplate(int templateID, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("rtvTemplate", ref FieldsArrayList);
                NotificationTemplateID.SetValue(templateID.ToString(), ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "RtvTemplate");
                return new DataSet();
            }
        }

        public DataSet RtvTemplates(int? channelId, bool? isActive, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("rtvTemplates", ref FieldsArrayList);
                if (channelId.HasValue)
                {
                    ChannelID.SetValue(channelId.Value.ToString(), ref FieldsArrayList);
                }
                if (isActive.HasValue)
                {
                    new TableField("IsActive", SqlDbType.Bit).SetValue(isActive.Value ? "1" : "0", ref FieldsArrayList);
                }
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "RtvTemplates");
                return new DataSet();
            }
        }

        public DataSet RtvChannels()
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("rtvChannels", ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "RtvChannels");
                return new DataSet();
            }
        }

        public DataSet RtvConnections(int? channelId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("rtvConnections", ref FieldsArrayList);
                if (channelId.HasValue)
                {
                    ChannelID.SetValue(channelId.Value.ToString(), ref FieldsArrayList);
                }
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "RtvConnections");
                return new DataSet();
            }
        }

        public DataSet SaveAll(Models.Notification.S7SNotificationStatus status, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("SaveAll", ref FieldsArrayList);
                NotificationStatusID.SetValue(status.S7SNotificationStatusID?.ToString() ?? string.Empty, ref FieldsArrayList);
                NotificationTemplateID.SetValue(status.NotificationTemplateID.ToString(), ref FieldsArrayList);
                Email.SetValue(status.Email ?? string.Empty, ref FieldsArrayList);
                CCEmails.SetValue(status.CCEmails ?? string.Empty, ref FieldsArrayList);
                BCCEmails.SetValue(status.BCCEmails ?? string.Empty, ref FieldsArrayList);
                CountryCode.SetValue(status.CountryCode ?? string.Empty, ref FieldsArrayList);
                MobileNo.SetValue(status.MobileNo ?? string.Empty, ref FieldsArrayList);
                ChannelID.SetValue(status.ChannelID.ToString(), ref FieldsArrayList);
                ConnectionID.SetValue(status.ConnectionID?.ToString() ?? string.Empty, ref FieldsArrayList);
                ToUserID.SetValue(status.ToUserID?.ToString() ?? string.Empty, ref FieldsArrayList);
                Sent.SetValue(status.Sent ? "1" : "0", ref FieldsArrayList);
                Success.SetValue(status.Success ? "1" : "0", ref FieldsArrayList);
                Priority.SetValue(status.Priority.ToString(), ref FieldsArrayList);
                ScheduledAt.SetValue(status.ScheduledAt?.ToString("s") ?? string.Empty, ref FieldsArrayList);
                PayloadJson.SetValue(status.PayloadJson ?? string.Empty, ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveAll");
                return new DataSet();
            }
        }

        public int DeleteTemplate(int templateID, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("DeleteTemplate", ref FieldsArrayList);
                NotificationTemplateID.SetValue(templateID.ToString(), ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (int)QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, "DeleteTemplate");
                return -1;
            }
        }

        public DataSet SaveStatus(Models.Notification.S7SNotificationStatus status, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("SaveStatus", ref FieldsArrayList);
                NotificationStatusID.SetValue(status.S7SNotificationStatusID?.ToString() ?? string.Empty, ref FieldsArrayList);
                NotificationTemplateID.SetValue(status.NotificationTemplateID.ToString(), ref FieldsArrayList);
                Email.SetValue(status.Email ?? string.Empty, ref FieldsArrayList);
                CCEmails.SetValue(status.CCEmails ?? string.Empty, ref FieldsArrayList);
                BCCEmails.SetValue(status.BCCEmails ?? string.Empty, ref FieldsArrayList);
                CountryCode.SetValue(status.CountryCode ?? string.Empty, ref FieldsArrayList);
                MobileNo.SetValue(status.MobileNo ?? string.Empty, ref FieldsArrayList);
                ChannelID.SetValue(status.ChannelID.ToString(), ref FieldsArrayList);
                ConnectionID.SetValue(status.ConnectionID?.ToString() ?? string.Empty, ref FieldsArrayList);
                ToUserID.SetValue(status.ToUserID?.ToString() ?? string.Empty, ref FieldsArrayList);
                Sent.SetValue(status.Sent ? "1" : "0", ref FieldsArrayList);
                Success.SetValue(status.Success ? "1" : "0", ref FieldsArrayList);
                TryAgain.SetValue(status.TryAgain ? "1" : "0", ref FieldsArrayList);
                TryDate.SetValue(status.TryDate?.ToString("s") ?? string.Empty, ref FieldsArrayList);
                LastTryDate.SetValue(status.LastTryDate?.ToString("s") ?? string.Empty, ref FieldsArrayList);
                NoOfTry.SetValue(status.NoOfTry.ToString(), ref FieldsArrayList);
                MaxTry.SetValue(status.MaxTry.ToString(), ref FieldsArrayList);
                Priority.SetValue(status.Priority.ToString(), ref FieldsArrayList);
                ScheduledAt.SetValue(status.ScheduledAt?.ToString("s") ?? string.Empty, ref FieldsArrayList);
                ErrorMessage.SetValue(status.ErrorMessage ?? string.Empty, ref FieldsArrayList);
                ErrorStack.SetValue(status.ErrorStack ?? string.Empty, ref FieldsArrayList);
                PayloadJson.SetValue(status.PayloadJson ?? string.Empty, ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveStatus");
                return new DataSet();
            }
        }

        public DataSet GetStatus(int notificationStatusID, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("GetStatus", ref FieldsArrayList);
                NotificationStatusID.SetValue(notificationStatusID.ToString(), ref FieldsArrayList);
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "GetStatus");
                return new DataSet();
            }
        }

        public DataSet ListStatus(int? channelId, int? templateId, bool? sent, bool? success, int currentUserId)
        {
            try
            {
                FieldsArrayList = new ArrayList();
                Operation.SetValue("ListStatus", ref FieldsArrayList);
                if (channelId.HasValue)
                {
                    ChannelID.SetValue(channelId.Value.ToString(), ref FieldsArrayList);
                }
                if (templateId.HasValue)
                {
                    NotificationTemplateID.SetValue(templateId.Value.ToString(), ref FieldsArrayList);
                }
                if (sent.HasValue)
                {
                    Sent.SetValue(sent.Value ? "1" : "0", ref FieldsArrayList);
                }
                if (success.HasValue)
                {
                    Success.SetValue(success.Value ? "1" : "0", ref FieldsArrayList);
                }
                CurrentUserID.SetValue(currentUserId.ToString(), ref FieldsArrayList);
                return (DataSet)QueryDatabase(SqlQueryType.FillDataset);
            }
            catch (Exception ex)
            {
                LogError(ex, "ListStatus");
                return new DataSet();
            }
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                _errorHandler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: ex.Message,
                    Form: context,
                    Source: ex.Source,
                    TargetSite: ex.TargetSite?.Name,
                    StackTrace: ex.StackTrace);
            }
            catch
            {
            }
>>>>>>> theirs
        }
    }
}
