using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.BCM
{
    public class BcmPositionResponsibility
    {
        public string? CompanyID { get; set; }

        public string? UnitID { get; set; }

        public string? PositionID { get; set; }

        [Required(ErrorMessage = "PositionTitleRequired")]
        public string? PositionTitle { get; set; }

        public string? PositionCode { get; set; }

        [Required(ErrorMessage = "ResponsibilitiesRequired")]
        public string? Responsibilities { get; set; }

        public string? AuthorityLevel { get; set; }

        public string? ContactDetails { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? CreatedAt { get; set; }

        public string? UpdatedAt { get; set; }
    }
}
