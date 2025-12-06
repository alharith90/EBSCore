namespace EBSCore.Web.Models.GRC
{
    public class InformationSystem
    {
        public int? SystemID { get; set; }
        public string? CompanyID { get; set; }
        public string? UnitID { get; set; }
        public string? SystemName { get; set; }
        public string? RPO { get; set; }
        public string? ApplicationLifecycleStatus { get; set; }
        public string? Type { get; set; }
        public string? RequiredFor { get; set; }
        public string? SystemDescription { get; set; }
        public string? PrimaryOwnerId { get; set; }
        public string? SecondaryOwner { get; set; }
        public string? BusinessOwner { get; set; }
        public bool? InternetFacing { get; set; }
        public bool? ThirdPartyAccess { get; set; }
        public string? NumberOfUsers { get; set; }
        public string? LicenseType { get; set; }
        public string? Infrastructure { get; set; }
        public string? MFAEnabled { get; set; }
        public string? MFAStatusDetails { get; set; }
        public string? AssociatedInformationSystems { get; set; }
        public string? Confidentiality { get; set; }
        public string? Integrity { get; set; }
        public string? Availability { get; set; }
        public string? OverallCategorizationRating { get; set; }
        public string? HighestInformationClassification { get; set; }
        public string? RiskHighlightedByIT { get; set; }
        public string? AdditionalNote { get; set; }
        public byte[]? Logo { get; set; }
    }
}
