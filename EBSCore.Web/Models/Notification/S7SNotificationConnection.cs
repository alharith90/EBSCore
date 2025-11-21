using System;

namespace EBSCore.Web.Models
{
    public class S7SNotificationConnection
    {
        public int? NotificationConnectionID { get; set; }
        public int? ChannelID { get; set; }
        public string? Name { get; set; }
        public string? ProviderType { get; set; }
        public string? ConfigurationJson { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
        public int? CompanyID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
