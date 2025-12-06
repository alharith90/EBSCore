/****** Template reference: patterned after ProcessSP for permissions/history ******/
IF OBJECT_ID('dbo.RiskTreatmentPlan', 'U') IS NOT NULL
    DROP TABLE dbo.RiskTreatmentPlan;
GO

CREATE TABLE [dbo].[RiskTreatmentPlan]
(
    [ActionID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [RelatedRisk] NVARCHAR(500) NOT NULL,
    [MitigationAction] NVARCHAR(MAX) NOT NULL,
    [ActionOwner] NVARCHAR(200) NULL,
    [DueDate] DATETIME NULL,
    [CompletionStatus] NVARCHAR(100) NULL,
    [TreatmentType] NVARCHAR(150) NULL,
    [AssociatedControl] NVARCHAR(200) NULL,
    [ProgressNotes] NVARCHAR(MAX) NULL,
    [Verification] NVARCHAR(200) NULL,
    [Effectiveness] NVARCHAR(100) NULL,
    [CreatedBy] BIGINT NULL,
    [ModifiedBy] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RiskTreatmentPlan_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RiskTreatmentPlan_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[RiskTreatmentPlanSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ActionID INT = NULL,
    @RelatedRisk NVARCHAR(500) = NULL,
    @MitigationAction NVARCHAR(MAX) = NULL,
    @ActionOwner NVARCHAR(200) = NULL,
    @DueDate DATETIME = NULL,
    @CompletionStatus NVARCHAR(100) = NULL,
    @TreatmentType NVARCHAR(150) = NULL,
    @AssociatedControl NVARCHAR(200) = NULL,
    @ProgressNotes NVARCHAR(MAX) = NULL,
    @Verification NVARCHAR(200) = NULL,
    @Effectiveness NVARCHAR(100) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SavePlan'
    BEGIN
        IF EXISTS(SELECT 1 FROM RiskTreatmentPlan WHERE ActionID = @ActionID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'RiskTreatmentPlan', @ActionID,
                    (SELECT * FROM RiskTreatmentPlan WHERE ActionID = @ActionID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[RiskTreatmentPlan]
            SET [RelatedRisk] = @RelatedRisk,
                [MitigationAction] = @MitigationAction,
                [ActionOwner] = @ActionOwner,
                [DueDate] = @DueDate,
                [CompletionStatus] = @CompletionStatus,
                [TreatmentType] = @TreatmentType,
                [AssociatedControl] = @AssociatedControl,
                [ProgressNotes] = @ProgressNotes,
                [Verification] = @Verification,
                [Effectiveness] = @Effectiveness,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE ActionID = @ActionID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[RiskTreatmentPlan]
                ([CompanyID], [RelatedRisk], [MitigationAction], [ActionOwner], [DueDate], [CompletionStatus], [TreatmentType], [AssociatedControl], [ProgressNotes], [Verification], [Effectiveness], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @RelatedRisk, @MitigationAction, @ActionOwner, @DueDate, @CompletionStatus, @TreatmentType, @AssociatedControl, @ProgressNotes, @Verification, @Effectiveness, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewActionID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'RiskTreatmentPlan', @NewActionID,
                    (SELECT * FROM RiskTreatmentPlan WHERE ActionID = @NewActionID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvPlans'
    BEGIN
        SELECT
            ActionID,
            CompanyID,
            RelatedRisk,
            MitigationAction,
            ActionOwner,
            DueDate,
            CompletionStatus,
            TreatmentType,
            AssociatedControl,
            ProgressNotes,
            Verification,
            Effectiveness,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [RiskTreatmentPlan]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RiskTreatmentPlan', NULL,
                (SELECT * FROM RiskTreatmentPlan WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvPlan'
    BEGIN
        SELECT
            ActionID,
            CompanyID,
            RelatedRisk,
            MitigationAction,
            ActionOwner,
            DueDate,
            CompletionStatus,
            TreatmentType,
            AssociatedControl,
            ProgressNotes,
            Verification,
            Effectiveness,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [RiskTreatmentPlan]
        WHERE ActionID = @ActionID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'RiskTreatmentPlan', @ActionID,
                (SELECT * FROM RiskTreatmentPlan WHERE ActionID = @ActionID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeletePlan'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'RiskTreatmentPlan', @ActionID,
                (SELECT * FROM RiskTreatmentPlan WHERE ActionID = @ActionID FOR XML AUTO), GETDATE());

        DELETE FROM [RiskTreatmentPlan]
        WHERE ActionID = @ActionID
          AND CompanyID = @CompanyID;
    END
END
GO
