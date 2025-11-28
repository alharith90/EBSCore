using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class ProcessControl
    {
        public int? ProcessControlID { get; set; }

        [Required]
        public int? ProcessID { get; set; }

        [Required]
        public int? StepID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Type { get; set; }

        public bool EvidenceRequired { get; set; }

        public string? RelatedStandards { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }

    public class ProcessControlSaveRequest
    {
        [Required]
        public int ProcessID { get; set; }

        public List<ProcessControl> Controls { get; set; } = new();
    }
}
