using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class AuditUniverse
    {
        public long? EntityProcessID { get; set; }

        [Required]
        public string EntityProcessName { get; set; }

        public string EntityOwner { get; set; }

        public string RiskRating { get; set; }

        public DateTime? LastAuditDate { get; set; }

        public DateTime? NextAuditDue { get; set; }

        public string AuditFrequency { get; set; }

        public string AuditPriority { get; set; }
    }
}
