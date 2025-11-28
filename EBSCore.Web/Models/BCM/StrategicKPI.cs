using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class StrategicKPI
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? KPIID { get; set; }

        [Required]
        public string ObjectiveID { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? TargetValue { get; set; }

        public decimal? CurrentValue { get; set; }

        public string? Unit { get; set; }

        public string? Frequency { get; set; }

        public string? Status { get; set; }

        public string? Owner { get; set; }

        public string? EscalationPlan { get; set; }

        public string? ActivationCriteria { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
