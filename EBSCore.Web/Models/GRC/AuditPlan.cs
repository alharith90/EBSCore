using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class AuditPlan
    {
        public long? AuditID { get; set; }

        [Required]
        public string AuditTitle { get; set; }

        public string AuditType { get; set; }

        public string ObjectivesScope { get; set; }

        public string Auditee { get; set; }

        public string LeadAuditor { get; set; }

        public string AuditTeam { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string AuditCriteria { get; set; }

        public int? FindingsCount { get; set; }

        public string OverallResult { get; set; }

        public DateTime? ReportIssuedDate { get; set; }

        public string RelatedRisksReviewed { get; set; }

        public string RelatedControlsReviewed { get; set; }

        public string RelatedObligationsReviewed { get; set; }

        public DateTime? NextAuditDate { get; set; }
    }
}
