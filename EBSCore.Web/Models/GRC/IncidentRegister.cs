using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class IncidentRegister
    {
        public int? IncidentID { get; set; }

        [Required]
        public string IncidentDescription { get; set; }

        [Required]
        public DateTime? IncidentDate { get; set; }

        public string ImpactedArea { get; set; }

        public string Severity { get; set; }

        public string RootCause { get; set; }

        public string ActionsTaken { get; set; }

        public string IncidentOwner { get; set; }

        public string Status { get; set; }
    }
}
