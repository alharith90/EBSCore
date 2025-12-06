using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class HSRiskAssessment
    {
        public long? HazardID { get; set; }

        [Required]
        public string HazardDescription { get; set; }

        public string LocationArea { get; set; }

        public string RelatedActivity { get; set; }

        public string PotentialImpact { get; set; }

        public string Likelihood { get; set; }

        public string Severity { get; set; }

        public string RiskLevel { get; set; }

        public string ExistingControls { get; set; }

        public string AdditionalControlsNeeded { get; set; }

        public string RiskOwner { get; set; }

        public DateTime? NextReviewDate { get; set; }

        public string Status { get; set; }
    }
}
