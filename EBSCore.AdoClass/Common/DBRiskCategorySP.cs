using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBRiskCategorySP : DBParentStoredProcedureClass
    {
        public DBRiskCategorySP(IConfiguration configuration)
            : base("RiskCategorySP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? CategoryID = null,
            string CategoryNameEN = null,
            string CategoryNameAR = null,
            string DescriptionEN = null,
            string DescriptionAR = null,
            int? ParentCategoryID = null,
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
                CategoryID,
                CategoryNameEN,
                CategoryNameAR,
                DescriptionEN,
                DescriptionAR,
                ParentCategoryID,
                StatusID,
                CreatedBy,
                UpdatedBy,
                SerializedObject
            });
        }
    }
}
