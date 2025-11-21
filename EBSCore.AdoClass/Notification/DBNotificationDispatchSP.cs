using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Notification
{
    public class DBNotificationDispatchSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField OutboxID = new TableField("OutboxID", SqlDbType.BigInt);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField ResponseJson = new TableField("ResponseJson", SqlDbType.NVarChar);
        public TableField LockForSeconds = new TableField("LockForSeconds", SqlDbType.Int);

        public DBNotificationDispatchSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SNotificationDispatchSP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
            string Operation = "",
            string OutboxID = "",
            string Status = "",
            string ErrorMessage = "",
            string ResponseJson = "",
            string LockForSeconds = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.OutboxID.SetValue(OutboxID, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.ErrorMessage.SetValue(ErrorMessage, ref FieldsArrayList);
            this.ResponseJson.SetValue(ResponseJson, ref FieldsArrayList);
            this.LockForSeconds.SetValue(LockForSeconds, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
