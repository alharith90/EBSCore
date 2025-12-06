using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class PolicyLibrary
    {
        public long? PolicyID { get; set; }

        public string PolicyCode { get; set; }

        [Required]
        public string PolicyNameEN { get; set; }

        public string PolicyNameAR { get; set; }

        public string PolicyType { get; set; }

        public string CategoryEN { get; set; }

        public string CategoryAR { get; set; }

        public string DescriptionEN { get; set; }

        public string DescriptionAR { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? ReviewDate { get; set; }

        public long? OwnerUserID { get; set; }

        public long? ApproverUserID { get; set; }

        public string StatusID { get; set; }

        public string RelatedRegulationIDs { get; set; }

        public string RelatedControlIDs { get; set; }

        public string VersionNumber { get; set; }

        public string DocumentPath { get; set; }

        public bool? IsMandatory { get; set; }

        public string AppliesToRoles { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
