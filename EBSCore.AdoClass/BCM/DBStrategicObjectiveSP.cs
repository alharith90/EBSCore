using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBStrategicObjectiveSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField ObjectiveID = new TableField("ObjectiveID", SqlDbType.Int);
        public TableField ObjectiveCode = new TableField("ObjectiveCode", SqlDbType.NVarChar);
        public TableField Strategy = new TableField("Strategy", SqlDbType.NVarChar);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField OwnerID = new TableField("OwnerID", SqlDbType.BigInt);
        public TableField RiskLink = new TableField("RiskLink", SqlDbType.NVarChar);
        public TableField ComplianceLink = new TableField("ComplianceLink", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField StartDate = new TableField("StartDate", SqlDbType.DateTime);
        public TableField EndDate = new TableField("EndDate", SqlDbType.DateTime);
        public TableField EscalationLevel = new TableField("EscalationLevel", SqlDbType.NVarChar);
        public TableField EscalationContact = new TableField("EscalationContact", SqlDbType.NVarChar);
        public TableField ActivationCriteria = new TableField("ActivationCriteria", SqlDbType.NVarChar);
        public TableField ActivationStatus = new TableField("ActivationStatus", SqlDbType.NVarChar);
        public TableField ActivationTrigger = new TableField("ActivationTrigger", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBStrategicObjectiveSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "StrategicObjectiveSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string ObjectiveID = "",
            string ObjectiveCode = "",
            string Strategy = "",
            string Title = "",
            string Description = "",
            string OwnerID = "",
            string RiskLink = "",
            string ComplianceLink = "",
            string Status = "",
            string StartDate = "",
            string EndDate = "",
            string EscalationLevel = "",
            string EscalationContact = "",
            string ActivationCriteria = "",
            string ActivationStatus = "",
            string ActivationTrigger = "",
            string CreatedBy = "",
            string ModifiedBy = "",
            string CreatedAt = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.ObjectiveID.SetValue(ObjectiveID, ref FieldsArrayList);
            this.ObjectiveCode.SetValue(ObjectiveCode, ref FieldsArrayList);
            this.Strategy.SetValue(Strategy, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.OwnerID.SetValue(OwnerID, ref FieldsArrayList);
            this.RiskLink.SetValue(RiskLink, ref FieldsArrayList);
            this.ComplianceLink.SetValue(ComplianceLink, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.StartDate.SetValue(StartDate, ref FieldsArrayList);
            this.EndDate.SetValue(EndDate, ref FieldsArrayList);
            this.EscalationLevel.SetValue(EscalationLevel, ref FieldsArrayList);
            this.EscalationContact.SetValue(EscalationContact, ref FieldsArrayList);
            this.ActivationCriteria.SetValue(ActivationCriteria, ref FieldsArrayList);
            this.ActivationStatus.SetValue(ActivationStatus, ref FieldsArrayList);
            this.ActivationTrigger.SetValue(ActivationTrigger, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
