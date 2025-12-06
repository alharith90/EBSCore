using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class RiskTreatmentPlan
    {
        public int? ActionID { get; set; }

        [Required]
        public string RelatedRisk { get; set; }

        [Required]
        public string MitigationAction { get; set; }

        public string ActionOwner { get; set; }

        public DateTime? DueDate { get; set; }

        public string CompletionStatus { get; set; }

        public string TreatmentType { get; set; }

        public string AssociatedControl { get; set; }

        public string ProgressNotes { get; set; }

        public string Verification { get; set; }

        public string Effectiveness { get; set; }
    }
}
