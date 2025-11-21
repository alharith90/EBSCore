using System;

namespace EBSCore.Web.Models
{
    public class S7SNotificationStatus
    {
        public int? NotificationStatusID { get; set; }
        public int? NotificationTemplateID { get; set; }
        public string? Email { get; set; }
        public string? CCEmails { get; set; }
        public string? BCCEmails { get; set; }
        public int? CountryCode { get; set; }
        public string? MobileNo { get; set; }
        public int? ToUserID { get; set; }
        public int? ChannelID { get; set; }
        public int? ConnectionID { get; set; }
        public bool? Sent { get; set; }
        public bool? Success { get; set; }
        public bool? TryAgain { get; set; }
        public DateTime? TryDate { get; set; }
        public DateTime? LastTryDate { get; set; }
        public int? NoOfTry { get; set; }
        public int? MaxTry { get; set; }
        public int? Priority { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? ExceptionID { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorStack { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
