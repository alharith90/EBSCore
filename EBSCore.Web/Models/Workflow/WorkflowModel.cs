namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowModel
    {
        public int? WorkflowID { get; set; }
        public int? CompanyID { get; set; }
        public long? UnitID { get; set; }
        public string? WorkflowCode { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowDescription { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Frequency { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
