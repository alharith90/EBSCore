using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Notification
{
    public class DBS7SNotificationSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField NotificationStatusID = new TableField("NotificationStatusID", SqlDbType.Int);
        public TableField NotificationTemplateID = new TableField("NotificationTemplateID", SqlDbType.Int);
        public TableField TemplateKey = new TableField("TemplateKey", SqlDbType.NVarChar);
        public TableField Name = new TableField("Name", SqlDbType.NVarChar);
        public TableField ChannelID = new TableField("ChannelID", SqlDbType.Int);
        public TableField ConnectionID = new TableField("ConnectionID", SqlDbType.Int);
        public TableField Subject = new TableField("Subject", SqlDbType.NVarChar);
        public TableField Body = new TableField("Body", SqlDbType.NVarChar);
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
        public TableField TryDate = new TableField("TryDate", SqlDbType.DateTime);
        public TableField MaxTry = new TableField("MaxTry", SqlDbType.Int);
        public TableField Priority = new TableField("Priority", SqlDbType.Int);
        public TableField ScheduledAt = new TableField("ScheduledAt", SqlDbType.DateTime);
        public TableField ExceptionID = new TableField("ExceptionID", SqlDbType.Int);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField ErrorStack = new TableField("ErrorStack", SqlDbType.NVarChar);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public TableField BatchSize = new TableField("BatchSize", SqlDbType.Int);
        public TableField PayloadJson = new TableField("PayloadJson", SqlDbType.NVarChar);

        public DBS7SNotificationSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SNotificationSP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
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
            string BatchSize = "",
            string PayloadJson = "")
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
            this.PayloadJson.SetValue(PayloadJson, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
