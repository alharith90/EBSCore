using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBComplianceObligationSP : DBParentStoredProcedureClass
    {
        public DBComplianceObligationSP(IConfiguration configuration)
            : base("ComplianceObligationSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? ObligationID = null,
            string ObligationTitle = null,
            string ObligationDescription = null,
            string Source = null,
            string ClauseReference = null,
            string ResponsibleDepartment = null,
            string ObligationOwner = null,
            string ComplianceStatus = null,
            string LastAssessmentDate = null,
            string NextReviewDate = null,
            string RelatedControls = null,
            string RelatedRisks = null,
            string RelatedPolicy = null,
            string EvidenceOfCompliance = null,
            string Comments = null,
            long? CreatedBy = null,
            long? ModifiedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ObligationID,
                ObligationTitle,
                ObligationDescription,
                Source,
                ClauseReference,
                ResponsibleDepartment,
                ObligationOwner,
                ComplianceStatus,
                LastAssessmentDate,
                NextReviewDate,
                RelatedControls,
                RelatedRisks,
                RelatedPolicy,
                EvidenceOfCompliance,
                Comments,
                CreatedBy,
                ModifiedBy,
                SerializedObject
            });
        }
    }
}
