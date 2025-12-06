using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class AuditFinding
    {
        public long? FindingID { get; set; }

        public long? AuditID { get; set; }

        [Required]
        public string FindingTitle { get; set; }

        public string Criteria { get; set; }

        public string Condition { get; set; }

        public string Cause { get; set; }

        public string Effect { get; set; }

        public string Recommendation { get; set; }

        public string Category { get; set; }

        public string Severity { get; set; }

        public string RelatedRiskControl { get; set; }

        public string ResponsibleOwner { get; set; }

        public string ActionPlanLink { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; }

        public DateTime? DateClosed { get; set; }

        public string VerifiedBy { get; set; }
    }
}
