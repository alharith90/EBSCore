USE [EBS]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    Schema: Notification Engine
    Notes: All tables start with S7S* prefix and are designed for a durable, channel-agnostic notification pipeline
*/

-- Exchanges represent routing contracts for notification flows (topic/direct style)
CREATE TABLE [dbo].[S7SNotificationExchange](
    [ExchangeID] [int] IDENTITY(1,1) NOT NULL,
    [ExchangeCode] [nvarchar](100) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [ExchangeType] [nvarchar](50) NOT NULL, -- Direct | Topic | Fanout | DeadLetter
    [RoutingKey] [nvarchar](200) NULL,
    [DeadLetterExchangeID] [int] NULL,
    [IsDurable] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationExchange_IsDurable] DEFAULT(1),
    [RetryPolicyJson] [nvarchar](max) NULL,
    [MetadataJson] [nvarchar](max) NULL,
    [IsActive] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationExchange_IsActive] DEFAULT(1),
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationExchange_IsDeleted] DEFAULT(0),
    [CreatedBy] [nvarchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationExchange_CreatedAt] DEFAULT(sysutcdatetime()),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_S7SNotificationExchange] PRIMARY KEY CLUSTERED ([ExchangeID] ASC)
);
GO

-- Channels describe provider connections (Email/SMS/WhatsApp/Webhook/etc.)
CREATE TABLE [dbo].[S7SNotificationChannel](
    [ChannelID] [int] IDENTITY(1,1) NOT NULL,
    [ChannelName] [nvarchar](200) NOT NULL,
    [ChannelType] [nvarchar](50) NOT NULL, -- Email | SMS | WhatsApp | Webhook | Push
    [ExchangeID] [int] NULL,
    [CredentialID] [int] NULL,
    [Endpoint] [nvarchar](500) NULL,
    [FromIdentity] [nvarchar](200) NULL, -- sender email/number/origin
    [ConfigurationJson] [nvarchar](max) NULL,
    [MaxRetryCount] [int] NOT NULL CONSTRAINT [DF_S7SNotificationChannel_MaxRetry] DEFAULT(5),
    [BackoffSeconds] [int] NOT NULL CONSTRAINT [DF_S7SNotificationChannel_Backoff] DEFAULT(30),
    [RateLimitPerMinute] [int] NULL,
    [IsEnabled] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationChannel_IsEnabled] DEFAULT(1),
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationChannel_IsDeleted] DEFAULT(0),
    [CreatedBy] [nvarchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationChannel_CreatedAt] DEFAULT(sysutcdatetime()),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_S7SNotificationChannel] PRIMARY KEY CLUSTERED ([ChannelID] ASC)
);
GO

-- Explicit connection/endpoints per channel instance
CREATE TABLE [dbo].[S7SNotificationChannelConnection](
    [ConnectionID] [int] IDENTITY(1,1) NOT NULL,
    [ChannelID] [int] NOT NULL,
    [ConnectionType] [nvarchar](50) NOT NULL, -- Email/SMS/WhatsApp/Webhook/etc.
    [DisplayName] [nvarchar](200) NULL,
    [Destination] [nvarchar](300) NOT NULL,
    [MetadataJson] [nvarchar](max) NULL,
    [IsPrimary] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationChannelConnection_IsPrimary] DEFAULT(0),
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationChannelConnection_IsDeleted] DEFAULT(0),
    [CreatedBy] [nvarchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationChannelConnection_CreatedAt] DEFAULT(sysutcdatetime()),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_S7SNotificationChannelConnection] PRIMARY KEY CLUSTERED ([ConnectionID] ASC)
);
GO

-- Templates per channel/language
CREATE TABLE [dbo].[S7SNotificationTemplate](
    [TemplateID] [int] IDENTITY(1,1) NOT NULL,
    [TemplateCode] [nvarchar](100) NOT NULL,
    [ChannelType] [nvarchar](50) NOT NULL,
    [Language] [nvarchar](10) NULL,
    [Subject] [nvarchar](250) NULL,
    [Body] [nvarchar](max) NOT NULL,
    [FallbackBody] [nvarchar](max) NULL,
    [IsHtml] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationTemplate_IsHtml] DEFAULT(0),
    [IsActive] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationTemplate_IsActive] DEFAULT(1),
    [CreatedBy] [nvarchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationTemplate_CreatedAt] DEFAULT(sysutcdatetime()),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime2](7) NULL,
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationTemplate_IsDeleted] DEFAULT(0),
 CONSTRAINT [PK_S7SNotificationTemplate] PRIMARY KEY CLUSTERED ([TemplateID] ASC)
);
GO

