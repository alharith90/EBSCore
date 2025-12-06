using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class Stakeholder
    {
        public long? StakeholderID { get; set; }

        [Required]
        public string StakeholderName { get; set; }

        public string StakeholderType { get; set; }

        public string NeedsOrExpectations { get; set; }

        public string EngagementPlan { get; set; }

        public string Owner { get; set; }
    }
}
