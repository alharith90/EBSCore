using System;
using System.Collections.Generic;
using System.Linq;

namespace EBSCore.Web.Models.BCM
{
    public class S7SCommittee
    {
        public int CommitteeID { get; set; }
        public string? Name { get; set; }
        public string? Charter { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public string? Chairperson { get; set; }
        public string? Status { get; set; }
        public List<S7SCommitteeMember> Members { get; set; } = new();
        public List<S7SCommitteePlanLink> Plans { get; set; } = new();
    }

    public class S7SCommitteeMember
    {
        public int MembershipID { get; set; }
        public int? CommitteeID { get; set; }
        public int? TeamMemberID { get; set; }
        public string? Role { get; set; }
        public string? Responsibility { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class S7SRole
    {
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? Responsibilities { get; set; }
        public string? AssignedTo { get; set; }
        public List<S7SResponsibility> ResponsibilityMatrix { get; set; } = new();
        public List<S7SCompetency> Competencies { get; set; } = new();
        public List<S7STrainingRequirement> TrainingRequirements { get; set; } = new();
    }

    public class S7SResponsibility
    {
        public int MatrixID { get; set; }
        public int? RoleID { get; set; }
        public string? Responsibility { get; set; }
        public string? AccountabilityType { get; set; }
    }

    public class S7SCompetency
    {
        public int CompetencyID { get; set; }
        public int? RoleID { get; set; }
        public string? SkillRequired { get; set; }
        public string? RequiredLevel { get; set; }
        public string? AssessedLevel { get; set; }
        public string? GapAnalysis { get; set; }
        public bool TrainingRequired { get; set; }

        public CompetencyGapResult EvaluateGap()
        {
            var requiredScore = S7SGapCalculator.ToScore(RequiredLevel);
            var assessedScore = S7SGapCalculator.ToScore(AssessedLevel);
            var delta = assessedScore - requiredScore;

            return new CompetencyGapResult
            {
                RequiredScore = requiredScore,
                AssessedScore = assessedScore,
                GapScore = delta,
                MeetsRequirement = delta >= 0,
                RecommendedAction = delta >= 0 ? "Maintain" : "Training"
            };
        }
    }

    public class S7SCommitteePlanLink
    {
        public int CommitteeID { get; set; }
        public int PlanID { get; set; }
        public string? PlanName { get; set; }
        public string? LinkageType { get; set; }
    }

    public class S7STrainingRequirement
    {
        public int TrainingRequirementID { get; set; }
        public int? RoleID { get; set; }
        public int? CompetencyID { get; set; }
        public string? TrainingCourse { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Notes { get; set; }
    }

    public class S7SGovernanceSnapshot
    {
        public List<S7SCommittee> Committees { get; set; } = new();
        public List<S7SRole> Roles { get; set; } = new();
        public List<S7SCompetency> Competencies { get; set; } = new();

        public IEnumerable<CompetencyGapResult> GetGapAnalysis()
        {
            foreach (var competency in Competencies)
            {
                var evaluation = competency.EvaluateGap();
                yield return evaluation with
                {
                    CompetencyID = competency.CompetencyID,
                    SkillRequired = competency.SkillRequired
                };
            }
        }
    }

    public record CompetencyGapResult
    {
        public int CompetencyID { get; init; }
        public string? SkillRequired { get; init; }
        public int RequiredScore { get; init; }
        public int AssessedScore { get; init; }
        public int GapScore { get; init; }
        public bool MeetsRequirement { get; init; }
        public string? RecommendedAction { get; init; }
    }

    public static class S7SGapCalculator
    {
        private static readonly Dictionary<string, int> _iso22301Scale = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Awareness", 1 },
            { "Basic", 1 },
            { "Working", 2 },
            { "Intermediate", 2 },
            { "Proficient", 3 },
            { "Skilled", 3 },
            { "Advanced", 4 },
            { "Expert", 4 }
        };

        public static int ToScore(string? level)
        {
            if (string.IsNullOrWhiteSpace(level))
            {
                return 0;
            }

            return _iso22301Scale.TryGetValue(level.Trim(), out var score) ? score : 0;
        }

        public static string DescribeGap(string? requiredLevel, string? assessedLevel)
        {
            var requiredScore = ToScore(requiredLevel);
            var assessedScore = ToScore(assessedLevel);
            var gap = assessedScore - requiredScore;

            if (gap >= 0)
            {
                return "Meets or exceeds ISO 22301 competency threshold";
            }

            if (gap == -1)
            {
                return "Minor gap - prioritize refresher training";
            }

            return "Significant gap - schedule formal competency development";
        }
    }
}
