using System;

namespace EBSCore.Web.Models
{
    public class S7SNotificationChannel
    {
        public int? NotificationChannelID { get; set; }
        public string? ChannelCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
