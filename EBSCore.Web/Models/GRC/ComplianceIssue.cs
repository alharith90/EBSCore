using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class ComplianceIssue
    {
        public long? IssueID { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        public DateTime? DateDetected { get; set; }

        public string Source { get; set; }

        public string RelatedObligation { get; set; }

        public string Severity { get; set; }

        public string Impact { get; set; }

        public string RootCause { get; set; }

        public string CorrectiveActionPlan { get; set; }

        public DateTime? TargetCompletionDate { get; set; }

        public string ActionStatus { get; set; }

        public DateTime? RegulatoryReportingDate { get; set; }

        public string Status { get; set; }

        public string LessonsLearned { get; set; }
    }
}
