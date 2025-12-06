using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRiskCategorySP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField CategoryID = new TableField("CategoryID", SqlDbType.Int);
        public TableField CategoryNameEN = new TableField("CategoryNameEN", SqlDbType.NVarChar);
        public TableField CategoryNameAR = new TableField("CategoryNameAR", SqlDbType.NVarChar);
        public TableField DescriptionEN = new TableField("DescriptionEN", SqlDbType.NVarChar);
        public TableField DescriptionAR = new TableField("DescriptionAR", SqlDbType.NVarChar);
        public TableField ParentCategoryID = new TableField("ParentCategoryID", SqlDbType.Int);
        public TableField StatusID = new TableField("StatusID", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

        public DBRiskCategorySP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "RiskCategorySP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, CategoryID, CategoryNameEN, CategoryNameAR, DescriptionEN, DescriptionAR, ParentCategoryID, StatusID, CreatedBy, UpdatedBy };
        }
    }
}
