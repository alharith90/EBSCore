using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBS7SWorkflowExecutionSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField WorkflowID = new TableField("WorkflowID", SqlDbType.Int);
        public TableField ExecutionID = new TableField("ExecutionID", SqlDbType.BigInt);
        public TableField PayloadJson = new TableField("PayloadJson", SqlDbType.NVarChar);
        public TableField WebhookSecret = new TableField("WebhookSecret", SqlDbType.NVarChar);
        public TableField RequestJson = new TableField("RequestJson", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField ErrorMessage = new TableField("ErrorMessage", SqlDbType.NVarChar);
        public TableField NodeID = new TableField("NodeID", SqlDbType.Int);
        public TableField OutputJson = new TableField("OutputJson", SqlDbType.NVarChar);
        public TableField OutputKey = new TableField("OutputKey", SqlDbType.NVarChar);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);

        public DBS7SWorkflowExecutionSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SWorkflowExecutionSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string WorkflowID = "",
            string ExecutionID = "",
            string PayloadJson = "",
            string WebhookSecret = "",
            string RequestJson = "",
            string Status = "",
            string ErrorMessage = "",
            string NodeID = "",
            string OutputJson = "",
            string OutputKey = "",
            string PageNumber = "",
            string PageSize = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.WorkflowID.SetValue(WorkflowID, ref FieldsArrayList);
            this.ExecutionID.SetValue(ExecutionID, ref FieldsArrayList);
            this.PayloadJson.SetValue(PayloadJson, ref FieldsArrayList);
            this.WebhookSecret.SetValue(WebhookSecret, ref FieldsArrayList);
            this.RequestJson.SetValue(RequestJson, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.ErrorMessage.SetValue(ErrorMessage, ref FieldsArrayList);
            this.NodeID.SetValue(NodeID, ref FieldsArrayList);
            this.OutputJson.SetValue(OutputJson, ref FieldsArrayList);
            this.OutputKey.SetValue(OutputKey, ref FieldsArrayList);
            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
