using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SRisk
    {
        public int? RiskID { get; set; }
        public int? BCMRiskAssessmentId { get; set; }
        public int? ImpactAspectID { get; set; }
        public int? ImpactTimeFrameID { get; set; }
        public int ImpactID { get; set; }
        public int LikelihoodID { get; set; }
        public int? RiskCategoryID { get; set; }
        public string RiskTitle { get; set; } = string.Empty;
        public string? RiskDescription { get; set; }
        public string? MitigationPlan { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string NCEMAPillar { get; set; } = "Risk Assessment";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
