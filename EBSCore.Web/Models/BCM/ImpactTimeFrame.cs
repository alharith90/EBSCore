namespace EBSCore.Web.Models
{
    public class ImpactTimeFrame
    {
        public int? ImpactTimeFrameID { get; set; }
        public string? TimeLabel { get; set; }
        public int? MinHours { get; set; }
        public int? MaxHours { get; set; }
    }
}
