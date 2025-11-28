using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBProcessControlSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField ProcessID = new TableField("ProcessID", SqlDbType.Int);
        public TableField StepID = new TableField("StepID", SqlDbType.Int);
        public TableField ProcessControlID = new TableField("ProcessControlID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField Type = new TableField("Type", SqlDbType.NVarChar);
        public TableField EvidenceRequired = new TableField("EvidenceRequired", SqlDbType.Bit);
        public TableField RelatedStandards = new TableField("RelatedStandards", SqlDbType.NVarChar);
        public TableField ControlsJson = new TableField("ControlsJson", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBProcessControlSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "ProcessControlSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string ProcessID = "",
            string StepID = "",
            string ProcessControlID = "",
            string Title = "",
            string Description = "",
            string Type = "",
            string EvidenceRequired = "",
            string RelatedStandards = "",
            string ControlsJson = "",
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
            this.ProcessControlID.SetValue(ProcessControlID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.Type.SetValue(Type, ref FieldsArrayList);
            this.EvidenceRequired.SetValue(EvidenceRequired, ref FieldsArrayList);
            this.RelatedStandards.SetValue(RelatedStandards, ref FieldsArrayList);
            this.ControlsJson.SetValue(ControlsJson, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
