using System;
using System.Collections.Generic;

namespace EBSCore.Web.Models.GRC
{
    public class GovernanceExecutiveSummary
    {
        public int TotalActiveRisks { get; set; }
        public int CriticalRisks { get; set; }
        public int RisksAboveAppetite { get; set; }
        public decimal ControlEffectivenessRate { get; set; }
        public decimal ComplianceScore { get; set; }
        public int CompletedAssessments { get; set; }
        public decimal AuditCoverageRate { get; set; }
        public int OpenFindings { get; set; }
        public int OverdueActions { get; set; }
        public int RepeatedIssues { get; set; }
        public decimal GovernanceHealthScore { get; set; }
    }

    public class GovernanceHeatmapCell
    {
        public string Likelihood { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class GovernanceOrganisationScorecard
    {
        public string UnitID { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ParentUnitID { get; set; } = string.Empty;
        public int RiskCount { get; set; }
        public int HighRiskCount { get; set; }
        public decimal ControlEffectivenessPercentage { get; set; }
        public decimal ComplianceScore { get; set; }
        public int AuditFindingsCount { get; set; }
        public int OverdueActions { get; set; }
        public int RepeatedFindings { get; set; }
        public decimal GovernanceHealthScore { get; set; }
    }

    public class GovernanceAlert
    {
        public string AlertType { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    public class GovernanceDashboardResponse
    {
        public GovernanceExecutiveSummary ExecutiveSummary { get; set; } = new();
        public List<GovernanceHeatmapCell> Heatmap { get; set; } = new();
        public List<GovernanceOrganisationScorecard> OrganisationScorecards { get; set; } = new();
        public List<GovernanceAlert> Alerts { get; set; } = new();
    }
}
