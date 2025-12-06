using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class ESGMetric
    {
        public long? MetricID { get; set; }
        public int? CompanyID { get; set; }

        [Required]
        public string MetricName { get; set; }

        public string Category { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public string DataSource { get; set; }
        public string ReportingFrequency { get; set; }
        public string TargetValue { get; set; }
        public string LatestValue { get; set; }
        public DateTime? MeasurementDate { get; set; }
        public string Owner { get; set; }
        public string RelatedObjective { get; set; }
        public string RelatedRisk { get; set; }
        public string Trend { get; set; }
        public string Comments { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
