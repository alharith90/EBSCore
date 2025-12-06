using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class ComplianceAssessment
    {
        public long? AssessmentID { get; set; }

        [Required]
        public string ScopeCriteria { get; set; }

        public string Assessor { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public decimal? ComplianceScore { get; set; }

        public int? FindingsCount { get; set; }

        public string Findings { get; set; }

        public string ActionPlanReference { get; set; }

        public DateTime? NextAssessmentDue { get; set; }

        public string Status { get; set; }

        public string RelatedObligations { get; set; }

        public string RelatedControls { get; set; }
    }
}
