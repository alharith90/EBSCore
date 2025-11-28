using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models
{
    public class Incident
    {
        public long? IncidentID { get; set; }
        public int? CompanyID { get; set; }

        [Required]
        public string? UnitID { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime? IncidentDate { get; set; }

        public string? ReportedBy { get; set; }
        public string? AffectedAssets { get; set; }
        public string? RelatedRiskIDs { get; set; }
        public string? ImpactedActivities { get; set; }
        public string? EscalationLevel { get; set; }
        public string? EscalationNotes { get; set; }
        public bool EscalatedToBC { get; set; }
        public bool BCPActivated { get; set; }
        public string? ActivationReason { get; set; }
        public DateTime? ActivationTime { get; set; }
        public DateTime? RecoveryStartTime { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
