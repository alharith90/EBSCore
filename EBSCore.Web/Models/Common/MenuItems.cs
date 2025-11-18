namespace EBSCore.Web.Models
{
    public class MenuItem
    {
        public string? MenuItemID { get; set; }
        public string? ParentID { get; set; }
        public string? LabelAR { get; set; }
        public string? LabelEN { get; set; }
        public string? DescriptionAR { get; set; }
        public string? DescriptionEn { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public string? Order { get; set; }
        public string? IsActive { get; set; }
        public string? Permission { get; set; }
        public string? Type { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
    }
}
