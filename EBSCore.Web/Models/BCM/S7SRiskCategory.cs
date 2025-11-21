using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SRiskCategory
    {
        public int? RiskCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
