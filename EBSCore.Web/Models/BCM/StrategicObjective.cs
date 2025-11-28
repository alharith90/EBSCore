using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class StrategicObjective
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? ObjectiveID { get; set; }

        [Required]
        public string ObjectiveCode { get; set; } = string.Empty;

        [Required]
        public string Strategy { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? OwnerID { get; set; }

        public string? RiskLink { get; set; }

        public string? ComplianceLink { get; set; }

        public string Status { get; set; } = "On Track";

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? EscalationLevel { get; set; }

        public string? EscalationContact { get; set; }

        public string? ActivationCriteria { get; set; }

        public string ActivationStatus { get; set; } = "Not Activated";

        public string? ActivationTrigger { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
