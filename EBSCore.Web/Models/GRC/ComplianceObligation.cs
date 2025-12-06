using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class ComplianceObligation
    {
        public long? ObligationID { get; set; }

        [Required]
        public string ObligationTitle { get; set; }

        [Required]
        public string ObligationDescription { get; set; }

        public string Source { get; set; }

        public string ClauseReference { get; set; }

        public string ResponsibleDepartment { get; set; }

        public string ObligationOwner { get; set; }

        public string ComplianceStatus { get; set; }

        public DateTime? LastAssessmentDate { get; set; }

        public DateTime? NextReviewDate { get; set; }

        public string RelatedControls { get; set; }

        public string RelatedRisks { get; set; }

        public string RelatedPolicy { get; set; }

        public string EvidenceOfCompliance { get; set; }

        public string Comments { get; set; }
    }
}
