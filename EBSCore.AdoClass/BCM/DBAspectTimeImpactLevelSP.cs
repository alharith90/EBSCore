using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBAspectTimeImpactLevelSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        public TableField AspectTimeImpactLevelID = new TableField("AspectTimeImpactLevelID", SqlDbType.Int);
        public TableField ImpactAspectID = new TableField("ImpactAspectID", SqlDbType.Int);
        public TableField ImpactTimeFrameID = new TableField("ImpactTimeFrameID", SqlDbType.Int);
        public TableField LevelID = new TableField("LevelID", SqlDbType.Int);
        public TableField ImpactLevel = new TableField("ImpactLevel", SqlDbType.NVarChar);
        public TableField Justification = new TableField("Justification", SqlDbType.NVarChar);
        public TableField ImpactColor = new TableField("ImpactColor", SqlDbType.NVarChar);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchFields = new TableField("SearchFields", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);
        public DBAspectTimeImpactLevelSP(IConfiguration configuration) : base(configuration) { base.SPName = "BCMAspectTimeImpactLevelSP"; }
        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string CurrentUserID = "", string AspectTimeImpactLevelID = "",
            string ImpactAspectID = "", string ImpactTimeFrameID = "", string LevelID = "",
            string ImpactLevel = "", string Justification = "", string ImpactColor = "",
            string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = "",
            string SearchFields = "", string SearchQuery = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.AspectTimeImpactLevelID.SetValue(AspectTimeImpactLevelID, ref FieldsArrayList);
            this.ImpactAspectID.SetValue(ImpactAspectID, ref FieldsArrayList);
            this.ImpactTimeFrameID.SetValue(ImpactTimeFrameID, ref FieldsArrayList);
            this.LevelID.SetValue(LevelID, ref FieldsArrayList);
            this.ImpactLevel.SetValue(ImpactLevel, ref FieldsArrayList);
            this.Justification.SetValue(Justification, ref FieldsArrayList);
            this.ImpactColor.SetValue(ImpactColor, ref FieldsArrayList);
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
