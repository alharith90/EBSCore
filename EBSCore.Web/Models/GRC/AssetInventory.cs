using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class AssetInventory
    {
        public long? AssetID { get; set; }

        [Required]
        public string AssetName { get; set; }

        public string AssetType { get; set; }

        public string Owner { get; set; }

        public string Location { get; set; }

        public string ConfidentialityRating { get; set; }

        public string IntegrityRating { get; set; }

        public string AvailabilityRating { get; set; }

        public string ValueCriticality { get; set; }
    }
}
