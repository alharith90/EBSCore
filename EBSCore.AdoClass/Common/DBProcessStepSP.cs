using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBProcessStepSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField ProcessID = new TableField("ProcessID", SqlDbType.Int);
        public TableField StepID = new TableField("StepID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField StepOrder = new TableField("StepOrder", SqlDbType.Int);
        public TableField RoleID = new TableField("RoleID", SqlDbType.BigInt);
        public TableField ExpectedOutput = new TableField("ExpectedOutput", SqlDbType.NVarChar);
        public TableField EscalationMinutes = new TableField("EscalationMinutes", SqlDbType.Int);
        public TableField ActivationCriteria = new TableField("ActivationCriteria", SqlDbType.NVarChar);
        public TableField StepsJson = new TableField("StepsJson", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBProcessStepSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "ProcessStepSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string ProcessID = "",
            string StepID = "",
            string Title = "",
            string Description = "",
            string StepOrder = "",
            string RoleID = "",
            string ExpectedOutput = "",
            string EscalationMinutes = "",
            string ActivationCriteria = "",
            string StepsJson = "",
            string CreatedBy = "",
            string UpdatedBy = "",
            string CreatedAt = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.ProcessID.SetValue(ProcessID, ref FieldsArrayList);
            this.StepID.SetValue(StepID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.StepOrder.SetValue(StepOrder, ref FieldsArrayList);
            this.RoleID.SetValue(RoleID, ref FieldsArrayList);
            this.ExpectedOutput.SetValue(ExpectedOutput, ref FieldsArrayList);
            this.EscalationMinutes.SetValue(EscalationMinutes, ref FieldsArrayList);
            this.ActivationCriteria.SetValue(ActivationCriteria, ref FieldsArrayList);
            this.StepsJson.SetValue(StepsJson, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
