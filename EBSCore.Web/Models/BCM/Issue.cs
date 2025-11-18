namespace EBSCore.Web.Models
{
    public class Issue
    {
        public int? IssueID { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Source { get; set; }
        public string? Impact { get; set; }
        public DateTime? DateIdentified { get; set; }
        public string? Owner { get; set; }
        public string? Status { get; set; }
        public string? RootCause { get; set; }
        public string? CorrectiveAction { get; set; }
        public string? ActionOwner { get; set; }
        public DateTime? ActionDueDate { get; set; }
        public DateTime? ActionCompletionDate { get; set; }
        public string? VerificationOfEffectiveness { get; set; }
        public string? RelatedProcess { get; set; }
        public string? AuditReference { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? RiskCategory { get; set; }
        public int? Likelihood { get; set; }
        public int? Consequence { get; set; }
        public string? DetectionMethod { get; set; }
        public string? IssueType { get; set; }
        public string? LinkedBCP { get; set; }
        public string? LessonsLearned { get; set; }
        public bool? IsRecurring { get; set; }
        public string? MitigationActions { get; set; }
        public string? EscalationLevel { get; set; }
        public string? ClosureApprovedBy { get; set; }
        public string? ClosureComments { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
