using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBWorkflowSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField WorkflowID = new TableField("WorkflowID", SqlDbType.Int);
        public TableField WorkflowCode = new TableField("WorkflowCode", SqlDbType.NVarChar);
        public TableField WorkflowName = new TableField("WorkflowName", SqlDbType.NVarChar);
        public TableField WorkflowDescription = new TableField("WorkflowDescription", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField Priority = new TableField("Priority", SqlDbType.NVarChar);
        public TableField Frequency = new TableField("Frequency", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField IsActive = new TableField("IsActive", SqlDbType.Bit);
        public TableField NodesJson = new TableField("NodesJson", SqlDbType.NVarChar);
        public TableField ConnectionsJson = new TableField("ConnectionsJson", SqlDbType.NVarChar);
        public TableField TriggersJson = new TableField("TriggersJson", SqlDbType.NVarChar);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);

        public DBWorkflowSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SWorkflowSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string WorkflowID = "",
            string WorkflowCode = "",
            string WorkflowName = "",
            string WorkflowDescription = "",
            string Status = "",
            string Priority = "",
            string Frequency = "",
            string Notes = "",
            string IsActive = "",
            string NodesJson = "",
            string ConnectionsJson = "",
            string TriggersJson = "",
            string PageNumber = "",
            string PageSize = "",
            string SearchQuery = "",
            string SortColumn = "",
            string SortDirection = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.WorkflowID.SetValue(WorkflowID, ref FieldsArrayList);
            this.WorkflowCode.SetValue(WorkflowCode, ref FieldsArrayList);
            this.WorkflowName.SetValue(WorkflowName, ref FieldsArrayList);
            this.WorkflowDescription.SetValue(WorkflowDescription, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.Priority.SetValue(Priority, ref FieldsArrayList);
            this.Frequency.SetValue(Frequency, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);
            this.IsActive.SetValue(IsActive, ref FieldsArrayList);
            this.NodesJson.SetValue(NodesJson, ref FieldsArrayList);
            this.ConnectionsJson.SetValue(ConnectionsJson, ref FieldsArrayList);
            this.TriggersJson.SetValue(TriggersJson, ref FieldsArrayList);
            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);
            this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);
            this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
            this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
