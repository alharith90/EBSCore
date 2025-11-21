namespace EBSCore.Web.Models.BCM
{
    public class S7SRiskLevel
    {
        public int? RiskToleranceID { get; set; }
        public int? RiskMatrixConfigID { get; set; }
        public int HighThreshold { get; set; }
        public int MediumThreshold { get; set; }
        public string LowLabel { get; set; } = "Low";
        public string MediumLabel { get; set; } = "Medium";
        public string HighLabel { get; set; } = "High";
    }
}
