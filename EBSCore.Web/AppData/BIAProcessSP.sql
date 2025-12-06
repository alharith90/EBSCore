USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[BIAProcessSP]
    @Operation              NVARCHAR(100) = NULL,
    @UserID                 BIGINT = NULL,
    @CompanyID              INT = NULL,
    @BIAID                  BIGINT = NULL,
    @ProcessName            NVARCHAR(250) = NULL,
    @Department             NVARCHAR(250) = NULL,
    @ProcessOwner           NVARCHAR(250) = NULL,
    @CriticalityLevel       NVARCHAR(100) = NULL,
    @MAO                    NVARCHAR(100) = NULL,
    @Impact1Hour            NVARCHAR(250) = NULL,
    @Impact1Day             NVARCHAR(250) = NULL,
    @Impact1Week            NVARCHAR(250) = NULL,
    @ImpactDimensions       NVARCHAR(MAX) = NULL,
    @RTO                    NVARCHAR(100) = NULL,
    @RPO                    NVARCHAR(100) = NULL,
    @MinimumResources       NVARCHAR(MAX) = NULL,
    @InternalDependencies   NVARCHAR(MAX) = NULL,
    @ExternalDependencies   NVARCHAR(MAX) = NULL,
    @RecoveryStrategies     NVARCHAR(MAX) = NULL,
    @StrategyLibraryRef     NVARCHAR(250) = NULL,
    @AllowableDataLoss      NVARCHAR(250) = NULL,
    @BackupAvailability     NVARCHAR(250) = NULL,
    @HasAlternateWorkaround BIT = NULL,
    @BCPLink                NVARCHAR(250) = NULL,
    @LastReviewDate         DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveBIAProcess'
    BEGIN
        IF EXISTS(SELECT 1 FROM BIAProcess WHERE BIAID = @BIAID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'BIAProcess', @BIAID,
            (SELECT * FROM BIAProcess WHERE BIAID = @BIAID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[BIAProcess]
            SET
                [ProcessName] = @ProcessName,
                [Department] = @Department,
                [ProcessOwner] = @ProcessOwner,
                [CriticalityLevel] = @CriticalityLevel,
                [MAO] = @MAO,
                [Impact1Hour] = @Impact1Hour,
                [Impact1Day] = @Impact1Day,
                [Impact1Week] = @Impact1Week,
                [ImpactDimensions] = @ImpactDimensions,
                [RTO] = @RTO,
                [RPO] = @RPO,
                [MinimumResources] = @MinimumResources,
                [InternalDependencies] = @InternalDependencies,
                [ExternalDependencies] = @ExternalDependencies,
                [RecoveryStrategies] = @RecoveryStrategies,
                [StrategyLibraryRef] = @StrategyLibraryRef,
                [AllowableDataLoss] = @AllowableDataLoss,
                [BackupAvailability] = @BackupAvailability,
                [HasAlternateWorkaround] = @HasAlternateWorkaround,
                [BCPLink] = @BCPLink,
                [LastReviewDate] = @LastReviewDate,
                [ModifiedBy] = @UserID,
                [UpdatedAt] = GETUTCDATE()
            WHERE BIAID = @BIAID
            AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[BIAProcess]
            (
                [CompanyID],
                [ProcessName],
                [Department],
                [ProcessOwner],
                [CriticalityLevel],
                [MAO],
                [Impact1Hour],
                [Impact1Day],
                [Impact1Week],
                [ImpactDimensions],
                [RTO],
                [RPO],
                [MinimumResources],
                [InternalDependencies],
                [ExternalDependencies],
                [RecoveryStrategies],
                [StrategyLibraryRef],
                [AllowableDataLoss],
                [BackupAvailability],
                [HasAlternateWorkaround],
                [BCPLink],
                [LastReviewDate],
                [CreatedBy],
                [CreatedAt],
                [UpdatedAt]
            )
            VALUES
            (
                @CompanyID,
                @ProcessName,
                @Department,
                @ProcessOwner,
                @CriticalityLevel,
                @MAO,
                @Impact1Hour,
                @Impact1Day,
                @Impact1Week,
                @ImpactDimensions,
                @RTO,
                @RPO,
                @MinimumResources,
                @InternalDependencies,
                @ExternalDependencies,
                @RecoveryStrategies,
                @StrategyLibraryRef,
                @AllowableDataLoss,
                @BackupAvailability,
                @HasAlternateWorkaround,
                @BCPLink,
                @LastReviewDate,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'BIAProcess', @BIAID,
            (SELECT * FROM BIAProcess WHERE BIAID = (SELECT TOP 1 @@IDENTITY FROM BIAProcess) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvBIAProcesses'
    BEGIN
        SELECT * FROM [BIAProcess] WHERE CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'BIAProcess', NULL, (SELECT * FROM BIAProcess FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvBIAProcess'
    BEGIN
        SELECT * FROM [BIAProcess] WHERE BIAID = @BIAID AND CompanyID = @CompanyID
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'BIAProcess', @BIAID, (SELECT * FROM BIAProcess WHERE BIAID = @BIAID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteBIAProcess'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'BIAProcess', @BIAID, (SELECT * FROM BIAProcess WHERE BIAID = @BIAID FOR XML AUTO), GETDATE())

        DELETE FROM [BIAProcess] WHERE BIAID = @BIAID AND CompanyID = @CompanyID
    END
END
GO
