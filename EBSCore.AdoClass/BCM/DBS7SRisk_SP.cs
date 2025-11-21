using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBS7SRisk_SP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        public TableField RiskID = new TableField("RiskID", SqlDbType.Int);
        public TableField BCMRiskAssessmentId = new TableField("BCMRiskAssessmentId", SqlDbType.Int);
        public TableField ImpactAspectID = new TableField("ImpactAspectID", SqlDbType.Int);
        public TableField ImpactTimeFrameID = new TableField("ImpactTimeFrameID", SqlDbType.Int);
        public TableField ImpactID = new TableField("ImpactID", SqlDbType.Int);
        public TableField LikelihoodID = new TableField("LikelihoodID", SqlDbType.Int);
        public TableField RiskCategoryID = new TableField("RiskCategoryID", SqlDbType.Int);
        public TableField RiskTitle = new TableField("RiskTitle", SqlDbType.NVarChar);
        public TableField RiskDescription = new TableField("RiskDescription", SqlDbType.NVarChar);
        public TableField MitigationPlan = new TableField("MitigationPlan", SqlDbType.NVarChar);
        public TableField MatrixName = new TableField("MatrixName", SqlDbType.NVarChar);
        public TableField MatrixSize = new TableField("MatrixSize", SqlDbType.Int);
        public TableField IsDynamic = new TableField("IsDynamic", SqlDbType.Bit);
        public TableField ConfigJson = new TableField("ConfigJson", SqlDbType.NVarChar);
        public TableField RiskMatrixConfigID = new TableField("RiskMatrixConfigID", SqlDbType.Int);
        public TableField RiskToleranceID = new TableField("RiskToleranceID", SqlDbType.Int);
        public TableField HighThreshold = new TableField("HighThreshold", SqlDbType.Int);
        public TableField MediumThreshold = new TableField("MediumThreshold", SqlDbType.Int);
        public TableField LowLabel = new TableField("LowLabel", SqlDbType.NVarChar);
        public TableField MediumLabel = new TableField("MediumLabel", SqlDbType.NVarChar);
        public TableField HighLabel = new TableField("HighLabel", SqlDbType.NVarChar);
        public TableField LikelihoodName = new TableField("LikelihoodName", SqlDbType.NVarChar);
        public TableField LikelihoodValue = new TableField("LikelihoodValue", SqlDbType.Int);
        public TableField LikelihoodDescription = new TableField("LikelihoodDescription", SqlDbType.NVarChar);
        public TableField CategoryName = new TableField("CategoryName", SqlDbType.NVarChar);
        public TableField CategoryDescription = new TableField("CategoryDescription", SqlDbType.NVarChar);
        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);

        public DBS7SRisk_SP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SRisk_SP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string CurrentUserID = "", string RiskID = "", string BCMRiskAssessmentId = "",
            string ImpactAspectID = "", string ImpactTimeFrameID = "", string ImpactID = "", string LikelihoodID = "",
            string RiskCategoryID = "", string RiskTitle = "", string RiskDescription = "", string MitigationPlan = "",
            string MatrixName = "", string MatrixSize = "", string IsDynamic = "", string ConfigJson = "", string RiskMatrixConfigID = "",
            string RiskToleranceID = "", string HighThreshold = "", string MediumThreshold = "", string LowLabel = "",
            string MediumLabel = "", string HighLabel = "", string LikelihoodName = "", string LikelihoodValue = "",
            string LikelihoodDescription = "", string CategoryName = "", string CategoryDescription = "",
            string PageSize = "", string PageNumber = "", string SearchQuery = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.RiskID.SetValue(RiskID, ref FieldsArrayList);
            this.BCMRiskAssessmentId.SetValue(BCMRiskAssessmentId, ref FieldsArrayList);
            this.ImpactAspectID.SetValue(ImpactAspectID, ref FieldsArrayList);
            this.ImpactTimeFrameID.SetValue(ImpactTimeFrameID, ref FieldsArrayList);
            this.ImpactID.SetValue(ImpactID, ref FieldsArrayList);
            this.LikelihoodID.SetValue(LikelihoodID, ref FieldsArrayList);
            this.RiskCategoryID.SetValue(RiskCategoryID, ref FieldsArrayList);
            this.RiskTitle.SetValue(RiskTitle, ref FieldsArrayList);
            this.RiskDescription.SetValue(RiskDescription, ref FieldsArrayList);
            this.MitigationPlan.SetValue(MitigationPlan, ref FieldsArrayList);
            this.MatrixName.SetValue(MatrixName, ref FieldsArrayList);
            this.MatrixSize.SetValue(MatrixSize, ref FieldsArrayList);
            this.IsDynamic.SetValue(IsDynamic, ref FieldsArrayList);
            this.ConfigJson.SetValue(ConfigJson, ref FieldsArrayList);
            this.RiskMatrixConfigID.SetValue(RiskMatrixConfigID, ref FieldsArrayList);
            this.RiskToleranceID.SetValue(RiskToleranceID, ref FieldsArrayList);
            this.HighThreshold.SetValue(HighThreshold, ref FieldsArrayList);
            this.MediumThreshold.SetValue(MediumThreshold, ref FieldsArrayList);
            this.LowLabel.SetValue(LowLabel, ref FieldsArrayList);
            this.MediumLabel.SetValue(MediumLabel, ref FieldsArrayList);
            this.HighLabel.SetValue(HighLabel, ref FieldsArrayList);
            this.LikelihoodName.SetValue(LikelihoodName, ref FieldsArrayList);
            this.LikelihoodValue.SetValue(LikelihoodValue, ref FieldsArrayList);
            this.LikelihoodDescription.SetValue(LikelihoodDescription, ref FieldsArrayList);
            this.CategoryName.SetValue(CategoryName, ref FieldsArrayList);
            this.CategoryDescription.SetValue(CategoryDescription, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);
            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);
            return base.QueryDatabase(QueryType);
        }
    }
}
