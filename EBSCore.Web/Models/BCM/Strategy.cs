using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class Strategy
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? StrategyID { get; set; }

        [Required(ErrorMessage = "TitleRequired")]
        public string? Title { get; set; }

        public string? TitleAr { get; set; }

        public string? Vision { get; set; }

        public string? Mission { get; set; }

        public string? EscalationCriteria { get; set; }

        public string? ActivationCriteria { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OwnerID { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
