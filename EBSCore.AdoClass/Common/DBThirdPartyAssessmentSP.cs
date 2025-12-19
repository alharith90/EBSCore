using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyAssessmentSP : DBParentStoredProcedureClass
    {
        public DBThirdPartyAssessmentSP(IConfiguration configuration)
            : base("ThirdPartyAssessmentSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? AssessmentID = null,
            int? ThirdPartyID = null,
            string AssessmentType = null,
            string DateOfAssessment = null,
            string AreasAssessed = null,
            string QuestionnaireScore = null,
            string OverallRiskRating = null,
            string FindingsIssues = null,
            string RequiredMitigations = null,
            string ResidualRiskRating = null,
            bool? ApprovedForOnboarding = null,
            string NextAssessmentDue = null,
            string AssessedBy = null,
            string Notes = null,
            int? CreatedBy = null,
            int? UpdatedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                AssessmentID,
                ThirdPartyID,
                AssessmentType,
                DateOfAssessment,
                AreasAssessed,
                QuestionnaireScore,
                OverallRiskRating,
                FindingsIssues,
                RequiredMitigations,
                ResidualRiskRating,
                ApprovedForOnboarding,
                NextAssessmentDue,
                AssessedBy,
                Notes,
                CreatedBy,
                UpdatedBy,
                SerializedObject
            });
        }
    }
}
