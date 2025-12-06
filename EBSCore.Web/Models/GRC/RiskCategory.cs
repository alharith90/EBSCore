namespace EBSCore.Web.Models.GRC
{
    public class RiskCategory
    {
        public int? CategoryID { get; set; }
        public int CompanyID { get; set; }
        public string CategoryNameEN { get; set; }
        public string CategoryNameAR { get; set; }
        public string DescriptionEN { get; set; }
        public string DescriptionAR { get; set; }
        public int? ParentCategoryID { get; set; }
        public string StatusID { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
