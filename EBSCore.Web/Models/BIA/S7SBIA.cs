using System.Collections.Generic;

namespace EBSCore.Web.Models.BIA
{
    public class S7SBIA
    {
        public string? BIAID { get; set; }
        public string? CompanyID { get; set; }
        public string? BIACode { get; set; }
        public string? UnitID { get; set; }
        public string? ProcessID { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessDescription { get; set; }
        public string? Frequency { get; set; }
        public string? Criticality { get; set; }
        public string? RTO { get; set; }
        public string? RPO { get; set; }
        public string? MTPD { get; set; }
        public string? MAO { get; set; }
        public string? MBCO { get; set; }
        public string? Priority { get; set; }
        public string? RequiredCompetencies { get; set; }
        public string? AlternativeWorkLocation { get; set; }
        public string? RegulatoryRequirements { get; set; }
        public string? PrimaryStaff { get; set; }
        public string? BackupStaff { get; set; }
        public string? RTOJustification { get; set; }
        public string? MBCODetails { get; set; }
        public string? RevenueLossPerHour { get; set; }
        public string? CostOfDowntime { get; set; }
        public string? Remarks { get; set; }
        public string? LastComment { get; set; }
        public string? ReviewDate { get; set; }
        public string? WorkFlowStatus { get; set; }
        public string? IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedAt { get; set; }

        public List<S7SBIAKeyContact> KeyContacts { get; set; } = new();
        public List<S7SBIADependency> Dependencies { get; set; } = new();
        public List<S7SBIARecord> VitalRecords { get; set; } = new();
    }

    public class S7SBIAKeyContact
    {
        public string? EmployeeId { get; set; }
        public string? Role { get; set; }
        public string? ContactType { get; set; }
    }

    public class S7SBIADependency
    {
        public string? DependencyId { get; set; }
        public string? DependencyType { get; set; }
        public string? Description { get; set; }
    }

    public class S7SBIARecord
    {
        public string? VitalRecordId { get; set; }
        public string? RecordName { get; set; }
        public string? RecoveryTimeObjective { get; set; }
    }
}
