using System;

namespace EBSCore.Web.Models.BCM
{
    public class S7SRiskLikelihood
    {
        public int? LikelihoodID { get; set; }
        public string LikelihoodName { get; set; } = string.Empty;
        public int LikelihoodValue { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
