using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyAssessmentSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField AssessmentID = new TableField("AssessmentID", SqlDbType.Int);
        public TableField ThirdPartyID = new TableField("ThirdPartyID", SqlDbType.Int);
        public TableField AssessmentType = new TableField("AssessmentType", SqlDbType.NVarChar);
        public TableField DateOfAssessment = new TableField("DateOfAssessment", SqlDbType.DateTime);
        public TableField AreasAssessed = new TableField("AreasAssessed", SqlDbType.NVarChar);
        public TableField QuestionnaireScore = new TableField("QuestionnaireScore", SqlDbType.NVarChar);
        public TableField OverallRiskRating = new TableField("OverallRiskRating", SqlDbType.NVarChar);
        public TableField FindingsIssues = new TableField("FindingsIssues", SqlDbType.NVarChar);
        public TableField RequiredMitigations = new TableField("RequiredMitigations", SqlDbType.NVarChar);
        public TableField ResidualRiskRating = new TableField("ResidualRiskRating", SqlDbType.NVarChar);
        public TableField ApprovedForOnboarding = new TableField("ApprovedForOnboarding", SqlDbType.Bit);
        public TableField NextAssessmentDue = new TableField("NextAssessmentDue", SqlDbType.DateTime);
        public TableField AssessedBy = new TableField("AssessedBy", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

        public DBThirdPartyAssessmentSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "ThirdPartyAssessmentSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, AssessmentID, ThirdPartyID, AssessmentType, DateOfAssessment, AreasAssessed, QuestionnaireScore, OverallRiskRating, FindingsIssues, RequiredMitigations, ResidualRiskRating, ApprovedForOnboarding, NextAssessmentDue, AssessedBy, Notes, CreatedBy, UpdatedBy };
        }
    }
}
