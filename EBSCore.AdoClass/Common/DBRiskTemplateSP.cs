using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBRiskTemplateSP : DBParentStoredProcedureClass
    {
        public DBRiskTemplateSP(IConfiguration configuration)
            : base("RiskTemplateSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? TemplateID = null,
            string TemplateNameEN = null,
            string TemplateNameAR = null,
            int? DefaultCategoryID = null,
            string DefaultImpact = null,
            string DefaultLikelihood = null,
            string DefaultRiskLevel = null,
            string GuidanceEN = null,
            string GuidanceAR = null,
            string StatusID = null,
            int? CreatedBy = null,
            int? UpdatedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                TemplateID,
                TemplateNameEN,
                TemplateNameAR,
                DefaultCategoryID,
                DefaultImpact,
                DefaultLikelihood,
                DefaultRiskLevel,
                GuidanceEN,
                GuidanceAR,
                StatusID,
                CreatedBy,
                UpdatedBy,
                SerializedObject
            });
        }
    }
}
