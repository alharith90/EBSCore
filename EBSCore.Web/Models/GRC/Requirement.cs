using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class Requirement
    {
        public string? RequirementCode { get; set; }

        [Required]
        public string? RequirementNo { get; set; }

        [Required]
        public string? RequirementType { get; set; }

        [Required]
        public string? RequirementTitle { get; set; }

        [Required]
        public string? RequirementDescription { get; set; }

        public string? Subcategory { get; set; }
        public string? RequirementDetails { get; set; }
        public string? RequirementTags { get; set; }
        public string? RequirementFrequency { get; set; }
        public string? ExternalAudit { get; set; }
        public string? InternalAudit { get; set; }
        public string? AuditReference { get; set; }
        public string? RiskCategory { get; set; }
        public string? ControlOwner { get; set; }
        public string? ControlOwnerFunction { get; set; }
        public string? EvidenceRequired { get; set; }
        public string? EvidenceDetails { get; set; }
        public string? ControlID { get; set; }

        [Required]
        public string? OrganizationUnitID { get; set; }

        [Required]
        public string? EscalationProcess { get; set; }

        public string? EscalationThreshold { get; set; }

        [Required]
        public string? BCMActivationType { get; set; }

        public string? BCMActivationDecision { get; set; }
        public string? EscalationContacts { get; set; }

        [Required]
        public string? Status { get; set; }

        public string? UpdatedAt { get; set; }
        public string? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
