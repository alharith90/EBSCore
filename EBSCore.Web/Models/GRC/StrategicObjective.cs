using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class StrategicObjective
    {
        public long? ObjectiveID { get; set; }

        public string ObjectiveCode { get; set; }

        [Required]
        public string ObjectiveNameEN { get; set; }

        public string ObjectiveNameAR { get; set; }

        public string DescriptionEN { get; set; }

        public string DescriptionAR { get; set; }

        public string Category { get; set; }

        public string Perspective { get; set; }

        public long? OwnerUserID { get; set; }

        public long? DepartmentID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? TargetValue { get; set; }

        public string UnitEN { get; set; }

        public string UnitAR { get; set; }

        public string RiskAppetiteThreshold { get; set; }

        public string StatusID { get; set; }
    }
}
