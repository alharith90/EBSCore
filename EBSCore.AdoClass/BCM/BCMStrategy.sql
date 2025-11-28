USE [EBS]
GO

/****** Object:  Table [dbo].[BCMStrategy] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BCMStrategy]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BCMStrategy]
    (
        [StrategyID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [UnitID] BIGINT NOT NULL,
        [Title] NVARCHAR(250) NOT NULL,
        [TitleAr] NVARCHAR(250) NULL,
        [Vision] NVARCHAR(500) NULL,
        [Mission] NVARCHAR(500) NULL,
        [EscalationCriteria] NVARCHAR(MAX) NULL,
        [ActivationCriteria] NVARCHAR(MAX) NULL,
        [StartDate] DATETIME NULL,
        [EndDate] DATETIME NULL,
        [OwnerID] INT NULL,
        [Status] NVARCHAR(50) NULL,
        [CreatedBy] INT NULL,
        [ModifiedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE())
    );
END
GO

/****** Object:  StoredProcedure [dbo].[BCMStrategySP] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[BCMStrategySP]') IS NOT NULL
    DROP PROCEDURE [dbo].[BCMStrategySP];
GO

CREATE PROCEDURE [dbo].[BCMStrategySP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @UnitID                 BIGINT = NULL,
    @StrategyID             INT = NULL,
    @Title                  NVARCHAR(250) = NULL,
    @TitleAr                NVARCHAR(250) = NULL,
    @Vision                 NVARCHAR(500) = NULL,
    @Mission                NVARCHAR(500) = NULL,
    @EscalationCriteria     NVARCHAR(MAX) = NULL,
    @ActivationCriteria     NVARCHAR(MAX) = NULL,
    @StartDate              DATETIME = NULL,
    @EndDate                DATETIME = NULL,
    @OwnerID                INT = NULL,
    @Status                 NVARCHAR(50) = NULL,
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

    IF @Operation = 'SaveStrategy'
    BEGIN
        IF EXISTS (SELECT 1 FROM BCMStrategy WHERE StrategyID = @StrategyID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'BCMStrategy', @StrategyID,
                    (SELECT * FROM BCMStrategy WHERE StrategyID = @StrategyID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[BCMStrategy]
            SET [UnitID] = @UnitID,
                [Title] = @Title,
                [TitleAr] = @TitleAr,
                [Vision] = @Vision,
                [Mission] = @Mission,
                [EscalationCriteria] = @EscalationCriteria,
                [ActivationCriteria] = @ActivationCriteria,
                [StartDate] = @StartDate,
                [EndDate] = @EndDate,
                [OwnerID] = @OwnerID,
                [Status] = @Status,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE StrategyID = @StrategyID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[BCMStrategy]
                ([CompanyID], [UnitID], [Title], [TitleAr], [Vision], [Mission], [EscalationCriteria], [ActivationCriteria],
                 [StartDate], [EndDate], [OwnerID], [Status], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @Title, @TitleAr, @Vision, @Mission, @EscalationCriteria, @ActivationCriteria,
                 @StartDate, @EndDate, @OwnerID, @Status, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewStrategyID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'BCMStrategy', @NewStrategyID,
                    (SELECT * FROM BCMStrategy WHERE StrategyID = @NewStrategyID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvStrategies'
    BEGIN
        SELECT StrategyID, CompanyID, UnitID, Title, TitleAr, Vision, Mission, EscalationCriteria, ActivationCriteria, StartDate,
               EndDate, OwnerID, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMStrategy]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BCMStrategy', NULL,
                (SELECT * FROM BCMStrategy FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategiesByUnit'
    BEGIN
        SELECT StrategyID, CompanyID, UnitID, Title, TitleAr, Vision, Mission, EscalationCriteria, ActivationCriteria, StartDate,
               EndDate, OwnerID, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMStrategy]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BCMStrategy', NULL,
                (SELECT * FROM BCMStrategy FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvStrategy'
    BEGIN
        SELECT StrategyID, CompanyID, UnitID, Title, TitleAr, Vision, Mission, EscalationCriteria, ActivationCriteria, StartDate,
               EndDate, OwnerID, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMStrategy]
        WHERE StrategyID = @StrategyID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'BCMStrategy', @StrategyID,
                (SELECT * FROM BCMStrategy WHERE StrategyID = @StrategyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteStrategy'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'BCMStrategy', @StrategyID,
                (SELECT * FROM BCMStrategy WHERE StrategyID = @StrategyID FOR XML AUTO), GETDATE());

        DELETE
        FROM [BCMStrategy]
        WHERE StrategyID = @StrategyID
          AND CompanyID = @CompanyID;
    END
END
GO
