using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class KPIResult
    {
        public string? CompanyID { get; set; }

        public string? ResultID { get; set; }

        [Required]
        public string KPIID { get; set; } = string.Empty;

        public string? Period { get; set; }

        public decimal? Value { get; set; }

        public string? Status { get; set; }

        public string? Comments { get; set; }

        public string? EvidenceFile { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedAt { get; set; }
    }
}
