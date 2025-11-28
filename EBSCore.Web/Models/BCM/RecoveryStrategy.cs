using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class RecoveryStrategy
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? RecoveryStrategyID { get; set; }

        [Required]
        public string FailureScenario { get; set; } = string.Empty;

        [Required]
        public string Strategy { get; set; } = string.Empty;

        public decimal? CostImpact { get; set; }

        public string? ConfidenceLevel { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }

        public List<RecoveryStrategyStep> Steps { get; set; } = new();
    }
}
