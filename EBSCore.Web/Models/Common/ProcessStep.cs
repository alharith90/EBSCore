using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class ProcessStep
    {
        public int? StepID { get; set; }

        [Required]
        public int? ProcessID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int StepOrder { get; set; } = 1;

        public long? RoleID { get; set; }

        public string? ExpectedOutput { get; set; }

        public int? EscalationMinutes { get; set; }

        public string? ActivationCriteria { get; set; } = string.Empty;

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }

    public class ProcessStepSaveRequest
    {
        [Required]
        public int ProcessID { get; set; }

        public List<ProcessStep> Steps { get; set; } = new();
    }
}
