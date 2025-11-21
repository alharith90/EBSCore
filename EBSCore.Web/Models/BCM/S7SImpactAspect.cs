namespace EBSCore.Web.Models.BCM
{
    public class S7SImpactAspect
    {
        public int ImpactAspectID { get; set; }
        public string AspectName { get; set; } = string.Empty;
        public int? ImpactTimeFrameID { get; set; }
        public int? ImpactID { get; set; }
        public string? ImpactLevel { get; set; }
    }
}
