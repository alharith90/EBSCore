using System.Collections.Generic;

namespace EBSCore.Web.WorkflowEngine.Domain.Entities;

public class Workflow
{
    public int WorkflowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public IList<Node> Nodes { get; } = new List<Node>();
    public IList<NodeConnection> Connections { get; } = new List<NodeConnection>();
    public IList<WorkflowTrigger> Triggers { get; } = new List<WorkflowTrigger>();
}
