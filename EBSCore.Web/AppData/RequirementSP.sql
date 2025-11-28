USE [EBS]
GO

IF OBJECT_ID('[dbo].[Requirement]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Requirement]
    (
        [RequirementCode] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RequirementNo] NVARCHAR(100) NOT NULL,
        [RequirementType] NVARCHAR(100) NOT NULL,
        [RequirementTitle] NVARCHAR(500) NOT NULL,
        [RequirementDescription] NVARCHAR(MAX) NULL,
        [Subcategory] NVARCHAR(250) NULL,
        [RequirementDetails] NVARCHAR(MAX) NULL,
        [RequirementTags] NVARCHAR(500) NULL,
        [RequirementFrequency] NVARCHAR(50) NULL,
        [ExternalAudit] NVARCHAR(10) NULL,
        [InternalAudit] NVARCHAR(10) NULL,
        [AuditReference] NVARCHAR(250) NULL,
        [RiskCategory] NVARCHAR(100) NULL,
        [ControlOwner] NVARCHAR(250) NULL,
        [ControlOwnerFunction] NVARCHAR(250) NULL,
        [EvidenceRequired] NVARCHAR(250) NULL,
        [EvidenceDetails] NVARCHAR(500) NULL,
        [ControlID] NVARCHAR(100) NULL,
        [OrganizationUnitID] BIGINT NOT NULL,
        [EscalationProcess] NVARCHAR(250) NOT NULL,
        [EscalationThreshold] NVARCHAR(500) NULL,
        [BCMActivationType] NVARCHAR(250) NOT NULL,
        [BCMActivationDecision] NVARCHAR(250) NULL,
        [EscalationContacts] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [CompanyID] INT NOT NULL,
        [CreatedBy] BIGINT NULL,
        [ModifiedBy] BIGINT NULL,
        [CreatedAt] DATETIME NULL,
        [UpdatedAt] DATETIME NULL
    );

    CREATE UNIQUE INDEX IX_Requirement_Company_RequirementNo ON [dbo].[Requirement]([CompanyID], [RequirementNo]);
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[RequirementSP]
    @Operation                  NVARCHAR(100) = NULL,
    @UserID                     BIGINT = NULL,
    @CompanyID                  INT = NULL,
    @RequirementCode            INT = NULL,
    @RequirementNo              NVARCHAR(100) = NULL,
    @RequirementType            NVARCHAR(100) = NULL,
    @RequirementTitle           NVARCHAR(500) = NULL,
    @RequirementDescription     NVARCHAR(MAX) = NULL,
    @Subcategory                NVARCHAR(250) = NULL,
    @RequirementDetails         NVARCHAR(MAX) = NULL,
    @RequirementTags            NVARCHAR(500) = NULL,
    @RequirementFrequency       NVARCHAR(50) = NULL,
    @ExternalAudit              NVARCHAR(10) = NULL,
    @InternalAudit              NVARCHAR(10) = NULL,
    @AuditReference             NVARCHAR(250) = NULL,
    @RiskCategory               NVARCHAR(100) = NULL,
    @ControlOwner               NVARCHAR(250) = NULL,
    @ControlOwnerFunction       NVARCHAR(250) = NULL,
    @EvidenceRequired           NVARCHAR(250) = NULL,
    @EvidenceDetails            NVARCHAR(500) = NULL,
    @ControlID                  NVARCHAR(100) = NULL,
    @OrganizationUnitID         BIGINT = NULL,
    @EscalationProcess          NVARCHAR(250) = NULL,
    @EscalationThreshold        NVARCHAR(500) = NULL,
    @BCMActivationType          NVARCHAR(250) = NULL,
    @BCMActivationDecision      NVARCHAR(250) = NULL,
    @EscalationContacts         NVARCHAR(MAX) = NULL,
    @Status                     NVARCHAR(50) = NULL,
    @CreatedBy                  BIGINT = NULL,
    @ModifiedBy                 BIGINT = NULL,
    @CreatedAt                  DATETIME = NULL,
    @UpdatedAt                  DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @CurrentCompanyID <> @CompanyID
        THROW 51000, 'Invalid company scope', 1;

    IF @Operation = 'SaveRequirement'
    BEGIN
        IF EXISTS(SELECT 1 FROM Requirement WHERE RequirementCode = @RequirementCode AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'Requirement', @RequirementCode,
                    (SELECT * FROM Requirement WHERE RequirementCode = @RequirementCode FOR XML AUTO), GETDATE());

            UPDATE [dbo].[Requirement]
            SET
                [RequirementNo] = @RequirementNo,
                [RequirementType] = @RequirementType,
                [RequirementTitle] = @RequirementTitle,
                [RequirementDescription] = @RequirementDescription,
                [Subcategory] = @Subcategory,
                [RequirementDetails] = @RequirementDetails,
                [RequirementTags] = @RequirementTags,
                [RequirementFrequency] = @RequirementFrequency,
                [ExternalAudit] = @ExternalAudit,
                [InternalAudit] = @InternalAudit,
                [AuditReference] = @AuditReference,
                [RiskCategory] = @RiskCategory,
                [ControlOwner] = @ControlOwner,
                [ControlOwnerFunction] = @ControlOwnerFunction,
                [EvidenceRequired] = @EvidenceRequired,
                [EvidenceDetails] = @EvidenceDetails,
                [ControlID] = @ControlID,
                [OrganizationUnitID] = @OrganizationUnitID,
                [EscalationProcess] = @EscalationProcess,
                [EscalationThreshold] = @EscalationThreshold,
                [BCMActivationType] = @BCMActivationType,
                [BCMActivationDecision] = @BCMActivationDecision,
                [EscalationContacts] = @EscalationContacts,
                [Status] = @Status,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE RequirementCode = @RequirementCode
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[Requirement]
            (
                [RequirementNo],
                [RequirementType],
                [RequirementTitle],
                [RequirementDescription],
                [Subcategory],
                [RequirementDetails],
                [RequirementTags],
                [RequirementFrequency],
                [ExternalAudit],
                [InternalAudit],
                [AuditReference],
                [RiskCategory],
                [ControlOwner],
                [ControlOwnerFunction],
                [EvidenceRequired],
                [EvidenceDetails],
                [ControlID],
                [OrganizationUnitID],
                [EscalationProcess],
                [EscalationThreshold],
                [BCMActivationType],
                [BCMActivationDecision],
                [EscalationContacts],
                [Status],
                [CompanyID],
                [CreatedBy],
                [CreatedAt],
                [UpdatedAt]
            )
            VALUES
            (
                @RequirementNo,
                @RequirementType,
                @RequirementTitle,
                @RequirementDescription,
                @Subcategory,
                @RequirementDetails,
                @RequirementTags,
                @RequirementFrequency,
                @ExternalAudit,
                @InternalAudit,
                @AuditReference,
                @RiskCategory,
                @ControlOwner,
                @ControlOwnerFunction,
                @EvidenceRequired,
                @EvidenceDetails,
                @ControlID,
                @OrganizationUnitID,
                @EscalationProcess,
                @EscalationThreshold,
                @BCMActivationType,
                @BCMActivationDecision,
                @EscalationContacts,
                @Status,
                @CompanyID,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            );

            DECLARE @NewRequirementCode INT = SCOPE_IDENTITY();
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'Requirement', @NewRequirementCode,
                    (SELECT * FROM Requirement WHERE RequirementCode = @NewRequirementCode FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvRequirements'
    BEGIN
        SELECT *
        FROM [Requirement]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Requirement', NULL,
                (SELECT * FROM Requirement WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvRequirementsByUnit'
    BEGIN
        SELECT *
        FROM [Requirement]
        WHERE CompanyID = @CompanyID
          AND (@OrganizationUnitID IS NULL OR OrganizationUnitID = @OrganizationUnitID);

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select By Unit', 'Requirement', NULL,
                (SELECT * FROM Requirement WHERE CompanyID = @CompanyID AND (@OrganizationUnitID IS NULL OR OrganizationUnitID = @OrganizationUnitID) FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvRequirement'
    BEGIN
        SELECT *
        FROM [Requirement]
        WHERE RequirementCode = @RequirementCode
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'Requirement', @RequirementCode,
                (SELECT * FROM Requirement WHERE RequirementCode = @RequirementCode FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteRequirement'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'Requirement', @RequirementCode,
                (SELECT * FROM Requirement WHERE RequirementCode = @RequirementCode FOR XML AUTO), GETDATE());

        DELETE FROM [Requirement]
        WHERE RequirementCode = @RequirementCode
          AND CompanyID = @CompanyID;
    END
END
GO
