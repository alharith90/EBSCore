using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class EnvironmentalObjective
    {
        public long? ObjectiveID { get; set; }
        public int? CompanyID { get; set; }
        public long? UnitID { get; set; }

        [Required]
        public string ObjectiveDescription { get; set; }

        public string TargetValue { get; set; }
        public string Unit { get; set; }
        public string BaselineValue { get; set; }
        public string CurrentValue { get; set; }
        public DateTime? TargetDate { get; set; }
        public string ResponsibleOwner { get; set; }
        public string Status { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
