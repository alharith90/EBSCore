using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBIncidentSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField IncidentID = new TableField("IncidentID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField IncidentDate = new TableField("IncidentDate", SqlDbType.DateTime);
        public TableField ReportedBy = new TableField("ReportedBy", SqlDbType.NVarChar);
        public TableField AffectedAssets = new TableField("AffectedAssets", SqlDbType.NVarChar);
        public TableField RelatedRiskIDs = new TableField("RelatedRiskIDs", SqlDbType.NVarChar);
        public TableField ImpactedActivities = new TableField("ImpactedActivities", SqlDbType.NVarChar);
        public TableField EscalationLevel = new TableField("EscalationLevel", SqlDbType.NVarChar);
        public TableField EscalationNotes = new TableField("EscalationNotes", SqlDbType.NVarChar);
        public TableField EscalatedToBC = new TableField("EscalatedToBC", SqlDbType.Bit);
        public TableField BCPActivated = new TableField("BCPActivated", SqlDbType.Bit);
        public TableField ActivationReason = new TableField("ActivationReason", SqlDbType.NVarChar);
        public TableField ActivationTime = new TableField("ActivationTime", SqlDbType.DateTime);
        public TableField RecoveryStartTime = new TableField("RecoveryStartTime", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBIncidentSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "IncidentSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string UnitID = "", string IncidentID = "",
            string Title = "", string Description = "", string IncidentDate = "", string ReportedBy = "",
            string AffectedAssets = "", string RelatedRiskIDs = "", string ImpactedActivities = "",
            string EscalationLevel = "", string EscalationNotes = "", string EscalatedToBC = "",
            string BCPActivated = "", string ActivationReason = "", string ActivationTime = "", string RecoveryStartTime = "",
            string Status = "", string Notes = "", string CreatedBy = "", string UpdatedBy = "", string CreatedAt = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.IncidentID.SetValue(IncidentID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.IncidentDate.SetValue(IncidentDate, ref FieldsArrayList);
            this.ReportedBy.SetValue(ReportedBy, ref FieldsArrayList);
            this.AffectedAssets.SetValue(AffectedAssets, ref FieldsArrayList);
            this.RelatedRiskIDs.SetValue(RelatedRiskIDs, ref FieldsArrayList);
            this.ImpactedActivities.SetValue(ImpactedActivities, ref FieldsArrayList);
            this.EscalationLevel.SetValue(EscalationLevel, ref FieldsArrayList);
            this.EscalationNotes.SetValue(EscalationNotes, ref FieldsArrayList);
            this.EscalatedToBC.SetValue(EscalatedToBC, ref FieldsArrayList);
            this.BCPActivated.SetValue(BCPActivated, ref FieldsArrayList);
            this.ActivationReason.SetValue(ActivationReason, ref FieldsArrayList);
            this.ActivationTime.SetValue(ActivationTime, ref FieldsArrayList);
            this.RecoveryStartTime.SetValue(RecoveryStartTime, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
