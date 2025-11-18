namespace EBSCore.Web.Models
{
    public class Employee
    {
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? OrganizationUnitId { get; set; }
        public string? SourceId { get; set; }
        public string? SourceSystem { get; set; }
        public string? JobTitle { get; set; }
        public string? JobFamily { get; set; }
        public int? SupervisorId { get; set; }
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

