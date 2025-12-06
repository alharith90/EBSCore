using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class VendorRisk
    {
        public long? VendorRiskID { get; set; }

        [Required]
        public string VendorName { get; set; }

        public string RiskDescription { get; set; }

        public string RiskRating { get; set; }

        public string KeyControls { get; set; }

        public string ContractualObligations { get; set; }

        public string VendorOwner { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string Status { get; set; }
    }
}
