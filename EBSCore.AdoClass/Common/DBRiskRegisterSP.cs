using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRiskRegisterSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField RiskID = new TableField("RiskID", SqlDbType.Int);
        public TableField RiskTitle = new TableField("RiskTitle", SqlDbType.NVarChar);
        public TableField RiskDescription = new TableField("RiskDescription", SqlDbType.NVarChar);
        public TableField RiskCategory = new TableField("RiskCategory", SqlDbType.NVarChar);
        public TableField RiskSource = new TableField("RiskSource", SqlDbType.NVarChar);
        public TableField PotentialImpact = new TableField("PotentialImpact", SqlDbType.NVarChar);
        public TableField InherentLikelihood = new TableField("InherentLikelihood", SqlDbType.NVarChar);
        public TableField InherentImpact = new TableField("InherentImpact", SqlDbType.NVarChar);
        public TableField InherentRiskLevel = new TableField("InherentRiskLevel", SqlDbType.NVarChar);
        public TableField ExistingControls = new TableField("ExistingControls", SqlDbType.NVarChar);
        public TableField ControlEffectiveness = new TableField("ControlEffectiveness", SqlDbType.NVarChar);
        public TableField ResidualLikelihood = new TableField("ResidualLikelihood", SqlDbType.NVarChar);
        public TableField ResidualImpact = new TableField("ResidualImpact", SqlDbType.NVarChar);
        public TableField ResidualRiskLevel = new TableField("ResidualRiskLevel", SqlDbType.NVarChar);
        public TableField RiskAppetiteThreshold = new TableField("RiskAppetiteThreshold", SqlDbType.NVarChar);
        public TableField RiskResponseStrategy = new TableField("RiskResponseStrategy", SqlDbType.NVarChar);
        public TableField TreatmentDecision = new TableField("TreatmentDecision", SqlDbType.NVarChar);
        public TableField TreatmentPlanID = new TableField("TreatmentPlanID", SqlDbType.Int);
        public TableField Likelihood = new TableField("Likelihood", SqlDbType.NVarChar);
        public TableField Impact = new TableField("Impact", SqlDbType.NVarChar);
        public TableField RiskScore = new TableField("RiskScore", SqlDbType.Int);
        public TableField RiskResponse = new TableField("RiskResponse", SqlDbType.NVarChar);
        public TableField RiskOwner = new TableField("RiskOwner", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField ReviewDate = new TableField("ReviewDate", SqlDbType.DateTime);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.DateTime);
        public TableField RiskTrend = new TableField("RiskTrend", SqlDbType.NVarChar);
        public TableField RelatedObjectives = new TableField("RelatedObjectives", SqlDbType.NVarChar);
        public TableField RelatedIncidents = new TableField("RelatedIncidents", SqlDbType.NVarChar);
        public TableField RelatedControls = new TableField("RelatedControls", SqlDbType.NVarChar);
        public TableField RelatedObligations = new TableField("RelatedObligations", SqlDbType.NVarChar);
        public TableField MonitoringFrequency = new TableField("MonitoringFrequency", SqlDbType.NVarChar);
        public TableField LastMonitoringDate = new TableField("LastMonitoringDate", SqlDbType.DateTime);
        public TableField KRIName = new TableField("KRIName", SqlDbType.NVarChar);
        public TableField KRIValue = new TableField("KRIValue", SqlDbType.NVarChar);
        public TableField KRIStatus = new TableField("KRIStatus", SqlDbType.NVarChar);
        public TableField RiskAlertTrigger = new TableField("RiskAlertTrigger", SqlDbType.NVarChar);
        public TableField NextMonitoringDate = new TableField("NextMonitoringDate", SqlDbType.DateTime);
        public TableField RiskHistoryNotes = new TableField("RiskHistoryNotes", SqlDbType.NVarChar);
        public TableField LastUpdatedBy = new TableField("LastUpdatedBy", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBRiskRegisterSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "RiskRegisterSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string RiskID = "",
            string RiskTitle = "", string RiskDescription = "", string RiskCategory = "", string RiskSource = "",
            string PotentialImpact = "", string InherentLikelihood = "", string InherentImpact = "",
            string InherentRiskLevel = "", string ExistingControls = "", string ControlEffectiveness = "",
            string ResidualLikelihood = "", string ResidualImpact = "", string ResidualRiskLevel = "",
            string RiskAppetiteThreshold = "", string RiskResponseStrategy = "", string TreatmentDecision = "", string TreatmentPlanID = "",
            string Likelihood = "", string Impact = "", string RiskScore = "",
            string RiskResponse = "", string RiskOwner = "", string Status = "", string ReviewDate = "", string NextReviewDate = "",
            string RiskTrend = "", string RelatedObjectives = "", string RelatedIncidents = "", string RelatedControls = "",
            string RelatedObligations = "", string MonitoringFrequency = "", string LastMonitoringDate = "", string KRIName = "",
            string KRIValue = "", string KRIStatus = "", string RiskAlertTrigger = "", string NextMonitoringDate = "",
            string RiskHistoryNotes = "", string LastUpdatedBy = "", string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.RiskID.SetValue(RiskID, ref FieldsArrayList);
            this.RiskTitle.SetValue(RiskTitle, ref FieldsArrayList);
            this.RiskDescription.SetValue(RiskDescription, ref FieldsArrayList);
            this.RiskCategory.SetValue(RiskCategory, ref FieldsArrayList);
            this.RiskSource.SetValue(RiskSource, ref FieldsArrayList);
            this.PotentialImpact.SetValue(PotentialImpact, ref FieldsArrayList);
            this.InherentLikelihood.SetValue(InherentLikelihood, ref FieldsArrayList);
            this.InherentImpact.SetValue(InherentImpact, ref FieldsArrayList);
            this.InherentRiskLevel.SetValue(InherentRiskLevel, ref FieldsArrayList);
            this.ExistingControls.SetValue(ExistingControls, ref FieldsArrayList);
            this.ControlEffectiveness.SetValue(ControlEffectiveness, ref FieldsArrayList);
            this.ResidualLikelihood.SetValue(ResidualLikelihood, ref FieldsArrayList);
            this.ResidualImpact.SetValue(ResidualImpact, ref FieldsArrayList);
            this.ResidualRiskLevel.SetValue(ResidualRiskLevel, ref FieldsArrayList);
            this.RiskAppetiteThreshold.SetValue(RiskAppetiteThreshold, ref FieldsArrayList);
            this.RiskResponseStrategy.SetValue(RiskResponseStrategy, ref FieldsArrayList);
            this.TreatmentDecision.SetValue(TreatmentDecision, ref FieldsArrayList);
            this.TreatmentPlanID.SetValue(TreatmentPlanID, ref FieldsArrayList);
            this.Likelihood.SetValue(Likelihood, ref FieldsArrayList);
            this.Impact.SetValue(Impact, ref FieldsArrayList);
            this.RiskScore.SetValue(RiskScore, ref FieldsArrayList);
            this.RiskResponse.SetValue(RiskResponse, ref FieldsArrayList);
            this.RiskOwner.SetValue(RiskOwner, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.ReviewDate.SetValue(ReviewDate, ref FieldsArrayList);
            this.NextReviewDate.SetValue(NextReviewDate, ref FieldsArrayList);
            this.RiskTrend.SetValue(RiskTrend, ref FieldsArrayList);
            this.RelatedObjectives.SetValue(RelatedObjectives, ref FieldsArrayList);
            this.RelatedIncidents.SetValue(RelatedIncidents, ref FieldsArrayList);
            this.RelatedControls.SetValue(RelatedControls, ref FieldsArrayList);
            this.RelatedObligations.SetValue(RelatedObligations, ref FieldsArrayList);
            this.MonitoringFrequency.SetValue(MonitoringFrequency, ref FieldsArrayList);
            this.LastMonitoringDate.SetValue(LastMonitoringDate, ref FieldsArrayList);
            this.KRIName.SetValue(KRIName, ref FieldsArrayList);
            this.KRIValue.SetValue(KRIValue, ref FieldsArrayList);
            this.KRIStatus.SetValue(KRIStatus, ref FieldsArrayList);
            this.RiskAlertTrigger.SetValue(RiskAlertTrigger, ref FieldsArrayList);
            this.NextMonitoringDate.SetValue(NextMonitoringDate, ref FieldsArrayList);
            this.RiskHistoryNotes.SetValue(RiskHistoryNotes, ref FieldsArrayList);
            this.LastUpdatedBy.SetValue(LastUpdatedBy, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
