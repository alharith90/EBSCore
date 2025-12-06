using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.GRC
{
    public class ControlLibrary
    {
        public long? ControlID { get; set; }

        [Required]
        public string ControlName { get; set; }

        public string Description { get; set; }

        public string ControlType { get; set; }

        public string ControlCategory { get; set; }

        public string ControlOwner { get; set; }

        public string Frequency { get; set; }

        public bool? IsKeyControl { get; set; }

        public string RelatedRisks { get; set; }

        public string RelatedObligations { get; set; }

        public string ImplementationStatus { get; set; }

        public DateTime? LastTestDate { get; set; }

        public string LastTestResult { get; set; }

        public string DocumentationReference { get; set; }
    }
}
