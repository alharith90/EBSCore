using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class StrategicInitiative
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? InitiativeID { get; set; }

        [Required]
        public string ObjectiveID { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? OwnerID { get; set; }

        public string? DepartmentID { get; set; }

        public decimal? Budget { get; set; }

        public int? Progress { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? Status { get; set; }

        public string? EscalationCriteria { get; set; }

        public string? EscalationContact { get; set; }

        public string? ActivationCriteria { get; set; }

        public string? ActivationTrigger { get; set; }

        public string? ActivationStatus { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
