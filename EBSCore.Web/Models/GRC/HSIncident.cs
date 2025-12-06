using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class HSIncident
    {
        public long? IncidentID { get; set; }

        [Required]
        public string IncidentTitle { get; set; }

        [Required]
        public DateTime? IncidentDateTime { get; set; }

        public string Location { get; set; }

        public string IncidentType { get; set; }

        public string PersonsInvolved { get; set; }

        public string InjurySeverity { get; set; }

        public string Description { get; set; }

        public string ImmediateActions { get; set; }

        public string RootCause { get; set; }

        public string RelatedHazard { get; set; }

        public string RelatedActivity { get; set; }

        public string RelatedRegulation { get; set; }

        public string CorrectiveActions { get; set; }

        public string IncidentStatus { get; set; }

        public string ReportedBy { get; set; }

        public bool? Reportable { get; set; }

        public DateTime? DateClosed { get; set; }
    }
}
