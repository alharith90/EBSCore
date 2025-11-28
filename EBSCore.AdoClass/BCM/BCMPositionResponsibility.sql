USE [EBS]
GO

/****** Object:  Table [dbo].[BCMPositionResponsibility]    Script Date: 2025-11-25 11:48:57 PM ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BCMPositionResponsibility]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BCMPositionResponsibility]
    (
        [PositionID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [UnitID] BIGINT NOT NULL,
        [PositionTitle] NVARCHAR(250) NOT NULL,
        [PositionCode] NVARCHAR(50) NULL,
        [Responsibilities] NVARCHAR(MAX) NULL,
        [AuthorityLevel] NVARCHAR(100) NULL,
        [ContactDetails] NVARCHAR(250) NULL,
        [Status] NVARCHAR(50) NULL,
        [CreatedBy] INT NULL,
        [ModifiedBy] INT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE())
    );
END
GO

/****** Object:  StoredProcedure [dbo].[BCMPositionResponsibilitySP] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[BCMPositionResponsibilitySP]') IS NOT NULL
    DROP PROCEDURE [dbo].[BCMPositionResponsibilitySP];
GO

CREATE PROCEDURE [dbo].[BCMPositionResponsibilitySP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @UnitID                 BIGINT = NULL,
    @PositionID             INT = NULL,
    @PositionTitle          NVARCHAR(250) = NULL,
    @PositionCode           NVARCHAR(50) = NULL,
    @Responsibilities       NVARCHAR(MAX) = NULL,
    @AuthorityLevel         NVARCHAR(100) = NULL,
    @ContactDetails         NVARCHAR(250) = NULL,
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

    IF @Operation = 'SavePositionResponsibility'
    BEGIN
        IF EXISTS (SELECT 1 FROM BCMPositionResponsibility WHERE PositionID = @PositionID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'BCMPositionResponsibility', @PositionID,
            (SELECT * FROM BCMPositionResponsibility WHERE PositionID = @PositionID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[BCMPositionResponsibility]
            SET [UnitID] = @UnitID,
                [PositionTitle] = @PositionTitle,
                [PositionCode] = @PositionCode,
                [Responsibilities] = @Responsibilities,
                [AuthorityLevel] = @AuthorityLevel,
                [ContactDetails] = @ContactDetails,
                [Status] = @Status,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE PositionID = @PositionID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[BCMPositionResponsibility]
                ([CompanyID], [UnitID], [PositionTitle], [PositionCode], [Responsibilities], [AuthorityLevel], [ContactDetails], [Status], [CreatedBy], [CreatedAt], [UpdatedAt])
            VALUES
                (@CompanyID, @UnitID, @PositionTitle, @PositionCode, @Responsibilities, @AuthorityLevel, @ContactDetails, @Status, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewPositionID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'BCMPositionResponsibility', @NewPositionID,
            (SELECT * FROM BCMPositionResponsibility WHERE PositionID = @NewPositionID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvPositionsResponsibilities'
    BEGIN
        SELECT PositionID, CompanyID, UnitID, PositionTitle, PositionCode, Responsibilities, AuthorityLevel, ContactDetails, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMPositionResponsibility]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BCMPositionResponsibility', NULL,
        (SELECT * FROM BCMPositionResponsibility FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvPositionsResponsibilitiesByUnit'
    BEGIN
        SELECT PositionID, CompanyID, UnitID, PositionTitle, PositionCode, Responsibilities, AuthorityLevel, ContactDetails, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMPositionResponsibility]
        WHERE CompanyID = @CompanyID
          AND UnitID = @UnitID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BCMPositionResponsibility', NULL,
        (SELECT * FROM BCMPositionResponsibility FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvPositionResponsibility'
    BEGIN
        SELECT PositionID, CompanyID, UnitID, PositionTitle, PositionCode, Responsibilities, AuthorityLevel, ContactDetails, Status, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [BCMPositionResponsibility]
        WHERE PositionID = @PositionID
          AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'BCMPositionResponsibility', @PositionID,
        (SELECT * FROM BCMPositionResponsibility WHERE PositionID = @PositionID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeletePositionResponsibility'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'BCMPositionResponsibility', @PositionID,
        (SELECT * FROM BCMPositionResponsibility WHERE PositionID = @PositionID FOR XML AUTO), GETDATE());

        DELETE FROM [BCMPositionResponsibility]
        WHERE PositionID = @PositionID
          AND CompanyID = @CompanyID;
    END
END
GO
