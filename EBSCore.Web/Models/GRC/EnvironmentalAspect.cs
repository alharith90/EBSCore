using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class EnvironmentalAspect
    {
        public long? AspectID { get; set; }
        public int? CompanyID { get; set; }
        public long? UnitID { get; set; }

        [Required]
        public string AspectDescription { get; set; }

        public string EnvironmentalImpact { get; set; }
        public string ImpactSeverity { get; set; }
        public string FrequencyLikelihood { get; set; }
        public string SignificanceRating { get; set; }
        public string ControlsInPlace { get; set; }
        public string LegalRequirement { get; set; }
        public string ImprovementActions { get; set; }
        public string AspectOwner { get; set; }
        public string MonitoringMetric { get; set; }
        public DateTime? LastEvaluationDate { get; set; }
        public string Status { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
