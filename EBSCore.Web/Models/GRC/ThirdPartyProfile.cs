using System;

namespace EBSCore.Web.Models.GRC
{
    public class ThirdPartyProfile
    {
        public int? ThirdPartyID { get; set; }
        public int? CompanyID { get; set; }
        public string ThirdPartyName { get; set; }
        public string ServiceType { get; set; }
        public string CriticalityTier { get; set; }
        public string InherentRiskRating { get; set; }
        public string CountryRegion { get; set; }
        public string BusinessOwner { get; set; }
        public string ContractValue { get; set; }
        public DateTime? ContractExpiryDate { get; set; }
        public string KeySLAKPIRequirements { get; set; }
        public string ComplianceRequirements { get; set; }
        public bool? PrivacyDataProcessing { get; set; }
        public string RelatedAssetProcess { get; set; }
        public string ContingencyPlan { get; set; }
        public DateTime? LastAssessmentDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public string Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
