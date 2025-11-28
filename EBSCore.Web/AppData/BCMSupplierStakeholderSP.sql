USE [EBS]
GO

/****** Object:  Table [dbo].[Supplier] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Supplier]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Supplier](
        [SupplierID] [int] IDENTITY(1,1) NOT NULL,
        [CompanyID] [int] NOT NULL,
        [UnitID] [bigint] NULL,
        [SupplierType] [nvarchar](100) NULL,
        [SupplierName] [nvarchar](250) NOT NULL,
        [Services] [nvarchar](max) NULL,
        [MainContactName] [nvarchar](200) NULL,
        [MainContactEmail] [nvarchar](200) NULL,
        [MainContactPhone] [nvarchar](50) NULL,
        [SecondaryContactName] [nvarchar](200) NULL,
        [SecondaryContactEmail] [nvarchar](200) NULL,
        [SecondaryContactPhone] [nvarchar](50) NULL,
        [SLAInPlace] [bit] NULL,
        [RTOHours] [int] NULL,
        [RPOHours] [int] NULL,
        [Notes] [nvarchar](max) NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedBy] [bigint] NULL,
        [CreatedAt] [datetime] NULL,
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([SupplierID] ASC)
    )
END
GO

/****** Object:  Table [dbo].[Stakeholder] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stakeholder]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Stakeholder](
        [StakeholderID] [int] IDENTITY(1,1) NOT NULL,
        [CompanyID] [int] NOT NULL,
        [UnitID] [bigint] NULL,
        [StakeholderName] [nvarchar](250) NOT NULL,
        [StakeholderType] [nvarchar](100) NOT NULL,
        [Role] [nvarchar](150) NULL,
        [ContactEmail] [nvarchar](200) NULL,
        [ContactPhone] [nvarchar](50) NULL,
        [IsCritical] [bit] NULL,
        [Notes] [nvarchar](max) NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedBy] [bigint] NULL,
        [CreatedAt] [datetime] NULL,
        [UpdatedAt] [datetime] NULL,
        CONSTRAINT [PK_Stakeholder] PRIMARY KEY CLUSTERED ([StakeholderID] ASC)
    )
END
GO

/****** Stored Procedure: [dbo].[SupplierSP] ******/
IF OBJECT_ID('[dbo].[SupplierSP]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SupplierSP]
GO
CREATE PROCEDURE [dbo].[SupplierSP]
    @Operation                  NVARCHAR(100) = NULL,
    @UserID                     BIGINT = NULL,
    @CompanyID                  INT = NULL,
    @UnitID                     BIGINT = NULL,
    @SupplierID                 INT = NULL,
    @SupplierType               NVARCHAR(100) = NULL,
    @SupplierName               NVARCHAR(250) = NULL,
    @Services                   NVARCHAR(MAX) = NULL,
    @MainContactName            NVARCHAR(200) = NULL,
    @MainContactEmail           NVARCHAR(200) = NULL,
    @MainContactPhone           NVARCHAR(50) = NULL,
    @SecondaryContactName       NVARCHAR(200) = NULL,
    @SecondaryContactEmail      NVARCHAR(200) = NULL,
    @SecondaryContactPhone      NVARCHAR(50) = NULL,
    @SLAInPlace                 BIT = NULL,
    @RTOHours                   INT = NULL,
    @RPOHours                   INT = NULL,
    @Notes                      NVARCHAR(MAX) = NULL,
    @CreatedBy                  INT = NULL,
    @ModifiedBy                 INT = NULL,
    @CreatedAt                  DATETIME = NULL,
    @UpdatedAt                  DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveSupplier'
    BEGIN
        IF EXISTS(SELECT 1 FROM Supplier WHERE SupplierID = @SupplierID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'Supplier', @SupplierID, (SELECT * FROM Supplier WHERE SupplierID = @SupplierID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[Supplier]
            SET [UnitID] = @UnitID,
                [SupplierType] = @SupplierType,
                [SupplierName] = @SupplierName,
                [Services] = @Services,
                [MainContactName] = @MainContactName,
                [MainContactEmail] = @MainContactEmail,
                [MainContactPhone] = @MainContactPhone,
                [SecondaryContactName] = @SecondaryContactName,
                [SecondaryContactEmail] = @SecondaryContactEmail,
                [SecondaryContactPhone] = @SecondaryContactPhone,
                [SLAInPlace] = @SLAInPlace,
                [RTOHours] = @RTOHours,
                [RPOHours] = @RPOHours,
                [Notes] = @Notes,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE SupplierID = @SupplierID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[Supplier]
                ([CompanyID],[UnitID],[SupplierType],[SupplierName],[Services],[MainContactName],[MainContactEmail],[MainContactPhone],
                 [SecondaryContactName],[SecondaryContactEmail],[SecondaryContactPhone],[SLAInPlace],[RTOHours],[RPOHours],[Notes],
                 [CreatedBy],[CreatedAt],[UpdatedAt])
            VALUES
                (@CompanyID,@UnitID,@SupplierType,@SupplierName,@Services,@MainContactName,@MainContactEmail,@MainContactPhone,
                 @SecondaryContactName,@SecondaryContactEmail,@SecondaryContactPhone,@SLAInPlace,@RTOHours,@RPOHours,@Notes,
                 @UserID,GETUTCDATE(),GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'Supplier', @SupplierID, (SELECT * FROM Supplier WHERE SupplierID = (SELECT TOP 1 @@IDENTITY FROM Supplier) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvSuppliers'
    BEGIN
        SELECT * FROM [Supplier] WHERE CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Supplier', NULL, (SELECT * FROM Supplier FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvSuppliersByUnit'
    BEGIN
        SELECT * FROM [Supplier] WHERE CompanyID = @CompanyID AND UnitID = @UnitID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Supplier', NULL, (SELECT * FROM Supplier FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvSupplier'
    BEGIN
        SELECT * FROM [Supplier] WHERE SupplierID = @SupplierID AND CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'Supplier', @SupplierID, (SELECT * FROM Supplier WHERE SupplierID = @SupplierID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteSupplier'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'Supplier', @SupplierID, (SELECT * FROM Supplier WHERE SupplierID = @SupplierID FOR XML AUTO), GETDATE())

        DELETE FROM [Supplier] WHERE SupplierID = @SupplierID AND CompanyID = @CompanyID
    END
END
GO

/****** Stored Procedure: [dbo].[StakeholderSP] ******/
IF OBJECT_ID('[dbo].[StakeholderSP]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[StakeholderSP]
GO
CREATE PROCEDURE [dbo].[StakeholderSP]
    @Operation                  NVARCHAR(100) = NULL,
    @UserID                     BIGINT = NULL,
    @CompanyID                  INT = NULL,
    @UnitID                     BIGINT = NULL,
    @StakeholderID              INT = NULL,
    @StakeholderName            NVARCHAR(250) = NULL,
    @StakeholderType            NVARCHAR(100) = NULL,
    @Role                       NVARCHAR(150) = NULL,
    @ContactEmail               NVARCHAR(200) = NULL,
    @ContactPhone               NVARCHAR(50) = NULL,
    @IsCritical                 BIT = NULL,
    @Notes                      NVARCHAR(MAX) = NULL,
    @CreatedBy                  INT = NULL,
    @ModifiedBy                 INT = NULL,
    @CreatedAt                  DATETIME = NULL,
    @UpdatedAt                  DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveStakeholder'
    BEGIN
        IF EXISTS(SELECT 1 FROM Stakeholder WHERE StakeholderID = @StakeholderID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'Stakeholder', @StakeholderID, (SELECT * FROM Stakeholder WHERE StakeholderID = @StakeholderID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[Stakeholder]
            SET [UnitID] = @UnitID,
                [StakeholderName] = @StakeholderName,
                [StakeholderType] = @StakeholderType,
                [Role] = @Role,
                [ContactEmail] = @ContactEmail,
                [ContactPhone] = @ContactPhone,
                [IsCritical] = @IsCritical,
                [Notes] = @Notes,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE StakeholderID = @StakeholderID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[Stakeholder]
                ([CompanyID],[UnitID],[StakeholderName],[StakeholderType],[Role],[ContactEmail],[ContactPhone],[IsCritical],[Notes],[CreatedBy],[CreatedAt],[UpdatedAt])
            VALUES
                (@CompanyID,@UnitID,@StakeholderName,@StakeholderType,@Role,@ContactEmail,@ContactPhone,@IsCritical,@Notes,@UserID,GETUTCDATE(),GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'Stakeholder', @StakeholderID, (SELECT * FROM Stakeholder WHERE StakeholderID = (SELECT TOP 1 @@IDENTITY FROM Stakeholder) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvStakeholders'
    BEGIN
        SELECT * FROM [Stakeholder] WHERE CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Stakeholder', NULL, (SELECT * FROM Stakeholder FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvStakeholdersByUnit'
    BEGIN
        SELECT * FROM [Stakeholder] WHERE CompanyID = @CompanyID AND UnitID = @UnitID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'Stakeholder', NULL, (SELECT * FROM Stakeholder FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvStakeholder'
    BEGIN
        SELECT * FROM [Stakeholder] WHERE StakeholderID = @StakeholderID AND CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'Stakeholder', @StakeholderID, (SELECT * FROM Stakeholder WHERE StakeholderID = @StakeholderID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteStakeholder'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'Stakeholder', @StakeholderID, (SELECT * FROM Stakeholder WHERE StakeholderID = @StakeholderID FOR XML AUTO), GETDATE())

        DELETE FROM [Stakeholder] WHERE StakeholderID = @StakeholderID AND CompanyID = @CompanyID
    END
END
GO
