namespace EBSCore.Web.Models.BCM
{
    public class S7SStrategy
    {
        public int StrategyID { get; set; }
        public int PlanID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
