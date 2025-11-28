using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class Stakeholder
    {
        public string? StakeholderID { get; set; }
        public string? CompanyID { get; set; }

        [Required]
        public string? UnitID { get; set; }

        [Required]
        public string? StakeholderName { get; set; }

        [Required]
        public string? StakeholderType { get; set; }

        public string? Role { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public bool? IsCritical { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
    }
}
