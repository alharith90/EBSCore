using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRiskTemplateSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField TemplateID = new TableField("TemplateID", SqlDbType.Int);
        public TableField TemplateNameEN = new TableField("TemplateNameEN", SqlDbType.NVarChar);
        public TableField TemplateNameAR = new TableField("TemplateNameAR", SqlDbType.NVarChar);
        public TableField DefaultCategoryID = new TableField("DefaultCategoryID", SqlDbType.Int);
        public TableField DefaultImpact = new TableField("DefaultImpact", SqlDbType.NVarChar);
        public TableField DefaultLikelihood = new TableField("DefaultLikelihood", SqlDbType.NVarChar);
        public TableField DefaultRiskLevel = new TableField("DefaultRiskLevel", SqlDbType.NVarChar);
        public TableField GuidanceEN = new TableField("GuidanceEN", SqlDbType.NVarChar);
        public TableField GuidanceAR = new TableField("GuidanceAR", SqlDbType.NVarChar);
        public TableField StatusID = new TableField("StatusID", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

        public DBRiskTemplateSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "RiskTemplateSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, TemplateID, TemplateNameEN, TemplateNameAR, DefaultCategoryID, DefaultImpact, DefaultLikelihood, DefaultRiskLevel, GuidanceEN, GuidanceAR, StatusID, CreatedBy, UpdatedBy };
        }
    }
}
