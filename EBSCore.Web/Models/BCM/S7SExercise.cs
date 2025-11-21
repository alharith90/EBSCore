using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SExercise
    {
        public int ExerciseID { get; set; }
        public int PlanID { get; set; }
        public int? ExerciseTypeID { get; set; }
        public string? ExerciseTypeName { get; set; }
        public DateTimeOffset? ScheduledAt { get; set; }
        public string? EvaluationNotes { get; set; }
    }
}