-- Core notification request
CREATE TABLE [dbo].[S7SNotification](
    [NotificationID] [bigint] IDENTITY(1,1) NOT NULL,
    [ExchangeID] [int] NULL,
    [CorrelationID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_S7SNotification_CorrelationID] DEFAULT(newid()),
    [Title] [nvarchar](200) NULL,
    [Priority] [int] NOT NULL CONSTRAINT [DF_S7SNotification_Priority] DEFAULT(0),
    [PayloadJson] [nvarchar](max) NULL,
    [Status] [nvarchar](50) NOT NULL CONSTRAINT [DF_S7SNotification_Status] DEFAULT('Pending'),
    [ScheduledAt] [datetime2](7) NULL,
    [ExpiresAt] [datetime2](7) NULL,
    [CreatedBy] [nvarchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotification_CreatedAt] DEFAULT(sysutcdatetime()),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime2](7) NULL,
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotification_IsDeleted] DEFAULT(0),
 CONSTRAINT [PK_S7SNotification] PRIMARY KEY CLUSTERED ([NotificationID] ASC)
);
GO

-- Recipients per notification
CREATE TABLE [dbo].[S7SNotificationRecipient](
    [RecipientID] [bigint] IDENTITY(1,1) NOT NULL,
    [NotificationID] [bigint] NOT NULL,
    [ChannelType] [nvarchar](50) NOT NULL,
    [TargetAddress] [nvarchar](300) NOT NULL,
    [DisplayName] [nvarchar](200) NULL,
    [MetadataJson] [nvarchar](max) NULL,
    [IsDeleted] [bit] NOT NULL CONSTRAINT [DF_S7SNotificationRecipient_IsDeleted] DEFAULT(0),
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationRecipient_CreatedAt] DEFAULT(sysutcdatetime()),
 CONSTRAINT [PK_S7SNotificationRecipient] PRIMARY KEY CLUSTERED ([RecipientID] ASC)
);
GO

-- Outbox entries drive reliable dispatch
CREATE TABLE [dbo].[S7SNotificationOutbox](
    [OutboxID] [bigint] IDENTITY(1,1) NOT NULL,
    [NotificationID] [bigint] NOT NULL,
    [RecipientID] [bigint] NOT NULL,
    [ChannelID] [int] NULL,
    [ChannelType] [nvarchar](50) NOT NULL,
    [TemplateID] [int] NULL,
    [Subject] [nvarchar](250) NULL,
    [Body] [nvarchar](max) NULL,
    [PayloadJson] [nvarchar](max) NULL,
    [TargetAddress] [nvarchar](300) NULL,
    [Status] [nvarchar](50) NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_Status] DEFAULT('Pending'),
    [Priority] [int] NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_Priority] DEFAULT(0),
    [RetryCount] [int] NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_RetryCount] DEFAULT(0),
    [MaxRetryCount] [int] NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_MaxRetry] DEFAULT(5),
    [BackoffSeconds] [int] NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_Backoff] DEFAULT(30),
    [NextAttemptAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationOutbox_NextAttemptAt] DEFAULT(sysutcdatetime()),
    [LockedUntil] [datetime2](7) NULL,
    [DispatchStartedAt] [datetime2](7) NULL,
    [SentAt] [datetime2](7) NULL,
    [LastError] [nvarchar](max) NULL,
    [ResponseJson] [nvarchar](max) NULL,
    [DeliveryReportJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_S7SNotificationOutbox] PRIMARY KEY CLUSTERED ([OutboxID] ASC)
);
GO

