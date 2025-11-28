using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class Supplier
    {
        public string? SupplierID { get; set; }
        public string? CompanyID { get; set; }

        [Required]
        public string? UnitID { get; set; }

        [Required]
        public string? SupplierType { get; set; }

        [Required]
        public string? SupplierName { get; set; }

        public string? Services { get; set; }
        public string? MainContactName { get; set; }
        public string? MainContactEmail { get; set; }
        public string? MainContactPhone { get; set; }
        public string? SecondaryContactName { get; set; }
        public string? SecondaryContactEmail { get; set; }
        public string? SecondaryContactPhone { get; set; }
        public bool? SLAInPlace { get; set; }
        public string? RTOHours { get; set; }
        public string? RPOHours { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
    }
}
