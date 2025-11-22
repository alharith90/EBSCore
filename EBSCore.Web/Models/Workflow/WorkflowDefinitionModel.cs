using System.Collections.Generic;

namespace EBSCore.Web.Models.Workflow
{
    public class WorkflowDefinition : WorkflowModel
    {
        public List<WorkflowNodeModel> Nodes { get; set; } = new List<WorkflowNodeModel>();
        public List<WorkflowConnectionModel> Connections { get; set; } = new List<WorkflowConnectionModel>();
        public List<WorkflowTriggerModel> Triggers { get; set; } = new List<WorkflowTriggerModel>();
    }
}
