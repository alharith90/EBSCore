using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class BusinessImpactAnalysis
    {
        public long? BIAID { get; set; }

        [Required]
        public string ProcessName { get; set; }

        public string Department { get; set; }

        public string ProcessOwner { get; set; }

        public string CriticalityLevel { get; set; }

        public string MAO { get; set; }

        public string Impact1Hour { get; set; }

        public string Impact1Day { get; set; }

        public string Impact1Week { get; set; }

        public string ImpactDimensions { get; set; }

        public string RTO { get; set; }

        public string RPO { get; set; }

        public string MinimumResources { get; set; }

        public string InternalDependencies { get; set; }

        public string ExternalDependencies { get; set; }

        public string RecoveryStrategies { get; set; }

        public string StrategyLibraryRef { get; set; }

        public string AllowableDataLoss { get; set; }

        public string BackupAvailability { get; set; }

        public bool HasAlternateWorkaround { get; set; }

        public string BCPLink { get; set; }

        public DateTime? LastReviewDate { get; set; }
    }
}
