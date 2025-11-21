namespace EBSCore.Web.Models.Notification
{
    public class S7SNotificationStatus
    {
        public int? S7SNotificationStatusID { get; set; }
        public int NotificationTemplateID { get; set; }
        public string? Email { get; set; }
        public string? CCEmails { get; set; }
        public string? BCCEmails { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNo { get; set; }
        public int ChannelID { get; set; }
        public int? ConnectionID { get; set; }
        public int? ToUserID { get; set; }
        public bool Sent { get; set; }
        public bool Success { get; set; }
        public bool TryAgain { get; set; }
        public DateTime? TryDate { get; set; }
        public DateTime? LastTryDate { get; set; }
        public int NoOfTry { get; set; }
        public int MaxTry { get; set; }
        public int Priority { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? ExceptionID { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorStack { get; set; }
        public string? PayloadJson { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class S7SNotificationTemplate
    {
        public int? NotificationTemplateID { get; set; }
        public string? TemplateKey { get; set; }
        public string? Name { get; set; }
        public int ChannelID { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool UseDesign { get; set; }
        public string? Attachments { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class S7SNotificationChannel
    {
        public int NotificationChannelID { get; set; }
        public string? ChannelCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class S7SNotificationConnection
    {
        public int? NotificationConnectionID { get; set; }
        public int ChannelID { get; set; }
        public string? Name { get; set; }
        public string? ProviderType { get; set; }
        public string? ConfigurationJson { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class S7SNotificationHistory
    {
        public int? NotificationHistoryID { get; set; }
        public int NotificationStatusID { get; set; }
        public string? StatusBeforeJson { get; set; }
        public string? StatusAfterJson { get; set; }
        public string? ChangeType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
