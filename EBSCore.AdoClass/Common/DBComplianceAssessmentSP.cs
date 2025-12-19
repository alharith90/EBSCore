using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBComplianceAssessmentSP : DBParentStoredProcedureClass
    {
        public DBComplianceAssessmentSP(IConfiguration configuration)
            : base("ComplianceAssessmentSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? AssessmentID = null,
            string ScopeCriteria = null,
            string Assessor = null,
            string AssessmentDate = null,
            string ComplianceScore = null,
            string FindingsCount = null,
            string Findings = null,
            string ActionPlanReference = null,
            string NextAssessmentDue = null,
            string Status = null,
            string RelatedObligations = null,
            string RelatedControls = null,
            long? CreatedBy = null,
            long? ModifiedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                AssessmentID,
                ScopeCriteria,
                Assessor,
                AssessmentDate,
                ComplianceScore,
                FindingsCount,
                Findings,
                ActionPlanReference,
                NextAssessmentDue,
                Status,
                RelatedObligations,
                RelatedControls,
                CreatedBy,
                ModifiedBy,
                SerializedObject
            });
        }
    }
}
