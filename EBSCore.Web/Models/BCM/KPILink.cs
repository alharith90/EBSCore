using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class KPILink
    {
        public string? CompanyID { get; set; }

        public string? LinkID { get; set; }

        [Required]
        public string KPIID { get; set; } = string.Empty;

        public string? LinkedType { get; set; }

        public string? LinkedID { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedAt { get; set; }
    }
}
