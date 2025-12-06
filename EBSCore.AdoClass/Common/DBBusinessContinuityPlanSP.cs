using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBBusinessContinuityPlanSP : DBParentStoredProcedureClass
    {
        public DBBusinessContinuityPlanSP(IConfiguration configuration)
            : base("BusinessContinuityPlanSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? PlanID = null,
            string PlanName = null,
            string Scope = null,
            string Owner = null,
            string LastTestDate = null,
            string NextReviewDate = null,
            string Status = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                PlanID,
                PlanName,
                Scope,
                Owner,
                LastTestDate,
                NextReviewDate,
                Status,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
