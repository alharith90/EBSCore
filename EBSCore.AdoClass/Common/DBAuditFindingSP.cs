using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBAuditFindingSP : DBParentStoredProcedureClass
    {
        public DBAuditFindingSP(IConfiguration configuration)
            : base("AuditFindingSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? FindingID = null,
            long? AuditID = null,
            string FindingTitle = null,
            string Criteria = null,
            string Condition = null,
            string Cause = null,
            string Effect = null,
            string Recommendation = null,
            string Category = null,
            string Severity = null,
            string RelatedRiskControl = null,
            string ResponsibleOwner = null,
            string ActionPlanLink = null,
            string DueDate = null,
            string Status = null,
            string DateClosed = null,
            string VerifiedBy = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                FindingID,
                AuditID,
                FindingTitle,
                Criteria,
                Condition,
                Cause,
                Effect,
                Recommendation,
                Category,
                Severity,
                RelatedRiskControl,
                ResponsibleOwner,
                ActionPlanLink,
                DueDate,
                Status,
                DateClosed,
                VerifiedBy,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
