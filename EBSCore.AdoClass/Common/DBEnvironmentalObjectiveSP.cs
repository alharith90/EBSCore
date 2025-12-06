using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBEnvironmentalObjectiveSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField ObjectiveID = new TableField("ObjectiveID", SqlDbType.BigInt);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField ObjectiveDescription = new TableField("ObjectiveDescription", SqlDbType.NVarChar);
        public TableField TargetValue = new TableField("TargetValue", SqlDbType.NVarChar);
        public TableField Unit = new TableField("Unit", SqlDbType.NVarChar);
        public TableField BaselineValue = new TableField("BaselineValue", SqlDbType.NVarChar);
        public TableField CurrentValue = new TableField("CurrentValue", SqlDbType.NVarChar);
        public TableField TargetDate = new TableField("TargetDate", SqlDbType.DateTime);
        public TableField ResponsibleOwner = new TableField("ResponsibleOwner", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.BigInt);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBEnvironmentalObjectiveSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "EnvironmentalObjectiveSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, ObjectiveID, UnitID, ObjectiveDescription, TargetValue, Unit, BaselineValue, CurrentValue, TargetDate, ResponsibleOwner, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt };
        }
    }
}
