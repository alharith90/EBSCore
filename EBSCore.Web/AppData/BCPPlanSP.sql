USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[BCPPlanSP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @BCPID                  BIGINT = NULL,
    @PlanName               NVARCHAR(250) = NULL,
    @Scope                  NVARCHAR(MAX) = NULL,
    @RecoveryTeamRoles      NVARCHAR(MAX) = NULL,
    @ContactList            NVARCHAR(MAX) = NULL,
    @InvocationCriteria     NVARCHAR(MAX) = NULL,
    @RecoveryLocations      NVARCHAR(MAX) = NULL,
    @RecoveryStrategyDetails NVARCHAR(MAX) = NULL,
    @KeySteps               NVARCHAR(MAX) = NULL,
    @RequiredResources      NVARCHAR(MAX) = NULL,
    @DependentSystems       NVARCHAR(MAX) = NULL,
    @PlanRTO                NVARCHAR(100) = NULL,
    @PlanRPO                NVARCHAR(100) = NULL,
    @BackupSource           NVARCHAR(250) = NULL,
    @AlternateSupplier      NVARCHAR(250) = NULL,
    @LastTestDate           DATETIME = NULL,
    @TestResultSummary      NVARCHAR(MAX) = NULL,
    @PlanOwner              NVARCHAR(250) = NULL,
    @PlanStatusID           NVARCHAR(100) = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveBCPPlan'
    BEGIN
        IF EXISTS(SELECT 1 FROM BCPPlan WHERE BCPID = @BCPID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'BCPPlan', @BCPID,
            (SELECT * FROM BCPPlan WHERE BCPID = @BCPID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[BCPPlan]
            SET
                [PlanName] = @PlanName,
                [Scope] = @Scope,
                [RecoveryTeamRoles] = @RecoveryTeamRoles,
                [ContactList] = @ContactList,
                [InvocationCriteria] = @InvocationCriteria,
                [RecoveryLocations] = @RecoveryLocations,
                [RecoveryStrategyDetails] = @RecoveryStrategyDetails,
                [KeySteps] = @KeySteps,
                [RequiredResources] = @RequiredResources,
                [DependentSystems] = @DependentSystems,
                [PlanRTO] = @PlanRTO,
                [PlanRPO] = @PlanRPO,
                [BackupSource] = @BackupSource,
                [AlternateSupplier] = @AlternateSupplier,
                [LastTestDate] = @LastTestDate,
                [TestResultSummary] = @TestResultSummary,
                [PlanOwner] = @PlanOwner,
                [PlanStatusID] = @PlanStatusID,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE BCPID = @BCPID
            AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[BCPPlan]
            (
                [CompanyID],
                [PlanName],
                [Scope],
                [RecoveryTeamRoles],
                [ContactList],
                [InvocationCriteria],
                [RecoveryLocations],
                [RecoveryStrategyDetails],
                [KeySteps],
                [RequiredResources],
                [DependentSystems],
                [PlanRTO],
                [PlanRPO],
                [BackupSource],
                [AlternateSupplier],
                [LastTestDate],
                [TestResultSummary],
                [PlanOwner],
                [PlanStatusID],
                [CreatedBy],
                [CreatedAt],
                [UpdatedAt]
            )
            VALUES
            (
                @CompanyID,
                @PlanName,
                @Scope,
                @RecoveryTeamRoles,
                @ContactList,
                @InvocationCriteria,
                @RecoveryLocations,
                @RecoveryStrategyDetails,
                @KeySteps,
                @RequiredResources,
                @DependentSystems,
                @PlanRTO,
                @PlanRPO,
                @BackupSource,
                @AlternateSupplier,
                @LastTestDate,
                @TestResultSummary,
                @PlanOwner,
                @PlanStatusID,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'BCPPlan', @BCPID,
            (SELECT * FROM BCPPlan WHERE BCPID = (SELECT TOP 1 @@IDENTITY FROM BCPPlan) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvBCPPlans'
    BEGIN
        SELECT * FROM [BCPPlan] WHERE CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BCPPlan', NULL, (SELECT * FROM BCPPlan FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvBCPPlan'
    BEGIN
        SELECT * FROM [BCPPlan] WHERE BCPID = @BCPID AND CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'BCPPlan', @BCPID, (SELECT * FROM BCPPlan WHERE BCPID = @BCPID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteBCPPlan'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'BCPPlan', @BCPID, (SELECT * FROM BCPPlan WHERE BCPID = @BCPID FOR XML AUTO), GETDATE())

        DELETE FROM [BCPPlan] WHERE BCPID = @BCPID AND CompanyID = @CompanyID
    END
END
GO
