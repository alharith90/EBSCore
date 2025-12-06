using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class RiskRegister
    {
        public string? CompanyID { get; set; }

        public string? RiskID { get; set; }

        [Required]
        public string RiskTitle { get; set; } = string.Empty;

        [Required]
        public string RiskDescription { get; set; } = string.Empty;

        public string? RiskCategory { get; set; }

        public string? RiskSource { get; set; }

        public string? PotentialImpact { get; set; }

        public string? InherentLikelihood { get; set; }

        public string? InherentImpact { get; set; }

        public string? InherentRiskLevel { get; set; }

        public string? ExistingControls { get; set; }

        public string? ControlEffectiveness { get; set; }

        public string? ResidualLikelihood { get; set; }

        public string? ResidualImpact { get; set; }

        public string? ResidualRiskLevel { get; set; }

        public string? RiskAppetiteThreshold { get; set; }

        public string? RiskResponseStrategy { get; set; }

        public string? TreatmentDecision { get; set; }

        public string? TreatmentPlanID { get; set; }

        public string? Likelihood { get; set; }

        public string? Impact { get; set; }

        public int? RiskScore { get; set; }

        public string? RiskResponse { get; set; }

        public string? RiskOwner { get; set; }

        public string? Status { get; set; }

        public string? ReviewDate { get; set; }

        public string? NextReviewDate { get; set; }

        public string? RiskTrend { get; set; }

        public string? RelatedObjectives { get; set; }

        public string? RelatedIncidents { get; set; }

        public string? RelatedControls { get; set; }

        public string? RelatedObligations { get; set; }

        public string? MonitoringFrequency { get; set; }

        public string? LastMonitoringDate { get; set; }

        public string? KRIName { get; set; }

        public string? KRIValue { get; set; }

        public string? KRIStatus { get; set; }

        public string? RiskAlertTrigger { get; set; }

        public string? NextMonitoringDate { get; set; }

        public string? RiskHistoryNotes { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? LastUpdatedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
