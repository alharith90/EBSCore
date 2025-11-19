namespace EBSCore.Web.WorkflowEngine.Application.DTOs;

public class CredentialDto
{
    public int CredentialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CredentialType { get; set; } = string.Empty;
    public string DataJson { get; set; } = string.Empty;
}

public class CredentialRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string CredentialType { get; set; } = string.Empty;
    public string DataJson { get; set; } = string.Empty;
}
