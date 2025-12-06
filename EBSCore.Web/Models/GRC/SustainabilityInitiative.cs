using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class SustainabilityInitiative
    {
        public long? InitiativeID { get; set; }
        public int? CompanyID { get; set; }

        [Required]
        public string InitiativeName { get; set; }

        public string Description { get; set; }
        public string ESGCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ResponsibleDepartment { get; set; }
        public string KeyMetrics { get; set; }
        public string BudgetAllocated { get; set; }
        public string Outcome { get; set; }
        public string Status { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
