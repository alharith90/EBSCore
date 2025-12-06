USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AuditUniverseSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @EntityProcessID BIGINT = NULL,
    @EntityProcessName NVARCHAR(250) = NULL,
    @EntityOwner NVARCHAR(250) = NULL,
    @RiskRating NVARCHAR(50) = NULL,
    @LastAuditDate DATETIME = NULL,
    @NextAuditDue DATETIME = NULL,
    @AuditFrequency NVARCHAR(50) = NULL,
    @AuditPriority NVARCHAR(50) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveAuditUniverse'
    BEGIN
        IF EXISTS(SELECT 1 FROM AuditUniverse WHERE EntityProcessID = @EntityProcessID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'AuditUniverse', @EntityProcessID,
                    (SELECT * FROM AuditUniverse WHERE EntityProcessID = @EntityProcessID FOR XML AUTO), GETDATE())

            UPDATE [AuditUniverse]
            SET EntityProcessName = @EntityProcessName,
                EntityOwner = @EntityOwner,
                RiskRating = @RiskRating,
                LastAuditDate = @LastAuditDate,
                NextAuditDue = @NextAuditDue,
                AuditFrequency = @AuditFrequency,
                AuditPriority = @AuditPriority,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE EntityProcessID = @EntityProcessID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [AuditUniverse]
            (
                CompanyID,
                EntityProcessName,
                EntityOwner,
                RiskRating,
                LastAuditDate,
                NextAuditDue,
                AuditFrequency,
                AuditPriority,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @EntityProcessName,
                @EntityOwner,
                @RiskRating,
                @LastAuditDate,
                @NextAuditDue,
                @AuditFrequency,
                @AuditPriority,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'AuditUniverse', @EntityProcessID,
                    (SELECT * FROM AuditUniverse WHERE EntityProcessID = (SELECT TOP 1 @@IDENTITY FROM AuditUniverse) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvAuditUniverse'
    BEGIN
        SELECT EntityProcessID, CompanyID, EntityProcessName, EntityOwner, RiskRating, LastAuditDate, NextAuditDue, AuditFrequency, AuditPriority, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditUniverse]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'AuditUniverse', NULL,
                (SELECT * FROM AuditUniverse FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvAuditUniverseItem'
    BEGIN
        SELECT EntityProcessID, CompanyID, EntityProcessName, EntityOwner, RiskRating, LastAuditDate, NextAuditDue, AuditFrequency, AuditPriority, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM [AuditUniverse]
        WHERE EntityProcessID = @EntityProcessID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'AuditUniverse', @EntityProcessID,
                (SELECT * FROM AuditUniverse WHERE EntityProcessID = @EntityProcessID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteAuditUniverse'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'AuditUniverse', @EntityProcessID,
                (SELECT * FROM AuditUniverse WHERE EntityProcessID = @EntityProcessID FOR XML AUTO), GETDATE())

        DELETE FROM [AuditUniverse] WHERE EntityProcessID = @EntityProcessID AND CompanyID = @CompanyID
    END
END
GO
