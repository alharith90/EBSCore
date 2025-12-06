using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBSustainabilityInitiativeSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField InitiativeID = new TableField("InitiativeID", SqlDbType.BigInt);
        public TableField InitiativeName = new TableField("InitiativeName", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField ESGCategory = new TableField("ESGCategory", SqlDbType.NVarChar);
        public TableField StartDate = new TableField("StartDate", SqlDbType.DateTime);
        public TableField EndDate = new TableField("EndDate", SqlDbType.DateTime);
        public TableField ResponsibleDepartment = new TableField("ResponsibleDepartment", SqlDbType.NVarChar);
        public TableField KeyMetrics = new TableField("KeyMetrics", SqlDbType.NVarChar);
        public TableField BudgetAllocated = new TableField("BudgetAllocated", SqlDbType.NVarChar);
        public TableField Outcome = new TableField("Outcome", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.BigInt);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBSustainabilityInitiativeSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "SustainabilityInitiativeSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, InitiativeID, InitiativeName, Description, ESGCategory, StartDate, EndDate, ResponsibleDepartment, KeyMetrics, BudgetAllocated, Outcome, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt };
        }
    }
}
