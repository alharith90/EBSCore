using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRecoveryStrategySP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField RecoveryStrategyID = new TableField("RecoveryStrategyID", SqlDbType.Int);
        public TableField FailureScenario = new TableField("FailureScenario", SqlDbType.NVarChar);
        public TableField Strategy = new TableField("Strategy", SqlDbType.NVarChar);
        public TableField CostImpact = new TableField("CostImpact", SqlDbType.Decimal);
        public TableField ConfidenceLevel = new TableField("ConfidenceLevel", SqlDbType.NVarChar);
        public TableField StepsJson = new TableField("StepsJson", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBRecoveryStrategySP(IConfiguration configuration) : base(configuration)
        {
            SPName = "RecoveryStrategySP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string RecoveryStrategyID = "",
            string FailureScenario = "",
            string Strategy = "",
            string CostImpact = "",
            string ConfidenceLevel = "",
            string StepsJson = "",
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
            this.RecoveryStrategyID.SetValue(RecoveryStrategyID, ref FieldsArrayList);
            this.FailureScenario.SetValue(FailureScenario, ref FieldsArrayList);
            this.Strategy.SetValue(Strategy, ref FieldsArrayList);
            this.CostImpact.SetValue(CostImpact, ref FieldsArrayList);
            this.ConfidenceLevel.SetValue(ConfidenceLevel, ref FieldsArrayList);
            this.StepsJson.SetValue(StepsJson, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
