using System;

namespace EBSCore.Web.Models
{
    public class S7SNotificationHistory
    {
        public int? NotificationHistoryID { get; set; }
        public int? NotificationStatusID { get; set; }
        public string? StatusBeforeJson { get; set; }
        public string? StatusAfterJson { get; set; }
        public string? ChangeType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
