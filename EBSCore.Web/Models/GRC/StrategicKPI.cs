using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class StrategicKPI
    {
        public long? KPIID { get; set; }

        [Required]
        public long? ObjectiveID { get; set; }

        [Required]
        public string KPINameEN { get; set; }

        public string KPINameAR { get; set; }

        public string DescriptionEN { get; set; }

        public string DescriptionAR { get; set; }

        public string FormulaEN { get; set; }

        public string FormulaAR { get; set; }

        public decimal? TargetValue { get; set; }

        public decimal? BaselineValue { get; set; }

        public string Direction { get; set; }

        public string Frequency { get; set; }

        public long? OwnerUserID { get; set; }

        public string DataSourceEN { get; set; }

        public string DataSourceAR { get; set; }

        public decimal? ThresholdGreen { get; set; }

        public decimal? ThresholdAmber { get; set; }

        public decimal? ThresholdRed { get; set; }

        public bool? IsLeadingIndicator { get; set; }

        public string StatusID { get; set; }
    }
}
