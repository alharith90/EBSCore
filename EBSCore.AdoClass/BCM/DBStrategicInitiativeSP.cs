using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBStrategicInitiativeSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField InitiativeID = new TableField("InitiativeID", SqlDbType.Int);
        public TableField ObjectiveID = new TableField("ObjectiveID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField OwnerID = new TableField("OwnerID", SqlDbType.BigInt);
        public TableField DepartmentID = new TableField("DepartmentID", SqlDbType.BigInt);
        public TableField Budget = new TableField("Budget", SqlDbType.Decimal);
        public TableField Progress = new TableField("Progress", SqlDbType.Int);
        public TableField StartDate = new TableField("StartDate", SqlDbType.DateTime);
        public TableField EndDate = new TableField("EndDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField EscalationCriteria = new TableField("EscalationCriteria", SqlDbType.NVarChar);
        public TableField EscalationContact = new TableField("EscalationContact", SqlDbType.NVarChar);
        public TableField ActivationCriteria = new TableField("ActivationCriteria", SqlDbType.NVarChar);
        public TableField ActivationTrigger = new TableField("ActivationTrigger", SqlDbType.NVarChar);
        public TableField ActivationStatus = new TableField("ActivationStatus", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBStrategicInitiativeSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "StrategicInitiativeSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string InitiativeID = "",
            string ObjectiveID = "",
            string Title = "",
            string Description = "",
            string OwnerID = "",
            string DepartmentID = "",
            string Budget = "",
            string Progress = "",
            string StartDate = "",
            string EndDate = "",
            string Status = "",
            string EscalationCriteria = "",
            string EscalationContact = "",
            string ActivationCriteria = "",
            string ActivationTrigger = "",
            string ActivationStatus = "",
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
            this.InitiativeID.SetValue(InitiativeID, ref FieldsArrayList);
            this.ObjectiveID.SetValue(ObjectiveID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.OwnerID.SetValue(OwnerID, ref FieldsArrayList);
            this.DepartmentID.SetValue(DepartmentID, ref FieldsArrayList);
            this.Budget.SetValue(Budget, ref FieldsArrayList);
            this.Progress.SetValue(Progress, ref FieldsArrayList);
            this.StartDate.SetValue(StartDate, ref FieldsArrayList);
            this.EndDate.SetValue(EndDate, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.EscalationCriteria.SetValue(EscalationCriteria, ref FieldsArrayList);
            this.EscalationContact.SetValue(EscalationContact, ref FieldsArrayList);
            this.ActivationCriteria.SetValue(ActivationCriteria, ref FieldsArrayList);
            this.ActivationTrigger.SetValue(ActivationTrigger, ref FieldsArrayList);
            this.ActivationStatus.SetValue(ActivationStatus, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
