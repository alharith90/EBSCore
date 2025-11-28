USE [EBS]
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StrategicInitiative]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StrategicInitiative]
    (
        [InitiativeID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [UnitID] BIGINT NOT NULL,
        [ObjectiveID] INT NOT NULL,
        [Title] NVARCHAR(250) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [OwnerID] BIGINT NULL,
        [DepartmentID] BIGINT NULL,
        [Budget] DECIMAL(18,2) NULL,
        [Progress] INT NULL,
        [StartDate] DATETIME NULL,
        [EndDate] DATETIME NULL,
        [Status] NVARCHAR(50) NULL,
        [EscalationCriteria] NVARCHAR(MAX) NULL,
        [EscalationContact] NVARCHAR(250) NULL,
        [ActivationCriteria] NVARCHAR(MAX) NULL,
        [ActivationTrigger] NVARCHAR(250) NULL,
        [ActivationStatus] NVARCHAR(50) NULL,
        [CreatedBy] INT NULL,
        [ModifiedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE())
    );
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[StrategicInitiativeSP]') IS NOT NULL
    DROP PROCEDURE [dbo].[StrategicInitiativeSP];
GO

CREATE PROCEDURE [dbo].[StrategicInitiativeSP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @UnitID                 BIGINT = NULL,
    @InitiativeID           INT = NULL,
    @ObjectiveID            INT = NULL,
    @Title                  NVARCHAR(250) = NULL,
    @Description            NVARCHAR(MAX) = NULL,
    @OwnerID                BIGINT = NULL,
    @DepartmentID           BIGINT = NULL,
    @Budget                 DECIMAL(18,2) = NULL,
    @Progress               INT = NULL,
    @StartDate              DATETIME = NULL,
    @EndDate                DATETIME = NULL,
    @Status                 NVARCHAR(50) = NULL,
    @EscalationCriteria     NVARCHAR(MAX) = NULL,
    @EscalationContact      NVARCHAR(250) = NULL,
    @ActivationCriteria     NVARCHAR(MAX) = NULL,
    @ActivationTrigger      NVARCHAR(250) = NULL,
    @ActivationStatus       NVARCHAR(50) = NULL,
    @CreatedBy              INT = NULL,
    @ModifiedBy             INT = NULL,
    @CreatedAt              DATETIME = NULL,
    @UpdatedAt              DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID);

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveStrategicInitiative'
    BEGIN
        IF EXISTS (SELECT 1 FROM StrategicInitiative WHERE InitiativeID = @InitiativeID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'StrategicInitiative', @InitiativeID,
            (SELECT * FROM StrategicInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[StrategicInitiative]
            SET [UnitID] = @UnitID,
                [ObjectiveID] = @ObjectiveID,
                [Title] = @Title,
                [Description] = @Description,
                [OwnerID] = @OwnerID,
                [DepartmentID] = @DepartmentID,
                [Budget] = @Budget,
                [Progress] = @Progress,
                [StartDate] = @StartDate,
                [EndDate] = @EndDate,
                [Status] = @Status,
                [EscalationCriteria] = @EscalationCriteria,
                [EscalationContact] = @EscalationContact,
                [ActivationCriteria] = @ActivationCriteria,
                [ActivationTrigger] = @ActivationTrigger,
                [ActivationStatus] = @ActivationStatus,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE InitiativeID = @InitiativeID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[StrategicInitiative]
                ([CompanyID], [UnitID], [ObjectiveID], [Title], [Description], [OwnerID], [DepartmentID], [Budget], [Progress], [StartDate], [EndDate], [Status], [EscalationCriteria], [EscalationContact], [ActivationCriteria], [ActivationTrigger], [ActivationStatus], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @ObjectiveID, @Title, @Description, @OwnerID, @DepartmentID, @Budget, @Progress, @StartDate, @EndDate, @Status, @EscalationCriteria, @EscalationContact, @ActivationCriteria, @ActivationTrigger, @ActivationStatus, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewInitiativeID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'StrategicInitiative', @NewInitiativeID,
            (SELECT * FROM StrategicInitiative WHERE InitiativeID = @NewInitiativeID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvStrategicInitiatives'
    BEGIN
        SELECT InitiativeID, CompanyID, UnitID, ObjectiveID, Title, Description, OwnerID, DepartmentID, Budget, Progress, StartDate, EndDate, Status, EscalationCriteria, EscalationContact, ActivationCriteria, ActivationTrigger, ActivationStatus, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicInitiative]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicInitiative', NULL,
        (SELECT * FROM StrategicInitiative FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicInitiativesByUnit'
    BEGIN
        SELECT InitiativeID, CompanyID, UnitID, ObjectiveID, Title, Description, OwnerID, DepartmentID, Budget, Progress, StartDate, EndDate, Status, EscalationCriteria, EscalationContact, ActivationCriteria, ActivationTrigger, ActivationStatus, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicInitiative]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicInitiative', NULL,
        (SELECT * FROM StrategicInitiative FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategicInitiative'
    BEGIN
        SELECT InitiativeID, CompanyID, UnitID, ObjectiveID, Title, Description, OwnerID, DepartmentID, Budget, Progress, StartDate, EndDate, Status, EscalationCriteria, EscalationContact, ActivationCriteria, ActivationTrigger, ActivationStatus, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [StrategicInitiative]
        WHERE InitiativeID = @InitiativeID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'StrategicInitiative', @InitiativeID,
        (SELECT * FROM StrategicInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteStrategicInitiative'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'StrategicInitiative', @InitiativeID,
        (SELECT * FROM StrategicInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE());

        DELETE FROM [StrategicInitiative]
        WHERE InitiativeID = @InitiativeID
          AND CompanyID = @CompanyID;
    END
END
GO
