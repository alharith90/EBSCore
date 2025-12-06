using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBHSIncidentSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField IncidentID = new TableField("IncidentID", SqlDbType.Int);
        public TableField IncidentTitle = new TableField("IncidentTitle", SqlDbType.NVarChar);
        public TableField IncidentDateTime = new TableField("IncidentDateTime", SqlDbType.DateTime);
        public TableField Location = new TableField("Location", SqlDbType.NVarChar);
        public TableField IncidentType = new TableField("IncidentType", SqlDbType.NVarChar);
        public TableField PersonsInvolved = new TableField("PersonsInvolved", SqlDbType.NVarChar);
        public TableField InjurySeverity = new TableField("InjurySeverity", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField ImmediateActions = new TableField("ImmediateActions", SqlDbType.NVarChar);
        public TableField RootCause = new TableField("RootCause", SqlDbType.NVarChar);
        public TableField RelatedHazard = new TableField("RelatedHazard", SqlDbType.NVarChar);
        public TableField RelatedActivity = new TableField("RelatedActivity", SqlDbType.NVarChar);
        public TableField RelatedRegulation = new TableField("RelatedRegulation", SqlDbType.NVarChar);
        public TableField CorrectiveActions = new TableField("CorrectiveActions", SqlDbType.NVarChar);
        public TableField IncidentStatus = new TableField("IncidentStatus", SqlDbType.NVarChar);
        public TableField ReportedBy = new TableField("ReportedBy", SqlDbType.NVarChar);
        public TableField Reportable = new TableField("Reportable", SqlDbType.Bit);
        public TableField DateClosed = new TableField("DateClosed", SqlDbType.DateTime);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBHSIncidentSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "HSIncidentSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string IncidentID = "",
            string IncidentTitle = "", string IncidentDateTime = "", string Location = "", string IncidentType = "",
            string PersonsInvolved = "", string InjurySeverity = "", string Description = "", string ImmediateActions = "",
            string RootCause = "", string RelatedHazard = "", string RelatedActivity = "", string RelatedRegulation = "",
            string CorrectiveActions = "", string IncidentStatus = "", string ReportedBy = "", string Reportable = "",
            string DateClosed = "", string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.IncidentID.SetValue(IncidentID, ref FieldsArrayList);
            this.IncidentTitle.SetValue(IncidentTitle, ref FieldsArrayList);
            this.IncidentDateTime.SetValue(IncidentDateTime, ref FieldsArrayList);
            this.Location.SetValue(Location, ref FieldsArrayList);
            this.IncidentType.SetValue(IncidentType, ref FieldsArrayList);
            this.PersonsInvolved.SetValue(PersonsInvolved, ref FieldsArrayList);
            this.InjurySeverity.SetValue(InjurySeverity, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.ImmediateActions.SetValue(ImmediateActions, ref FieldsArrayList);
            this.RootCause.SetValue(RootCause, ref FieldsArrayList);
            this.RelatedHazard.SetValue(RelatedHazard, ref FieldsArrayList);
            this.RelatedActivity.SetValue(RelatedActivity, ref FieldsArrayList);
            this.RelatedRegulation.SetValue(RelatedRegulation, ref FieldsArrayList);
            this.CorrectiveActions.SetValue(CorrectiveActions, ref FieldsArrayList);
            this.IncidentStatus.SetValue(IncidentStatus, ref FieldsArrayList);
            this.ReportedBy.SetValue(ReportedBy, ref FieldsArrayList);
            this.Reportable.SetValue(Reportable, ref FieldsArrayList);
            this.DateClosed.SetValue(DateClosed, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
