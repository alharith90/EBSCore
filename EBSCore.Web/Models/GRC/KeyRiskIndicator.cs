using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class KeyRiskIndicator
    {
        public long? IndicatorID { get; set; }

        [Required]
        public string IndicatorName { get; set; }

        public string RelatedRisk { get; set; }

        public string MeasurementFrequency { get; set; }

        public string DataSource { get; set; }

        public string ThresholdValue { get; set; }

        public string CurrentValue { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        public DateTime? LastUpdateDate { get; set; }
    }
}
