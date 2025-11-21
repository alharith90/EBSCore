using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBS7SPlan_SP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField PlanID = new TableField("PlanID", SqlDbType.Int);
        public TableField PlanCode = new TableField("PlanCode", SqlDbType.NVarChar);
        public TableField PlanName = new TableField("PlanName", SqlDbType.NVarChar);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.DateTimeOffset);
        public TableField FrequencyMonths = new TableField("FrequencyMonths", SqlDbType.Int);
        public TableField ExerciseTypeID = new TableField("ExerciseTypeID", SqlDbType.Int);
        public TableField PostIncidentSummary = new TableField("PostIncidentSummary", SqlDbType.NVarChar);
        public TableField MandatoryControls = new TableField("MandatoryControls", SqlDbType.NVarChar);
        public TableField AttachmentID = new TableField("AttachmentID", SqlDbType.BigInt);

        public DBS7SPlan_SP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SPlan_SP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string PlanID = "",
            string PlanCode = "",
            string PlanName = "",
            string NextReviewDate = "",
            string FrequencyMonths = "",
            string ExerciseTypeID = "",
            string PostIncidentSummary = "",
            string MandatoryControls = "",
            string AttachmentID = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.PlanID.SetValue(PlanID, ref FieldsArrayList);
            this.PlanCode.SetValue(PlanCode, ref FieldsArrayList);
            this.PlanName.SetValue(PlanName, ref FieldsArrayList);
            this.NextReviewDate.SetValue(NextReviewDate, ref FieldsArrayList);
            this.FrequencyMonths.SetValue(FrequencyMonths, ref FieldsArrayList);
            this.ExerciseTypeID.SetValue(ExerciseTypeID, ref FieldsArrayList);
            this.PostIncidentSummary.SetValue(PostIncidentSummary, ref FieldsArrayList);
            this.MandatoryControls.SetValue(MandatoryControls, ref FieldsArrayList);
            this.AttachmentID.SetValue(AttachmentID, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
