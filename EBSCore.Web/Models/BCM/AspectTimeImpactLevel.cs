namespace EBSCore.Web.Models
{
    public class AspectTimeImpactLevel
    {
        public int? AspectTimeImpactLevelID { get; set; }
        public int? ImpactAspectID { get; set; }
        public int? ImpactTimeFrameID { get; set; }
        public int? LevelID { get; set; }
        public string? ImpactLevel { get; set; }
        public string? Justification { get; set; }
        public string? ImpactColor { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
