using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBStrategySP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField StrategyID = new TableField("StrategyID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField TitleAr = new TableField("TitleAr", SqlDbType.NVarChar);
        public TableField Vision = new TableField("Vision", SqlDbType.NVarChar);
        public TableField Mission = new TableField("Mission", SqlDbType.NVarChar);
        public TableField EscalationCriteria = new TableField("EscalationCriteria", SqlDbType.NVarChar);
        public TableField ActivationCriteria = new TableField("ActivationCriteria", SqlDbType.NVarChar);
        public TableField StartDate = new TableField("StartDate", SqlDbType.DateTime);
        public TableField EndDate = new TableField("EndDate", SqlDbType.DateTime);
        public TableField OwnerID = new TableField("OwnerID", SqlDbType.Int);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBStrategySP(IConfiguration configuration) : base(configuration)
        {
            SPName = "BCMStrategySP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string StrategyID = "",
            string Title = "",
            string TitleAr = "",
            string Vision = "",
            string Mission = "",
            string EscalationCriteria = "",
            string ActivationCriteria = "",
            string StartDate = "",
            string EndDate = "",
            string OwnerID = "",
            string Status = "",
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
            this.StrategyID.SetValue(StrategyID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.TitleAr.SetValue(TitleAr, ref FieldsArrayList);
            this.Vision.SetValue(Vision, ref FieldsArrayList);
            this.Mission.SetValue(Mission, ref FieldsArrayList);
            this.EscalationCriteria.SetValue(EscalationCriteria, ref FieldsArrayList);
            this.ActivationCriteria.SetValue(ActivationCriteria, ref FieldsArrayList);
            this.StartDate.SetValue(StartDate, ref FieldsArrayList);
            this.EndDate.SetValue(EndDate, ref FieldsArrayList);
            this.OwnerID.SetValue(OwnerID, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
