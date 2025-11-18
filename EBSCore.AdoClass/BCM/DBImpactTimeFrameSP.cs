using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBImpactTimeFrameSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        public TableField ImpactTimeFrameID = new TableField("ImpactTimeFrameID", SqlDbType.Int);
        public TableField TimeLabel = new TableField("TimeLabel", SqlDbType.NVarChar);
        public TableField MinHours = new TableField("MinHours", SqlDbType.Int);
        public TableField MaxHours = new TableField("MaxHours", SqlDbType.Int);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchFields = new TableField("SearchFields", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public DBImpactTimeFrameSP(IConfiguration configuration) : base(configuration) { base.SPName = "BCMImpactTimeFrameSP"; }
        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string CurrentUserID = "", string ImpactTimeFrameID = "",
            string TimeLabel = "", string MinHours = "", string MaxHours = "",
            string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = "",
            string SearchFields = "", string SearchQuery = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.ImpactTimeFrameID.SetValue(ImpactTimeFrameID, ref FieldsArrayList);
            this.TimeLabel.SetValue(TimeLabel, ref FieldsArrayList);
            this.MinHours.SetValue(MinHours, ref FieldsArrayList);
            this.MaxHours.SetValue(MaxHours, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);
            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
            this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);
            this.SearchFields.SetValue(SearchFields, ref FieldsArrayList);
            this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);
            return base.QueryDatabase(QueryType);
        }
    }
}
