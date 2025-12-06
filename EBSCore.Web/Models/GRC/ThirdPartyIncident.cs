using System;

namespace EBSCore.Web.Models.GRC
{
    public class ThirdPartyIncident
    {
        public int? IssueIncidentID { get; set; }
        public int? CompanyID { get; set; }
        public int? ThirdPartyID { get; set; }
        public DateTime? Date { get; set; }
        public string IssueType { get; set; }
        public string Description { get; set; }
        public string ImpactOnBusiness { get; set; }
        public string Severity { get; set; }
        public string RelatedSLABreach { get; set; }
        public string ActionsTakenByVendor { get; set; }
        public string ActionsTakenInternally { get; set; }
        public string Status { get; set; }
        public string LinkedRiskEvent { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
