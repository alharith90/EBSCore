namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class Credential
{
    public int CredentialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CredentialType { get; set; } = string.Empty;
    public string DataJson { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
