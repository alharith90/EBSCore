namespace EBSCore.Web.Models.GRC
{
    public class RiskTemplate
    {
        public int? TemplateID { get; set; }
        public int CompanyID { get; set; }
        public string TemplateNameEN { get; set; }
        public string TemplateNameAR { get; set; }
        public int? DefaultCategoryID { get; set; }
        public string DefaultImpact { get; set; }
        public string DefaultLikelihood { get; set; }
        public string DefaultRiskLevel { get; set; }
        public string GuidanceEN { get; set; }
        public string GuidanceAR { get; set; }
        public string StatusID { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
