namespace EBSCore.Web.Models
{
    public class SysOrganisationUnit
    {
        public string? UnitID { get; set; }
        public string? UnitCode { get; set; }
        public string? UnitName { get; set; }
        public string? Description { get; set; }
        public string? UnitTypeID { get; set; }
        public string? ColorCode { get; set; }
        public string? ParentUnitID { get; set; }
        public string? ManagerUserID { get; set; }
        public string? IsBCMLevel { get; set; }
        public string? Status { get; set; }
        public string? ExternalID { get; set; }
        public string? Location { get; set; }
        public string? HierarchyLevel { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
    }

}
