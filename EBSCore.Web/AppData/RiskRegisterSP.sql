/****** Template reference: modeled after ProcessSP permission and history flow ******/
IF OBJECT_ID('dbo.RiskRegister', 'U') IS NOT NULL
    DROP TABLE dbo.RiskRegister;
GO

CREATE TABLE [dbo].[RiskRegister]
(
    [RiskID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [RiskTitle] NVARCHAR(250) NOT NULL,
    [RiskDescription] NVARCHAR(MAX) NOT NULL,
    [RiskCategory] NVARCHAR(200) NULL,
    [RiskSource] NVARCHAR(MAX) NULL,
    [PotentialImpact] NVARCHAR(MAX) NULL,
    [InherentLikelihood] NVARCHAR(50) NULL,
    [InherentImpact] NVARCHAR(50) NULL,
    [InherentRiskLevel] NVARCHAR(50) NULL,
    [ExistingControls] NVARCHAR(MAX) NULL,
    [ControlEffectiveness] NVARCHAR(50) NULL,
    [ResidualLikelihood] NVARCHAR(50) NULL,
    [ResidualImpact] NVARCHAR(50) NULL,
    [ResidualRiskLevel] NVARCHAR(50) NULL,
    [RiskAppetiteThreshold] NVARCHAR(100) NULL,
    [RiskResponseStrategy] NVARCHAR(100) NULL,
    [TreatmentDecision] NVARCHAR(100) NULL,
    [TreatmentPlanID] INT NULL,
    [Likelihood] NVARCHAR(50) NULL,
    [Impact] NVARCHAR(50) NULL,
    [RiskScore] INT NULL,
    [RiskResponse] NVARCHAR(200) NULL,
    [RiskOwner] NVARCHAR(200) NULL,
    [Status] NVARCHAR(50) NULL,
    [ReviewDate] DATETIME NULL,
    [NextReviewDate] DATETIME NULL,
    [RiskTrend] NVARCHAR(50) NULL,
    [RelatedObjectives] NVARCHAR(MAX) NULL,
    [RelatedIncidents] NVARCHAR(MAX) NULL,
    [RelatedControls] NVARCHAR(MAX) NULL,
    [RelatedObligations] NVARCHAR(MAX) NULL,
    [MonitoringFrequency] NVARCHAR(50) NULL,
    [LastMonitoringDate] DATETIME NULL,
    [KRIName] NVARCHAR(200) NULL,
    [KRIValue] NVARCHAR(100) NULL,
    [KRIStatus] NVARCHAR(50) NULL,
    [RiskAlertTrigger] NVARCHAR(200) NULL,
    [NextMonitoringDate] DATETIME NULL,
    [RiskHistoryNotes] NVARCHAR(MAX) NULL,
    [LastUpdatedBy] NVARCHAR(250) NULL,
    [CreatedBy] BIGINT NULL,
    [ModifiedBy] BIGINT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RiskRegister_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_RiskRegister_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[RiskRegisterSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @RiskID INT = NULL,
    @RiskTitle NVARCHAR(250) = NULL,
    @RiskDescription NVARCHAR(MAX) = NULL,
    @RiskCategory NVARCHAR(200) = NULL,
    @RiskSource NVARCHAR(MAX) = NULL,
    @PotentialImpact NVARCHAR(MAX) = NULL,
    @InherentLikelihood NVARCHAR(50) = NULL,
    @InherentImpact NVARCHAR(50) = NULL,
    @InherentRiskLevel NVARCHAR(50) = NULL,
    @ExistingControls NVARCHAR(MAX) = NULL,
    @ControlEffectiveness NVARCHAR(50) = NULL,
    @ResidualLikelihood NVARCHAR(50) = NULL,
    @ResidualImpact NVARCHAR(50) = NULL,
    @ResidualRiskLevel NVARCHAR(50) = NULL,
    @RiskAppetiteThreshold NVARCHAR(100) = NULL,
    @RiskResponseStrategy NVARCHAR(100) = NULL,
    @TreatmentDecision NVARCHAR(100) = NULL,
    @TreatmentPlanID INT = NULL,
    @Likelihood NVARCHAR(50) = NULL,
    @Impact NVARCHAR(50) = NULL,
    @RiskScore INT = NULL,
    @RiskResponse NVARCHAR(200) = NULL,
    @RiskOwner NVARCHAR(200) = NULL,
    @Status NVARCHAR(50) = NULL,
    @ReviewDate DATETIME = NULL,
    @NextReviewDate DATETIME = NULL,
    @RiskTrend NVARCHAR(50) = NULL,
    @RelatedObjectives NVARCHAR(MAX) = NULL,
    @RelatedIncidents NVARCHAR(MAX) = NULL,
    @RelatedControls NVARCHAR(MAX) = NULL,
    @RelatedObligations NVARCHAR(MAX) = NULL,
    @MonitoringFrequency NVARCHAR(50) = NULL,
    @LastMonitoringDate DATETIME = NULL,
    @KRIName NVARCHAR(200) = NULL,
    @KRIValue NVARCHAR(100) = NULL,
    @KRIStatus NVARCHAR(50) = NULL,
    @RiskAlertTrigger NVARCHAR(200) = NULL,
    @NextMonitoringDate DATETIME = NULL,
    @RiskHistoryNotes NVARCHAR(MAX) = NULL,
    @LastUpdatedBy NVARCHAR(250) = NULL,
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

    IF @Operation = 'SaveRisk'
    BEGIN
        IF EXISTS(SELECT 1 FROM RiskRegister WHERE RiskID = @RiskID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'RiskRegister', @RiskID,
                    (SELECT * FROM RiskRegister WHERE RiskID = @RiskID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[RiskRegister]
            SET [RiskTitle] = @RiskTitle,
                [RiskDescription] = @RiskDescription,
                [RiskCategory] = @RiskCategory,
                [RiskSource] = @RiskSource,
                [PotentialImpact] = @PotentialImpact,
                [InherentLikelihood] = @InherentLikelihood,
                [InherentImpact] = @InherentImpact,
                [InherentRiskLevel] = @InherentRiskLevel,
                [ExistingControls] = @ExistingControls,
                [ControlEffectiveness] = @ControlEffectiveness,
                [ResidualLikelihood] = @ResidualLikelihood,
                [ResidualImpact] = @ResidualImpact,
                [ResidualRiskLevel] = @ResidualRiskLevel,
                [RiskAppetiteThreshold] = @RiskAppetiteThreshold,
                [RiskResponseStrategy] = @RiskResponseStrategy,
                [TreatmentDecision] = @TreatmentDecision,
                [TreatmentPlanID] = @TreatmentPlanID,
                [Likelihood] = @Likelihood,
                [Impact] = @Impact,
                [RiskScore] = @RiskScore,
                [RiskResponse] = @RiskResponse,
                [RiskOwner] = @RiskOwner,
                [Status] = @Status,
                [ReviewDate] = @ReviewDate,
                [NextReviewDate] = @NextReviewDate,
                [RiskTrend] = @RiskTrend,
                [RelatedObjectives] = @RelatedObjectives,
                [RelatedIncidents] = @RelatedIncidents,
                [RelatedControls] = @RelatedControls,
                [RelatedObligations] = @RelatedObligations,
                [MonitoringFrequency] = @MonitoringFrequency,
                [LastMonitoringDate] = @LastMonitoringDate,
                [KRIName] = @KRIName,
                [KRIValue] = @KRIValue,
                [KRIStatus] = @KRIStatus,
                [RiskAlertTrigger] = @RiskAlertTrigger,
                [NextMonitoringDate] = @NextMonitoringDate,
                [RiskHistoryNotes] = @RiskHistoryNotes,
                [LastUpdatedBy] = @LastUpdatedBy,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE RiskID = @RiskID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[RiskRegister]
                ([CompanyID], [RiskTitle], [RiskDescription], [RiskCategory], [RiskSource], [PotentialImpact], [InherentLikelihood], [InherentImpact], [InherentRiskLevel], [ExistingControls], [ControlEffectiveness], [ResidualLikelihood], [ResidualImpact], [ResidualRiskLevel], [RiskAppetiteThreshold], [RiskResponseStrategy], [TreatmentDecision], [TreatmentPlanID], [Likelihood], [Impact], [RiskScore], [RiskResponse], [RiskOwner], [Status], [ReviewDate], [NextReviewDate], [RiskTrend], [RelatedObjectives], [RelatedIncidents], [RelatedControls], [RelatedObligations], [MonitoringFrequency], [LastMonitoringDate], [KRIName], [KRIValue], [KRIStatus], [RiskAlertTrigger], [NextMonitoringDate], [RiskHistoryNotes], [LastUpdatedBy], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @RiskTitle, @RiskDescription, @RiskCategory, @RiskSource, @PotentialImpact, @InherentLikelihood, @InherentImpact, @InherentRiskLevel, @ExistingControls, @ControlEffectiveness, @ResidualLikelihood, @ResidualImpact, @ResidualRiskLevel, @RiskAppetiteThreshold, @RiskResponseStrategy, @TreatmentDecision, @TreatmentPlanID, @Likelihood, @Impact, @RiskScore, @RiskResponse, @RiskOwner, @Status, @ReviewDate, @NextReviewDate, @RiskTrend, @RelatedObjectives, @RelatedIncidents, @RelatedControls, @RelatedObligations, @MonitoringFrequency, @LastMonitoringDate, @KRIName, @KRIValue, @KRIStatus, @RiskAlertTrigger, @NextMonitoringDate, @RiskHistoryNotes, @LastUpdatedBy, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewRiskID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'RiskRegister', @NewRiskID,
                    (SELECT * FROM RiskRegister WHERE RiskID = @NewRiskID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvRisks'
    BEGIN
        SELECT
            RiskID,
            CompanyID,
            RiskTitle,
            RiskDescription,
            RiskCategory,
            RiskSource,
            PotentialImpact,
            InherentLikelihood,
            InherentImpact,
            InherentRiskLevel,
            ExistingControls,
            ControlEffectiveness,
            ResidualLikelihood,
            ResidualImpact,
            ResidualRiskLevel,
            RiskAppetiteThreshold,
            RiskResponseStrategy,
            TreatmentPlanID,
            Likelihood,
            Impact,
            RiskScore,
            RiskResponse,
            RiskOwner,
            Status,
            ReviewDate,
            NextReviewDate,
            RiskTrend,
            RelatedObjectives,
            RelatedIncidents,
            RelatedControls,
            RelatedObligations,
            MonitoringFrequency,
            LastMonitoringDate,
            KRIName,
            KRIValue,
            KRIStatus,
            RiskAlertTrigger,
            NextMonitoringDate,
            RiskHistoryNotes,
            LastUpdatedBy,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [RiskRegister]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RiskRegister', NULL,
                (SELECT * FROM RiskRegister WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvRisk'
    BEGIN
        SELECT
            RiskID,
            CompanyID,
            RiskTitle,
            RiskDescription,
            RiskCategory,
            RiskSource,
            PotentialImpact,
            InherentLikelihood,
            InherentImpact,
            InherentRiskLevel,
            ExistingControls,
            ControlEffectiveness,
            ResidualLikelihood,
            ResidualImpact,
            ResidualRiskLevel,
            RiskAppetiteThreshold,
            RiskResponseStrategy,
            TreatmentPlanID,
            Likelihood,
            Impact,
            RiskScore,
            RiskResponse,
            RiskOwner,
            Status,
            ReviewDate,
            NextReviewDate,
            RiskTrend,
            RelatedObjectives,
            RelatedIncidents,
            RelatedControls,
            RelatedObligations,
            MonitoringFrequency,
            LastMonitoringDate,
            KRIName,
            KRIValue,
            KRIStatus,
            RiskAlertTrigger,
            NextMonitoringDate,
            RiskHistoryNotes,
            LastUpdatedBy,
            CreatedBy,
            ModifiedBy,
            CreatedAt,
            UpdatedAt
        FROM [RiskRegister]
        WHERE RiskID = @RiskID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'RiskRegister', @RiskID,
                (SELECT * FROM RiskRegister WHERE RiskID = @RiskID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteRisk'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'RiskRegister', @RiskID,
                (SELECT * FROM RiskRegister WHERE RiskID = @RiskID FOR XML AUTO), GETDATE());

        DELETE FROM [RiskRegister]
        WHERE RiskID = @RiskID
          AND CompanyID = @CompanyID;
    END
END
GO
