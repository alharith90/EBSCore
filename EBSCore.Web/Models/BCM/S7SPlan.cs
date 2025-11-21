using System;
using System.Collections.Generic;

namespace EBSCore.Web.Models.BCM
{
    public class S7SPlan
    {
        public int PlanID { get; set; }
        public string PlanCode { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int CompanyID { get; set; }
        public long UnitID { get; set; }
        public DateTimeOffset NextReviewDate { get; set; }
        public int FrequencyMonths { get; set; }
        public IList<S7SExercise> Exercises { get; set; } = new List<S7SExercise>();
        public IList<S7SPlanVersion> Versions { get; set; } = new List<S7SPlanVersion>();
        public S7SPostIncident? PostIncident { get; set; }
    }

    public class S7SPlanVersion
    {
        public int PlanVersionID { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public string? ChangeSummary { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
