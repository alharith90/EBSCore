using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.GRC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GovernanceDashboardController : ControllerBase
    {
        private readonly User _currentUser;
        private readonly ILogger<GovernanceDashboardController> _logger;
        private readonly DBRiskRegisterSP _riskSP;
        private readonly DBControlLibrarySP _controlSP;
        private readonly DBComplianceAssessmentSP _complianceAssessmentSP;
        private readonly DBAuditPlanSP _auditPlanSP;
        private readonly DBAuditFindingSP _auditFindingSP;
        private readonly DBRiskTreatmentPlanSP _riskTreatmentPlanSP;
        private readonly DBComplianceIssueSP _complianceIssueSP;
        private readonly DBOrganisationUnitSP _organisationUnitSP;

        public GovernanceDashboardController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<GovernanceDashboardController> logger)
        {
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
            _riskSP = new DBRiskRegisterSP(configuration);
            _controlSP = new DBControlLibrarySP(configuration);
            _complianceAssessmentSP = new DBComplianceAssessmentSP(configuration);
            _auditPlanSP = new DBAuditPlanSP(configuration);
            _auditFindingSP = new DBAuditFindingSP(configuration);
            _riskTreatmentPlanSP = new DBRiskTreatmentPlanSP(configuration);
            _complianceIssueSP = new DBComplianceIssueSP(configuration);
            _organisationUnitSP = new DBOrganisationUnitSP(configuration);
        }

        [HttpGet]
        public object Get(int? organisationUnitId = null, bool includeChildren = true)
        {
            try
            {
                var risks = TableRows(GetTable(_riskSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvRisks", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var controls = TableRows(GetTable(_controlSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvControls", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var assessments = TableRows(GetTable(_complianceAssessmentSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvComplianceAssessments", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var audits = TableRows(GetTable(_auditPlanSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvAuditPlans", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var findings = TableRows(GetTable(_auditFindingSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvAuditFindings", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var actions = TableRows(GetTable(_riskTreatmentPlanSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvPlans", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var issues = TableRows(GetTable(_complianceIssueSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvComplianceIssues", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));
                var units = TableRows(GetTable(_organisationUnitSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvOrganisationUnits", CompanyID: _currentUser?.CompanyID, UserID: _currentUser?.UserID)));

                var selectedUnits = ResolveUnitScope(units, organisationUnitId, includeChildren);

                risks = FilterByUnitScope(risks, selectedUnits);
                controls = FilterByUnitScope(controls, selectedUnits);
                assessments = FilterByUnitScope(assessments, selectedUnits);
                audits = FilterByUnitScope(audits, selectedUnits);
                findings = FilterByUnitScope(findings, selectedUnits);
                actions = FilterByUnitScope(actions, selectedUnits);
                issues = FilterByUnitScope(issues, selectedUnits);

                var response = new GovernanceDashboardResponse
                {
                    ExecutiveSummary = BuildExecutiveSummary(risks, controls, assessments, audits, findings, actions, issues),
                    Heatmap = BuildRiskHeatmap(risks),
                    OrganisationScorecards = BuildOrganisationScorecards(units, risks, controls, assessments, findings, actions, includeChildren),
                    Alerts = BuildAlerts(risks, controls, assessments, audits, findings, actions, issues)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building governance dashboard response");
                return BadRequest("Error retrieving governance dashboard");
            }
        }

        private static GovernanceExecutiveSummary BuildExecutiveSummary(
            List<Dictionary<string, object>> risks,
            List<Dictionary<string, object>> controls,
            List<Dictionary<string, object>> assessments,
            List<Dictionary<string, object>> audits,
            List<Dictionary<string, object>> findings,
            List<Dictionary<string, object>> actions,
            List<Dictionary<string, object>> issues)
        {
            var activeRisks = risks.Where(r => !StringEquals(GetString(r, "Status"), "Closed")).ToList();
            var criticalRisks = activeRisks.Count(r => IsCriticalRisk(r));
            var aboveAppetite = activeRisks.Count(r => IsAboveAppetite(r));
            var effectiveControls = controls.Count(c => StringEquals(GetString(c, "ControlEffectivenessStatus"), "Effective"));
            var completedAssessments = assessments.Count(a => StringEquals(GetString(a, "Status"), "Completed"));
            var completedAudits = audits.Count(a => StringEquals(GetString(a, "Status"), "Completed"));
            var openFindings = findings.Count(f => !StringEquals(GetString(f, "Status"), "Closed"));
            var overdueActions = actions.Count(a => IsOverdue(a, "TargetDate") && !StringEquals(GetString(a, "Status"), "Closed"));
            var repeatedIssues = issues.Count(i => StringEquals(GetString(i, "IssueType"), "Repeated") || StringEquals(GetString(i, "Status"), "Reopened"));
            var complianceScore = assessments.Select(a => GetDecimal(a, "ComplianceScore")).DefaultIfEmpty(0).Average();
            var controlEffectivenessRate = controls.Count == 0 ? 0 : Math.Round((decimal)effectiveControls / controls.Count * 100, 2);
            var auditCoverageRate = audits.Count == 0 ? 0 : Math.Round((decimal)completedAudits / audits.Count * 100, 2);

            var healthScore = Math.Round(
                (controlEffectivenessRate * 0.25m) +
                (complianceScore * 0.25m) +
                (auditCoverageRate * 0.2m) +
                (Math.Max(0, 100 - (criticalRisks * 5)) * 0.15m) +
                (Math.Max(0, 100 - (openFindings * 2)) * 0.15m), 2);

            return new GovernanceExecutiveSummary
            {
                TotalActiveRisks = activeRisks.Count,
                CriticalRisks = criticalRisks,
                RisksAboveAppetite = aboveAppetite,
                ControlEffectivenessRate = controlEffectivenessRate,
                ComplianceScore = Math.Round(complianceScore, 2),
                CompletedAssessments = completedAssessments,
                AuditCoverageRate = auditCoverageRate,
                OpenFindings = openFindings,
                OverdueActions = overdueActions,
                RepeatedIssues = repeatedIssues,
                GovernanceHealthScore = healthScore
            };
        }

        private static List<GovernanceHeatmapCell> BuildRiskHeatmap(List<Dictionary<string, object>> risks)
        {
            return risks
                .Where(r => !StringEquals(GetString(r, "Status"), "Closed"))
                .GroupBy(r => new { Likelihood = NormalizeScale(GetString(r, "Likelihood")), Impact = NormalizeScale(GetString(r, "Impact")) })
                .Select(g => new GovernanceHeatmapCell
                {
                    Likelihood = g.Key.Likelihood,
                    Impact = g.Key.Impact,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        private static List<GovernanceOrganisationScorecard> BuildOrganisationScorecards(
            List<Dictionary<string, object>> units,
            List<Dictionary<string, object>> risks,
            List<Dictionary<string, object>> controls,
            List<Dictionary<string, object>> assessments,
            List<Dictionary<string, object>> findings,
            List<Dictionary<string, object>> actions,
            bool includeChildren)
        {
            var scorecards = new List<GovernanceOrganisationScorecard>();

            foreach (var unit in units)
            {
                var unitId = GetString(unit, "UnitID");
                if (string.IsNullOrWhiteSpace(unitId))
                {
                    continue;
                }

                var unitScope = includeChildren
                    ? ResolveUnitScope(units, TryParseInt(unitId), true)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase) { unitId };

                var unitRisks = FilterByUnitScope(risks, unitScope);
                var unitControls = FilterByUnitScope(controls, unitScope);
                var unitAssessments = FilterByUnitScope(assessments, unitScope);
                var unitFindings = FilterByUnitScope(findings, unitScope);
                var unitActions = FilterByUnitScope(actions, unitScope);

                var effectiveControls = unitControls.Count(c => StringEquals(GetString(c, "ControlEffectivenessStatus"), "Effective"));
                var controlRate = unitControls.Count == 0 ? 0 : Math.Round((decimal)effectiveControls / unitControls.Count * 100, 2);
                var complianceScore = unitAssessments.Select(a => GetDecimal(a, "ComplianceScore")).DefaultIfEmpty(0).Average();
                var repeatedFindings = unitFindings.Count(f => StringEquals(GetString(f, "Status"), "Reopened"));
                var overdueActions = unitActions.Count(a => IsOverdue(a, "TargetDate") && !StringEquals(GetString(a, "Status"), "Closed"));
                var riskCount = unitRisks.Count(r => !StringEquals(GetString(r, "Status"), "Closed"));
                var highRiskCount = unitRisks.Count(IsCriticalRisk);

                scorecards.Add(new GovernanceOrganisationScorecard
                {
                    UnitID = unitId,
                    UnitName = GetString(unit, "UnitName"),
                    ParentUnitID = GetString(unit, "ParentUnitID"),
                    RiskCount = riskCount,
                    HighRiskCount = highRiskCount,
                    ControlEffectivenessPercentage = controlRate,
                    ComplianceScore = Math.Round(complianceScore, 2),
                    AuditFindingsCount = unitFindings.Count,
                    OverdueActions = overdueActions,
                    RepeatedFindings = repeatedFindings,
                    GovernanceHealthScore = CalculateHealthScore(controlRate, complianceScore, highRiskCount, unitFindings.Count)
                });
            }

            return scorecards.OrderByDescending(s => s.GovernanceHealthScore).ThenBy(s => s.UnitName).ToList();
        }

        private static List<GovernanceAlert> BuildAlerts(
            List<Dictionary<string, object>> risks,
            List<Dictionary<string, object>> controls,
            List<Dictionary<string, object>> assessments,
            List<Dictionary<string, object>> audits,
            List<Dictionary<string, object>> findings,
            List<Dictionary<string, object>> actions,
            List<Dictionary<string, object>> issues)
        {
            var alerts = new List<GovernanceAlert>();

            alerts.AddRange(risks.Where(IsAboveAppetite).Select(r => new GovernanceAlert
            {
                AlertType = "RiskAboveAppetite",
                Reference = GetString(r, "RiskID"),
                Description = $"Risk '{GetString(r, "RiskTitle")}' is above appetite threshold",
                Severity = "High",
                DueDate = GetDate(r, "NextReviewDate")
            }));

            alerts.AddRange(actions.Where(a => IsOverdue(a, "TargetDate") && !StringEquals(GetString(a, "Status"), "Closed")).Select(a => new GovernanceAlert
            {
                AlertType = "OverdueAction",
                Reference = GetString(a, "ActionID"),
                Description = $"Action '{GetString(a, "ActionTitle")}' is overdue",
                Severity = "High",
                DueDate = GetDate(a, "TargetDate")
            }));

            alerts.AddRange(findings.Where(f => !StringEquals(GetString(f, "Status"), "Closed") && StringEquals(GetString(f, "Severity"), "Critical")).Select(f => new GovernanceAlert
            {
                AlertType = "CriticalFinding",
                Reference = GetString(f, "FindingID"),
                Description = $"Critical finding '{GetString(f, "FindingTitle")}' requires immediate closure",
                Severity = "Critical",
                DueDate = GetDate(f, "DueDate")
            }));

            alerts.AddRange(audits.Where(a => IsOverdue(a, "EndDate") && !StringEquals(GetString(a, "Status"), "Completed")).Select(a => new GovernanceAlert
            {
                AlertType = "DelayedAudit",
                Reference = GetString(a, "AuditID"),
                Description = $"Audit '{GetString(a, "AuditTitle")}' is delayed",
                Severity = "Medium",
                DueDate = GetDate(a, "EndDate")
            }));

            alerts.AddRange(assessments.Where(a => IsOverdue(a, "NextAssessmentDue") && !StringEquals(GetString(a, "Status"), "Completed")).Select(a => new GovernanceAlert
            {
                AlertType = "OverdueAssessment",
                Reference = GetString(a, "AssessmentID"),
                Description = $"Compliance assessment '{GetString(a, "ScopeCriteria")}' is overdue",
                Severity = "Medium",
                DueDate = GetDate(a, "NextAssessmentDue")
            }));

            alerts.AddRange(issues.Where(i => StringEquals(GetString(i, "Status"), "Reopened")).Select(i => new GovernanceAlert
            {
                AlertType = "RepeatedIssue",
                Reference = GetString(i, "IssueID"),
                Description = $"Issue '{GetString(i, "IssueTitle")}' has been reopened",
                Severity = "Medium",
                DueDate = GetDate(i, "DueDate")
            }));

            alerts.AddRange(controls.Where(c => StringEquals(GetString(c, "ControlEffectivenessStatus"), "Ineffective")).Select(c => new GovernanceAlert
            {
                AlertType = "ControlFailure",
                Reference = GetString(c, "ControlID"),
                Description = $"Control '{GetString(c, "ControlName")}' is marked ineffective",
                Severity = "High"
            }));

            return alerts
                .OrderByDescending(a => SeverityWeight(a.Severity))
                .ThenBy(a => a.DueDate)
                .Take(50)
                .ToList();
        }

        private static decimal CalculateHealthScore(decimal controlRate, decimal complianceScore, int highRiskCount, int findingsCount)
        {
            return Math.Round(
                (controlRate * 0.4m) +
                (complianceScore * 0.3m) +
                (Math.Max(0, 100 - highRiskCount * 10) * 0.2m) +
                (Math.Max(0, 100 - findingsCount * 4) * 0.1m), 2);
        }

        private static List<Dictionary<string, object>> FilterByUnitScope(List<Dictionary<string, object>> rows, HashSet<string> selectedUnits)
        {
            if (selectedUnits.Count == 0)
            {
                return rows;
            }

            return rows.Where(r =>
            {
                var unitId = GetString(r, "OrganisationUnitID");
                return !string.IsNullOrWhiteSpace(unitId) && selectedUnits.Contains(unitId);
            }).ToList();
        }

        private static HashSet<string> ResolveUnitScope(List<Dictionary<string, object>> units, int? rootUnitId, bool includeChildren)
        {
            if (rootUnitId == null)
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var root = rootUnitId.Value.ToString(CultureInfo.InvariantCulture);
            var scope = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { root };

            if (!includeChildren)
            {
                return scope;
            }

            var queue = new Queue<string>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var child in units.Where(u => StringEquals(GetString(u, "ParentUnitID"), current)))
                {
                    var childId = GetString(child, "UnitID");
                    if (!string.IsNullOrWhiteSpace(childId) && scope.Add(childId))
                    {
                        queue.Enqueue(childId);
                    }
                }
            }

            return scope;
        }

        private static DataTable GetTable(object result)
        {
            if (result is DataSet dataSet && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }

            return new DataTable();
        }

        private static List<Dictionary<string, object>> TableRows(DataTable table)
        {
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var item = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn column in table.Columns)
                {
                    item[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
                }

                rows.Add(item);
            }

            return rows;
        }

        private static string GetString(Dictionary<string, object> row, string key)
        {
            return row.TryGetValue(key, out var value) && value != null ? Convert.ToString(value) ?? string.Empty : string.Empty;
        }

        private static decimal GetDecimal(Dictionary<string, object> row, string key)
        {
            var value = GetString(row, key);
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : decimal.TryParse(value, out parsed) ? parsed : 0;
        }

        private static DateTime? GetDate(Dictionary<string, object> row, string key)
        {
            var value = GetString(row, key);
            return DateTime.TryParse(value, out var parsed) ? parsed.Date : null;
        }

        private static bool IsOverdue(Dictionary<string, object> row, string dateKey)
        {
            var dueDate = GetDate(row, dateKey);
            return dueDate.HasValue && dueDate.Value < DateTime.UtcNow.Date;
        }

        private static bool StringEquals(string left, string right)
        {
            return string.Equals(left?.Trim(), right?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCriticalRisk(Dictionary<string, object> risk)
        {
            var residual = GetString(risk, "ResidualRiskLevel");
            var score = GetDecimal(risk, "RiskScore");
            return StringEquals(residual, "Critical") || StringEquals(residual, "High") || score >= 20;
        }

        private static bool IsAboveAppetite(Dictionary<string, object> risk)
        {
            var score = GetDecimal(risk, "RiskScore");
            var appetiteText = GetString(risk, "RiskAppetiteThreshold");

            if (decimal.TryParse(appetiteText, NumberStyles.Any, CultureInfo.InvariantCulture, out var threshold) || decimal.TryParse(appetiteText, out threshold))
            {
                return score > threshold;
            }

            if (StringEquals(appetiteText, "Low")) return score >= 6;
            if (StringEquals(appetiteText, "Medium")) return score >= 12;
            if (StringEquals(appetiteText, "High")) return score >= 20;

            return score >= 20;
        }

        private static int SeverityWeight(string severity)
        {
            if (string.Equals(severity, "Critical", StringComparison.OrdinalIgnoreCase)) return 4;
            if (string.Equals(severity, "High", StringComparison.OrdinalIgnoreCase)) return 3;
            if (string.Equals(severity, "Medium", StringComparison.OrdinalIgnoreCase)) return 2;
            return 1;
        }

        private static string NormalizeScale(string scale)
        {
            return string.IsNullOrWhiteSpace(scale) ? "Unspecified" : scale.Trim();
        }

        private static int TryParseInt(string value)
        {
            return int.TryParse(value, out var parsed) ? parsed : 0;
        }
    }
}
