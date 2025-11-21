using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBNotificationDispatchSP : DBParentStoredProcedureClass
    {
        private readonly IConfiguration _configuration;
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField OutboxID = new TableField("OutboxID", SqlDbType.BigInt);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField ResponseJson = new TableField("ResponseJson", SqlDbType.NVarChar);
        public TableField LockForSeconds = new TableField("LockForSeconds", SqlDbType.Int);

        public DBNotificationDispatchSP(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            SPName = "S7SNotificationDispatchSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string OutboxID = "",
            string Status = "",
            string ErrorMessage = "",
            string ResponseJson = "",
            string LockForSeconds = ""
        )
        {
            try
            {
                FieldsArrayList = new ArrayList();

                this.Operation.SetValue(Operation, ref FieldsArrayList);
                this.OutboxID.SetValue(OutboxID, ref FieldsArrayList);
                this.Status.SetValue(Status, ref FieldsArrayList);
                this.ErrorMessage.SetValue(ErrorMessage, ref FieldsArrayList);
                this.ResponseJson.SetValue(ResponseJson, ref FieldsArrayList);
                this.LockForSeconds.SetValue(LockForSeconds, ref FieldsArrayList);

                LogInfo("DBNotificationDispatchSP.QueryDatabase", $"Operation:{Operation} OutboxID:{OutboxID} Status:{Status}");
                return base.QueryDatabase(queryType);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBNotificationDispatchSP.QueryDatabase Operation:{Operation} OutboxID:{OutboxID} Status:{Status}");
                return queryType == SqlQueryType.ExecuteNonQuery ? -1 : new DataSet();
            }
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(_configuration);
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
            }
        }

        private void LogInfo(string message, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(_configuration);
                handler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: message,
                    Form: context,
                    Source: "INFO",
                    TargetSite: "INFO",
                    StackTrace: string.Empty);
            }
            catch
            {
            }
        }
    }
}
