using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBIncidentRegisterSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField IncidentID = new TableField("IncidentID", SqlDbType.Int);
        public TableField IncidentDescription = new TableField("IncidentDescription", SqlDbType.NVarChar);
        public TableField IncidentDate = new TableField("IncidentDate", SqlDbType.DateTime);
        public TableField ImpactedArea = new TableField("ImpactedArea", SqlDbType.NVarChar);
        public TableField Severity = new TableField("Severity", SqlDbType.NVarChar);
        public TableField RootCause = new TableField("RootCause", SqlDbType.NVarChar);
        public TableField ActionsTaken = new TableField("ActionsTaken", SqlDbType.NVarChar);
        public TableField IncidentOwner = new TableField("IncidentOwner", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBIncidentRegisterSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "IncidentRegisterSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string IncidentID = "", string IncidentDescription = "",
            string IncidentDate = "", string ImpactedArea = "", string Severity = "", string RootCause = "",
            string ActionsTaken = "", string IncidentOwner = "", string Status = "", string CreatedBy = "", string ModifiedBy = "",
            string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.IncidentID.SetValue(IncidentID, ref FieldsArrayList);
            this.IncidentDescription.SetValue(IncidentDescription, ref FieldsArrayList);
            this.IncidentDate.SetValue(IncidentDate, ref FieldsArrayList);
            this.ImpactedArea.SetValue(ImpactedArea, ref FieldsArrayList);
            this.Severity.SetValue(Severity, ref FieldsArrayList);
            this.RootCause.SetValue(RootCause, ref FieldsArrayList);
            this.ActionsTaken.SetValue(ActionsTaken, ref FieldsArrayList);
            this.IncidentOwner.SetValue(IncidentOwner, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
