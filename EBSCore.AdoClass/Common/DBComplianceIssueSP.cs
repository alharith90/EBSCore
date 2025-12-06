using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBComplianceIssueSP : DBParentStoredProcedureClass
    {
        public DBComplianceIssueSP(IConfiguration configuration)
            : base("ComplianceIssueSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? IssueID = null,
            string IssueDescription = null,
            string DateDetected = null,
            string Source = null,
            string RelatedObligation = null,
            string Severity = null,
            string Impact = null,
            string RootCause = null,
            string CorrectiveActionPlan = null,
            string TargetCompletionDate = null,
            string ActionStatus = null,
            string RegulatoryReportingDate = null,
            string Status = null,
            string LessonsLearned = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                IssueID,
                IssueDescription,
                DateDetected,
                Source,
                RelatedObligation,
                Severity,
                Impact,
                RootCause,
                CorrectiveActionPlan,
                TargetCompletionDate,
                ActionStatus,
                RegulatoryReportingDate,
                Status,
                LessonsLearned,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
