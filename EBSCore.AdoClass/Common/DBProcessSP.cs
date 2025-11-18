using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBProcessSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField ProcessID = new TableField("ProcessID", SqlDbType.Int);
        public TableField ProcessName = new TableField("ProcessName", SqlDbType.NVarChar);
        public TableField ProcessCode = new TableField("ProcessCode", SqlDbType.NVarChar);
        public TableField ProcessDescription = new TableField("ProcessDescription", SqlDbType.NVarChar);
        public TableField ExpiryDate = new TableField("ExpiryDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField Priority = new TableField("Priority", SqlDbType.NVarChar);
        public TableField Frequency = new TableField("Frequency", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBProcessSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "ProcessSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string UnitID = "",
            string ProcessID = "", string ProcessName = "", string ProcessCode = "", string ProcessDescription = "",
            string ExpiryDate = "", string Status = "", string Priority = "", string Frequency = "",
            string Notes = "", string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID , ref FieldsArrayList);
            this.UnitID.SetValue(UnitID , ref FieldsArrayList);
            this.ProcessID.SetValue(ProcessID, ref FieldsArrayList);
            this.ProcessName.SetValue(ProcessName, ref FieldsArrayList);
            this.ProcessCode.SetValue(ProcessCode, ref FieldsArrayList);
            this.ProcessDescription.SetValue(ProcessDescription, ref FieldsArrayList);
            this.ExpiryDate.SetValue(ExpiryDate, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.Priority.SetValue(Priority, ref FieldsArrayList);
            this.Frequency.SetValue(Frequency, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);
            this.CreatedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt , ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
