USE [EBS]
GO

/****** Template reference: ProcessSP demonstrates the permissions and history model ******/

IF OBJECT_ID('dbo.StrategicObjective', 'U') IS NOT NULL
    DROP TABLE dbo.StrategicObjective;
GO

CREATE TABLE [dbo].[StrategicObjective]
(
    [ObjectiveID] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyID] INT NOT NULL,
    [UnitID] BIGINT NOT NULL,
    [ObjectiveCode] NVARCHAR(50) NOT NULL,
    [Strategy] NVARCHAR(250) NOT NULL,
    [Title] NVARCHAR(250) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [OwnerID] BIGINT NULL,
    [RiskLink] NVARCHAR(250) NULL,
    [ComplianceLink] NVARCHAR(250) NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [EscalationLevel] NVARCHAR(100) NULL,
    [EscalationContact] NVARCHAR(250) NULL,
    [ActivationStatus] NVARCHAR(50) NOT NULL CONSTRAINT DF_StrategicObjective_ActivationStatus DEFAULT ('Not Activated'),
    [ActivationTrigger] NVARCHAR(250) NULL,
    [ActivationCriteria] NVARCHAR(MAX) NULL,
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_StrategicObjective_CreatedAt DEFAULT (GETUTCDATE()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_StrategicObjective_UpdatedAt DEFAULT (GETUTCDATE())
);
GO

CREATE OR ALTER PROCEDURE [dbo].[StrategicObjectiveSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @ObjectiveID INT = NULL,
    @ObjectiveCode NVARCHAR(50) = NULL,
    @Strategy NVARCHAR(250) = NULL,
    @Title NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @OwnerID BIGINT = NULL,
    @RiskLink NVARCHAR(250) = NULL,
    @ComplianceLink NVARCHAR(250) = NULL,
    @Status NVARCHAR(50) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @EscalationLevel NVARCHAR(100) = NULL,
    @EscalationContact NVARCHAR(250) = NULL,
    @ActivationStatus NVARCHAR(50) = NULL,
    @ActivationTrigger NVARCHAR(250) = NULL,
    @ActivationCriteria NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveStrategicObjective'
    BEGIN
        IF EXISTS(SELECT 1 FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'StrategicObjective', @ObjectiveID,
                    (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[StrategicObjective]
            SET [UnitID] = @UnitID,
                [ObjectiveCode] = @ObjectiveCode,
                [Strategy] = @Strategy,
                [Title] = @Title,
                [Description] = @Description,
                [OwnerID] = @OwnerID,
                [RiskLink] = @RiskLink,
                [ComplianceLink] = @ComplianceLink,
                [Status] = @Status,
                [StartDate] = @StartDate,
                [EndDate] = @EndDate,
                [EscalationLevel] = @EscalationLevel,
                [EscalationContact] = @EscalationContact,
                [ActivationStatus] = @ActivationStatus,
                [ActivationTrigger] = @ActivationTrigger,
                [ActivationCriteria] = @ActivationCriteria,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE ObjectiveID = @ObjectiveID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[StrategicObjective]
                ([CompanyID], [UnitID], [ObjectiveCode], [Strategy], [Title], [Description], [OwnerID], [RiskLink], [ComplianceLink], [Status], [StartDate], [EndDate], [EscalationLevel], [EscalationContact], [ActivationStatus], [ActivationTrigger], [ActivationCriteria], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @ObjectiveCode, @Strategy, @Title, @Description, @OwnerID, @RiskLink, @ComplianceLink, ISNULL(@Status, 'On Track'), @StartDate, @EndDate, @EscalationLevel, @EscalationContact, ISNULL(@ActivationStatus, 'Not Activated'), @ActivationTrigger, @ActivationCriteria, @UserID, GETUTCDATE(), GETUTCDATE());

            SET @ObjectiveID = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'StrategicObjective', @ObjectiveID,
                    (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvStrategicObjectives'
    BEGIN
        SELECT ObjectiveID, CompanyID, UnitID, ObjectiveCode, Strategy, Title, Description, OwnerID, RiskLink, ComplianceLink,
               Status, StartDate, EndDate, EscalationLevel, EscalationContact, ActivationStatus, ActivationTrigger, ActivationCriteria,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[StrategicObjective]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicObjective', NULL,
                (SELECT * FROM StrategicObjective WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicObjectivesByUnit'
    BEGIN
        SELECT ObjectiveID, CompanyID, UnitID, ObjectiveCode, Strategy, Title, Description, OwnerID, RiskLink, ComplianceLink,
               Status, StartDate, EndDate, EscalationLevel, EscalationContact, ActivationStatus, ActivationTrigger, ActivationCriteria,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[StrategicObjective]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicObjective', NULL,
                (SELECT * FROM StrategicObjective WHERE CompanyID = @CompanyID AND UnitID = @UnitID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicObjective'
    BEGIN
        SELECT ObjectiveID, CompanyID, UnitID, ObjectiveCode, Strategy, Title, Description, OwnerID, RiskLink, ComplianceLink,
               Status, StartDate, EndDate, EscalationLevel, EscalationContact, ActivationStatus, ActivationTrigger, ActivationCriteria,
               CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [dbo].[StrategicObjective]
        WHERE ObjectiveID = @ObjectiveID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'StrategicObjective', @ObjectiveID,
                (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteStrategicObjective'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'StrategicObjective', @ObjectiveID,
                (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE());

        DELETE FROM [dbo].[StrategicObjective]
        WHERE ObjectiveID = @ObjectiveID
          AND CompanyID = @CompanyID;
    END
END
GO