-- Audit trail per attempt
CREATE TABLE [dbo].[S7SNotificationAudit](
    [AuditID] [bigint] IDENTITY(1,1) NOT NULL,
    [OutboxID] [bigint] NOT NULL,
    [Status] [nvarchar](50) NOT NULL,
    [Message] [nvarchar](max) NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationAudit_CreatedAt] DEFAULT(sysutcdatetime()),
 CONSTRAINT [PK_S7SNotificationAudit] PRIMARY KEY CLUSTERED ([AuditID] ASC)
);
GO

-- Attachments stored as references for multi-channel use
CREATE TABLE [dbo].[S7SNotificationAttachment](
    [AttachmentID] [bigint] IDENTITY(1,1) NOT NULL,
    [NotificationID] [bigint] NOT NULL,
    [FileName] [nvarchar](260) NOT NULL,
    [ContentType] [nvarchar](100) NULL,
    [StorageUri] [nvarchar](500) NOT NULL,
    [FileSizeBytes] [bigint] NULL,
    [CreatedAt] [datetime2](7) NOT NULL CONSTRAINT [DF_S7SNotificationAttachment_CreatedAt] DEFAULT(sysutcdatetime()),
 CONSTRAINT [PK_S7SNotificationAttachment] PRIMARY KEY CLUSTERED ([AttachmentID] ASC)
);
GO

-- Relationships
ALTER TABLE [dbo].[S7SNotificationExchange]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationExchange_DeadLetter] FOREIGN KEY([DeadLetterExchangeID])
REFERENCES [dbo].[S7SNotificationExchange] ([ExchangeID]);
GO
ALTER TABLE [dbo].[S7SNotificationChannel]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationChannel_Exchange] FOREIGN KEY([ExchangeID])
REFERENCES [dbo].[S7SNotificationExchange] ([ExchangeID]);
GO
ALTER TABLE [dbo].[S7SNotificationChannelConnection]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationChannelConnection_Channel] FOREIGN KEY([ChannelID])
REFERENCES [dbo].[S7SNotificationChannel] ([ChannelID]);
GO
ALTER TABLE [dbo].[S7SNotification]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotification_Exchange] FOREIGN KEY([ExchangeID])
REFERENCES [dbo].[S7SNotificationExchange] ([ExchangeID]);
GO
ALTER TABLE [dbo].[S7SNotificationRecipient]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationRecipient_Notification] FOREIGN KEY([NotificationID])
REFERENCES [dbo].[S7SNotification] ([NotificationID]);
GO
ALTER TABLE [dbo].[S7SNotificationOutbox]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationOutbox_Notification] FOREIGN KEY([NotificationID])
REFERENCES [dbo].[S7SNotification] ([NotificationID]);
GO
ALTER TABLE [dbo].[S7SNotificationOutbox]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationOutbox_Recipient] FOREIGN KEY([RecipientID])
REFERENCES [dbo].[S7SNotificationRecipient] ([RecipientID]);
GO
ALTER TABLE [dbo].[S7SNotificationOutbox]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationOutbox_Template] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[S7SNotificationTemplate] ([TemplateID]);
GO
ALTER TABLE [dbo].[S7SNotificationOutbox]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationOutbox_Channel] FOREIGN KEY([ChannelID])
REFERENCES [dbo].[S7SNotificationChannel] ([ChannelID]);
GO
ALTER TABLE [dbo].[S7SNotificationAudit]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationAudit_Outbox] FOREIGN KEY([OutboxID])
REFERENCES [dbo].[S7SNotificationOutbox] ([OutboxID]);
GO
ALTER TABLE [dbo].[S7SNotificationAttachment]  WITH CHECK ADD  CONSTRAINT [FK_S7SNotificationAttachment_Notification] FOREIGN KEY([NotificationID])
REFERENCES [dbo].[S7SNotification] ([NotificationID]);
GO

-- Indexes for fast lookup and reliability
CREATE UNIQUE NONCLUSTERED INDEX [IX_S7SNotificationExchange_Code] ON [dbo].[S7SNotificationExchange]([ExchangeCode]);
CREATE NONCLUSTERED INDEX [IX_S7SNotification_ChannelType] ON [dbo].[S7SNotificationChannel]([ChannelType]) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX [IX_S7SNotificationRecipient_Notification] ON [dbo].[S7SNotificationRecipient]([NotificationID]);
CREATE NONCLUSTERED INDEX [IX_S7SNotificationOutbox_NextAttempt] ON [dbo].[S7SNotificationOutbox]([Status],[NextAttemptAt]) INCLUDE([Priority],[ChannelType],[RetryCount]);
GO

