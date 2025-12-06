using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class CSRInitiative
    {
        public long? InitiativeID { get; set; }

        [Required]
        public string InitiativeName { get; set; }

        public string Description { get; set; }

        public string SocialEnvironmentalGoal { get; set; }

        public string ResponsibleTeam { get; set; }

        public string KeyMetrics { get; set; }

        public string ProgressStatus { get; set; }
    }
}
