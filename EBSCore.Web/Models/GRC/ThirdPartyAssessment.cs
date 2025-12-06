using System;

namespace EBSCore.Web.Models.GRC
{
    public class ThirdPartyAssessment
    {
        public int? AssessmentID { get; set; }
        public int? CompanyID { get; set; }
        public int? ThirdPartyID { get; set; }
        public string AssessmentType { get; set; }
        public DateTime? DateOfAssessment { get; set; }
        public string AreasAssessed { get; set; }
        public string QuestionnaireScore { get; set; }
        public string OverallRiskRating { get; set; }
        public string FindingsIssues { get; set; }
        public string RequiredMitigations { get; set; }
        public string ResidualRiskRating { get; set; }
        public bool? ApprovedForOnboarding { get; set; }
        public DateTime? NextAssessmentDue { get; set; }
        public string AssessedBy { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