-- Dispatch stored procedure used by background worker
CREATE OR ALTER PROCEDURE [dbo].[S7SNotificationDispatchSP]
    @Operation [nvarchar](50),
    @OutboxID [bigint] = NULL,
    @Status [nvarchar](50) = NULL,
    @ErrorMessage [nvarchar](max) = NULL,
    @ResponseJson [nvarchar](max) = NULL,
    @LockForSeconds [int] = 30
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'DequeueOutbox'
    BEGIN
        ;WITH next_item AS (
            SELECT TOP (1) o.[OutboxID]
            FROM [dbo].[S7SNotificationOutbox] o WITH (ROWLOCK, READPAST, UPDLOCK)
            WHERE o.[Status] IN ('Pending','Retrying')
                AND o.[NextAttemptAt] <= sysutcdatetime()
                AND (o.[LockedUntil] IS NULL OR o.[LockedUntil] < sysutcdatetime())
            ORDER BY o.[Priority] DESC, o.[NextAttemptAt], o.[OutboxID]
        )
        UPDATE o
            SET o.[Status] = 'Dispatching',
                o.[LockedUntil] = DATEADD(second, ISNULL(@LockForSeconds, 30), sysutcdatetime()),
                o.[DispatchStartedAt] = sysutcdatetime()
        OUTPUT inserted.[OutboxID], inserted.[NotificationID], inserted.[RecipientID], inserted.[ChannelID], inserted.[ChannelType],
               inserted.[TemplateID], inserted.[Subject], inserted.[Body], inserted.[PayloadJson], inserted.[TargetAddress], inserted.[RetryCount],
               inserted.[MaxRetryCount], inserted.[BackoffSeconds], inserted.[Priority], inserted.[NextAttemptAt]
        FROM [dbo].[S7SNotificationOutbox] o
        INNER JOIN next_item n ON n.[OutboxID] = o.[OutboxID];
        RETURN;
    END

    IF @Operation = 'CompleteOutbox'
    BEGIN
        UPDATE o
            SET o.[Status] = ISNULL(@Status, 'Succeeded'),
                o.[SentAt] = sysutcdatetime(),
                o.[LockedUntil] = NULL,
                o.[ResponseJson] = ISNULL(@ResponseJson, o.[ResponseJson]),
                o.[LastError] = NULL
        OUTPUT inserted.[OutboxID], inserted.[Status], inserted.[SentAt]
        FROM [dbo].[S7SNotificationOutbox] o
        WHERE o.[OutboxID] = @OutboxID;

        INSERT INTO [dbo].[S7SNotificationAudit]([OutboxID],[Status],[Message])
        VALUES(@OutboxID, 'Succeeded', 'Notification marked as sent');
        RETURN;
    END

    IF @Operation = 'FailOutbox'
    BEGIN
        UPDATE o
            SET o.[RetryCount] = o.[RetryCount] + 1,
                o.[LastError] = @ErrorMessage,
                o.[LockedUntil] = NULL,
                o.[NextAttemptAt] = CASE WHEN o.[RetryCount] + 1 >= o.[MaxRetryCount]
                                          THEN o.[NextAttemptAt]
                                          ELSE DATEADD(second, POWER(2, o.[RetryCount]) * o.[BackoffSeconds], sysutcdatetime()) END,
                o.[Status] = CASE WHEN o.[RetryCount] + 1 >= o.[MaxRetryCount] THEN 'Failed' ELSE 'Retrying' END
        OUTPUT inserted.[OutboxID], inserted.[Status], inserted.[RetryCount], inserted.[NextAttemptAt]
        FROM [dbo].[S7SNotificationOutbox] o
        WHERE o.[OutboxID] = @OutboxID;

        INSERT INTO [dbo].[S7SNotificationAudit]([OutboxID],[Status],[Message])
        VALUES(@OutboxID, 'Failed', @ErrorMessage);
        RETURN;
    END
END
GO
