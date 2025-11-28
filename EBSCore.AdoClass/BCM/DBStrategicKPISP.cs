using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBStrategicKPISP : DBParentStoredProcedureClass
    {
        public TableField Operation = new("Operation", SqlDbType.NVarChar);
        public TableField UserID = new("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new("CompanyID", SqlDbType.Int);
        public TableField UnitID = new("UnitID", SqlDbType.BigInt);
        public TableField KPIID = new("KPIID", SqlDbType.Int);
        public TableField Category = new("Category", SqlDbType.NVarChar);
        public TableField Type = new("Type", SqlDbType.NVarChar);
        public TableField ObjectiveID = new("ObjectiveID", SqlDbType.NVarChar);
        public TableField Title = new("Title", SqlDbType.NVarChar);
        public TableField Description = new("Description", SqlDbType.NVarChar);
        public TableField TargetValue = new("TargetValue", SqlDbType.Decimal);
        public TableField ThresholdGreen = new("ThresholdGreen", SqlDbType.Decimal);
        public TableField ThresholdRed = new("ThresholdRed", SqlDbType.Decimal);
        public TableField ThresholdMethod = new("ThresholdMethod", SqlDbType.NVarChar);
        public TableField Unit = new("Unit", SqlDbType.NVarChar);
        public TableField Frequency = new("Frequency", SqlDbType.NVarChar);
        public TableField CalculationMethod = new("CalculationMethod", SqlDbType.NVarChar);
        public TableField Status = new("Status", SqlDbType.NVarChar);
        public TableField Owner = new("Owner", SqlDbType.NVarChar);
        public TableField EscalationPlan = new("EscalationPlan", SqlDbType.NVarChar);
        public TableField ActivationCriteria = new("ActivationCriteria", SqlDbType.NVarChar);
        public TableField CreatedBy = new("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new("UpdatedAt", SqlDbType.DateTime);
        public TableField ResultID = new("ResultID", SqlDbType.Int);
        public TableField Period = new("Period", SqlDbType.NVarChar);
        public TableField Value = new("Value", SqlDbType.Decimal);
        public TableField Comments = new("Comments", SqlDbType.NVarChar);
        public TableField EvidenceFile = new("EvidenceFile", SqlDbType.NVarChar);
        public TableField LinkID = new("LinkID", SqlDbType.Int);
        public TableField LinkedType = new("LinkedType", SqlDbType.NVarChar);
        public TableField LinkedID = new("LinkedID", SqlDbType.NVarChar);

        public DBStrategicKPISP(IConfiguration configuration) : base(configuration)
        {
            SPName = "StrategicKPISP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string KPIID = "",
            string Category = "",
            string Type = "",
            string ObjectiveID = "",
            string Title = "",
            string Description = "",
            string TargetValue = "",
            string ThresholdGreen = "",
            string ThresholdRed = "",
            string ThresholdMethod = "",
            string Unit = "",
            string Frequency = "",
            string CalculationMethod = "",
            string Status = "",
            string Owner = "",
            string EscalationPlan = "",
            string ActivationCriteria = "",
            string CreatedBy = "",
            string ModifiedBy = "",
            string CreatedAt = "",
            string UpdatedAt = "",
            string ResultID = "",
            string Period = "",
            string Value = "",
            string Comments = "",
            string EvidenceFile = "",
            string LinkID = "",
            string LinkedType = "",
            string LinkedID = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.KPIID.SetValue(KPIID, ref FieldsArrayList);
            this.Category.SetValue(Category, ref FieldsArrayList);
            this.Type.SetValue(Type, ref FieldsArrayList);
            this.ObjectiveID.SetValue(ObjectiveID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.TargetValue.SetValue(TargetValue, ref FieldsArrayList);
            this.ThresholdGreen.SetValue(ThresholdGreen, ref FieldsArrayList);
            this.ThresholdRed.SetValue(ThresholdRed, ref FieldsArrayList);
            this.ThresholdMethod.SetValue(ThresholdMethod, ref FieldsArrayList);
            this.Unit.SetValue(Unit, ref FieldsArrayList);
            this.Frequency.SetValue(Frequency, ref FieldsArrayList);
            this.CalculationMethod.SetValue(CalculationMethod, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.Owner.SetValue(Owner, ref FieldsArrayList);
            this.EscalationPlan.SetValue(EscalationPlan, ref FieldsArrayList);
            this.ActivationCriteria.SetValue(ActivationCriteria, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);
            this.ResultID.SetValue(ResultID, ref FieldsArrayList);
            this.Period.SetValue(Period, ref FieldsArrayList);
            this.Value.SetValue(Value, ref FieldsArrayList);
            this.Comments.SetValue(Comments, ref FieldsArrayList);
            this.EvidenceFile.SetValue(EvidenceFile, ref FieldsArrayList);
            this.LinkID.SetValue(LinkID, ref FieldsArrayList);
            this.LinkedType.SetValue(LinkedType, ref FieldsArrayList);
            this.LinkedID.SetValue(LinkedID, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
