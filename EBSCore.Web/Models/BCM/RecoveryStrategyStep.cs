using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class RecoveryStrategyStep
    {
        public int? StepID { get; set; }

        public int? RecoveryStrategyID { get; set; }

        public int StepOrder { get; set; }

        [Required]
        public string StepDescription { get; set; } = string.Empty;

        public string? ValidationCheck { get; set; }
    }
}
