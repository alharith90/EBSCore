using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SRiskMatrixConfiguration
    {
        public int? RiskMatrixConfigID { get; set; }
        public string MatrixName { get; set; } = "Default Matrix";
        public int MatrixSize { get; set; } = 5;
        public bool IsDynamic { get; set; }
        public string? ConfigJson { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
