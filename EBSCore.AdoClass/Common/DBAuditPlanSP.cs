using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBAuditPlanSP : DBParentStoredProcedureClass
    {
        public DBAuditPlanSP(IConfiguration configuration)
            : base("AuditPlanSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? AuditID = null,
            string AuditTitle = null,
            string AuditType = null,
            string ObjectivesScope = null,
            string Auditee = null,
            string LeadAuditor = null,
            string AuditTeam = null,
            string StartDate = null,
            string EndDate = null,
            string AuditCriteria = null,
            int? FindingsCount = null,
            string OverallResult = null,
            string ReportIssuedDate = null,
            string RelatedRisksReviewed = null,
            string RelatedControlsReviewed = null,
            string RelatedObligationsReviewed = null,
            string NextAuditDate = null,
            long? CreatedBy = null,
            long? ModifiedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                AuditID,
                AuditTitle,
                AuditType,
                ObjectivesScope,
                Auditee,
                LeadAuditor,
                AuditTeam,
                StartDate,
                EndDate,
                AuditCriteria,
                FindingsCount,
                OverallResult,
                ReportIssuedDate,
                RelatedRisksReviewed,
                RelatedControlsReviewed,
                RelatedObligationsReviewed,
                NextAuditDate,
                CreatedBy,
                ModifiedBy,
                SerializedObject
            });
        }
    }
}
