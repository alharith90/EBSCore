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
        public TableField ChannelID = new TableField("ChannelID", SqlDbType.Int);
        public TableField ConnectionID = new TableField("ConnectionID", SqlDbType.Int);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);
        public TableField PayloadJson = new TableField("PayloadJson", SqlDbType.NVarChar);

        public DBS7SNotificationSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SNotificationSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string NotificationStatusID = "", string NotificationTemplateID = "",
            string ChannelID = "", string ConnectionID = "", string CompanyID = "", string UserID = "",
            string PayloadJson = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.NotificationStatusID.SetValue(NotificationStatusID, ref FieldsArrayList);
            this.NotificationTemplateID.SetValue(NotificationTemplateID, ref FieldsArrayList);
            this.ChannelID.SetValue(ChannelID, ref FieldsArrayList);
            this.ConnectionID.SetValue(ConnectionID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.PayloadJson.SetValue(PayloadJson, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
