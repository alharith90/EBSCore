using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SPostIncident
    {
        public int PostIncidentReportID { get; set; }
        public int PlanID { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string MandatoryControls { get; set; } = string.Empty;
        public long? AttachmentID { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
