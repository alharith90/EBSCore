namespace EBSCore.Web.Models
{
    public class Lookup
    {
        public string? LookupID { get; set; } // Primary Key
        public string? LookupType { get; set; } // Type of lookup (e.g., Priority, Status)
        public string? LookupDescriptionAr { get; set; } // Description in Arabic
        public string? LookupDescriptionEn { get; set; } // Description in English
        public string? ParentID { get; set; } // Reference to parent lookup for hierarchy
        public string? Level { get; set; } // Level in the hierarchy (e.g., 1, 2, 3)
        public bool? Status { get; set; } // Status of the lookup (Active/Inactive)
        public string? CreatedBy { get; set; } // User ID of the creator
        public DateTime? CreatedAt { get; set; } // Creation timestamp
        public string? UpdatedBy { get; set; } // User ID of the last updater
        public DateTime? UpdatedAt { get; set; } // Last update timestamp
    }
}
