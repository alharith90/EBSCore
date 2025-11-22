namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowCredentialModel
    {
        public int? CredentialID { get; set; }
        public string? CredentialName { get; set; }
        public string? CredentialType { get; set; }
        public string? DataJson { get; set; }
        public string? Notes { get; set; }
    }
}
