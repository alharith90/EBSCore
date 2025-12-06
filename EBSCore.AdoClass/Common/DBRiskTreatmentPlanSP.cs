using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRiskTreatmentPlanSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField ActionID = new TableField("ActionID", SqlDbType.Int);
        public TableField RelatedRisk = new TableField("RelatedRisk", SqlDbType.NVarChar);
        public TableField MitigationAction = new TableField("MitigationAction", SqlDbType.NVarChar);
        public TableField ActionOwner = new TableField("ActionOwner", SqlDbType.NVarChar);
        public TableField DueDate = new TableField("DueDate", SqlDbType.DateTime);
        public TableField CompletionStatus = new TableField("CompletionStatus", SqlDbType.NVarChar);
        public TableField TreatmentType = new TableField("TreatmentType", SqlDbType.NVarChar);
        public TableField AssociatedControl = new TableField("AssociatedControl", SqlDbType.NVarChar);
        public TableField ProgressNotes = new TableField("ProgressNotes", SqlDbType.NVarChar);
        public TableField Verification = new TableField("Verification", SqlDbType.NVarChar);
        public TableField Effectiveness = new TableField("Effectiveness", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBRiskTreatmentPlanSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "RiskTreatmentPlanSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string ActionID = "", string RelatedRisk = "",
            string MitigationAction = "", string ActionOwner = "", string DueDate = "", string CompletionStatus = "",
            string TreatmentType = "", string AssociatedControl = "", string ProgressNotes = "", string Verification = "",
            string Effectiveness = "", string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.ActionID.SetValue(ActionID, ref FieldsArrayList);
            this.RelatedRisk.SetValue(RelatedRisk, ref FieldsArrayList);
            this.MitigationAction.SetValue(MitigationAction, ref FieldsArrayList);
            this.ActionOwner.SetValue(ActionOwner, ref FieldsArrayList);
            this.DueDate.SetValue(DueDate, ref FieldsArrayList);
            this.CompletionStatus.SetValue(CompletionStatus, ref FieldsArrayList);
            this.TreatmentType.SetValue(TreatmentType, ref FieldsArrayList);
            this.AssociatedControl.SetValue(AssociatedControl, ref FieldsArrayList);
            this.ProgressNotes.SetValue(ProgressNotes, ref FieldsArrayList);
            this.Verification.SetValue(Verification, ref FieldsArrayList);
            this.Effectiveness.SetValue(Effectiveness, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
