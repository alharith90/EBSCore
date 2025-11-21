namespace EBSCore.Web.Models
{
    public class S7SNotificationTemplate
    {
        public int? NotificationTemplateID { get; set; }
        public string? TemplateKey { get; set; }
        public string? Name { get; set; }
        public int? ChannelID { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool? UseDesign { get; set; }
        public string? Attachments { get; set; }
        public string? Description { get; set; }
        public int? CompanyID { get; set; }
        public bool? IsActive { get; set; }
    }
}
