using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class BusinessContinuityPlan
    {
        public long? BCPID { get; set; }

        [Required]
        public string PlanName { get; set; }

        public string Scope { get; set; }

        public string RecoveryTeamRoles { get; set; }

        public string ContactList { get; set; }

        public string InvocationCriteria { get; set; }

        public string RecoveryLocations { get; set; }

        public string RecoveryStrategyDetails { get; set; }

        public string KeySteps { get; set; }

        public string RequiredResources { get; set; }

        public string DependentSystems { get; set; }

        public string PlanRTO { get; set; }

        public string PlanRPO { get; set; }

        public string BackupSource { get; set; }

        public string AlternateSupplier { get; set; }

        public DateTime? LastTestDate { get; set; }

        public string TestResultSummary { get; set; }

        public string PlanOwner { get; set; }

        public string PlanStatusID { get; set; }
    }
}
