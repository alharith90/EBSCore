using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBHSRiskAssessmentSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField HazardID = new TableField("HazardID", SqlDbType.Int);
        public TableField HazardDescription = new TableField("HazardDescription", SqlDbType.NVarChar);
        public TableField LocationArea = new TableField("LocationArea", SqlDbType.NVarChar);
        public TableField RelatedActivity = new TableField("RelatedActivity", SqlDbType.NVarChar);
        public TableField PotentialImpact = new TableField("PotentialImpact", SqlDbType.NVarChar);
        public TableField Likelihood = new TableField("Likelihood", SqlDbType.NVarChar);
        public TableField Severity = new TableField("Severity", SqlDbType.NVarChar);
        public TableField RiskLevel = new TableField("RiskLevel", SqlDbType.NVarChar);
        public TableField ExistingControls = new TableField("ExistingControls", SqlDbType.NVarChar);
        public TableField AdditionalControlsNeeded = new TableField("AdditionalControlsNeeded", SqlDbType.NVarChar);
        public TableField RiskOwner = new TableField("RiskOwner", SqlDbType.NVarChar);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBHSRiskAssessmentSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "HSRiskAssessmentSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string HazardID = "",
            string HazardDescription = "", string LocationArea = "", string RelatedActivity = "", string PotentialImpact = "",
            string Likelihood = "", string Severity = "", string RiskLevel = "", string ExistingControls = "",
            string AdditionalControlsNeeded = "", string RiskOwner = "", string NextReviewDate = "", string Status = "",
            string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.HazardID.SetValue(HazardID, ref FieldsArrayList);
            this.HazardDescription.SetValue(HazardDescription, ref FieldsArrayList);
            this.LocationArea.SetValue(LocationArea, ref FieldsArrayList);
            this.RelatedActivity.SetValue(RelatedActivity, ref FieldsArrayList);
            this.PotentialImpact.SetValue(PotentialImpact, ref FieldsArrayList);
            this.Likelihood.SetValue(Likelihood, ref FieldsArrayList);
            this.Severity.SetValue(Severity, ref FieldsArrayList);
            this.RiskLevel.SetValue(RiskLevel, ref FieldsArrayList);
            this.ExistingControls.SetValue(ExistingControls, ref FieldsArrayList);
            this.AdditionalControlsNeeded.SetValue(AdditionalControlsNeeded, ref FieldsArrayList);
            this.RiskOwner.SetValue(RiskOwner, ref FieldsArrayList);
            this.NextReviewDate.SetValue(NextReviewDate, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
