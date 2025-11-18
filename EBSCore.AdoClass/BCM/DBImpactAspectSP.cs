using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBImpactAspectSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        public TableField ImpactAspectID = new TableField("ImpactAspectID", SqlDbType.Int);
        public TableField AspectName = new TableField("AspectName", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchFields = new TableField("SearchFields", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public DBImpactAspectSP(IConfiguration configuration) : base(configuration) { base.SPName = "BCMImpactAspectSP"; }
        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string CurrentUserID = "", string ImpactAspectID = "",
            string AspectName = "", string Description = "",
            string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = "",
            string SearchFields = "", string SearchQuery = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.ImpactAspectID.SetValue(ImpactAspectID, ref FieldsArrayList);
            this.AspectName.SetValue(AspectName, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
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
