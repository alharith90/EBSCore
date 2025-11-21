SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- User authentication schema updates
-- =============================================
IF COL_LENGTH('dbo.[User]', 'LastLoginAt') IS NULL
BEGIN
    ALTER TABLE [dbo].[User] ADD [LastLoginAt] DATETIME NULL;
END
GO

IF COL_LENGTH('dbo.[User]', 'FailedLoginAttempts') IS NULL
BEGIN
    ALTER TABLE [dbo].[User] ADD [FailedLoginAttempts] INT NOT NULL CONSTRAINT DF_User_FailedLoginAttempts DEFAULT(0);
END
GO

IF COL_LENGTH('dbo.[User]', 'LockUntil') IS NULL
BEGIN
    ALTER TABLE [dbo].[User] ADD [LockUntil] DATETIME NULL;
END
GO

-- Ensure password column can hold hashed values
ALTER TABLE [dbo].[User] ALTER COLUMN [Password] NVARCHAR(256) NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PasswordResetToken')
BEGIN
    CREATE TABLE [dbo].[PasswordResetToken]
    (
        [TokenID] INT IDENTITY(1,1) PRIMARY KEY,
        [UserID] INT NOT NULL,
        [Token] UNIQUEIDENTIFIER NOT NULL,
        [ExpiresAt] DATETIME NOT NULL,
        [IsUsed] BIT NOT NULL DEFAULT(0),
        [CreatedAt] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_PasswordResetToken_User FOREIGN KEY ([UserID]) REFERENCES [dbo].[User]([UserID])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LoginAuditHistory')
BEGIN
    CREATE TABLE [dbo].[LoginAuditHistory]
    (
        [LoginAuditHistoryID] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserID] INT NULL,
        [UserName] NVARCHAR(150) NULL,
        [IsSuccess] BIT NOT NULL,
        [FailureReason] NVARCHAR(250) NULL,
        [IPAddress] NVARCHAR(64) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LoginDeviceHistory')
BEGIN
    CREATE TABLE [dbo].[LoginDeviceHistory]
    (
        [LoginDeviceHistoryID] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserID] INT NOT NULL,
        [DeviceFingerprint] NVARCHAR(256) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [IPAddress] NVARCHAR(64) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_LoginDeviceHistory_User FOREIGN KEY ([UserID]) REFERENCES [dbo].[User]([UserID])
    );
END
GO

-- =============================================
-- Stored Procedure: S7SUserAuthSP
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[S7SUserAuthSP]
    @Operation NVARCHAR(50),
    @UserID INT = NULL,
    @UserName NVARCHAR(150) = NULL,
    @Email NVARCHAR(150) = NULL,
    @Password NVARCHAR(256) = NULL,
    @NewPassword NVARCHAR(256) = NULL,
    @Token UNIQUEIDENTIFIER = NULL,
    @TokenExpiryMinutes INT = 60,
    @IPAddress NVARCHAR(64) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @LockDurationMinutes INT = 15,
    @MaxFailedAttempts INT = 5
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NormalizedUser NVARCHAR(150) = COALESCE(@UserName, @Email);
    DECLARE @PasswordHash NVARCHAR(256) = CASE WHEN @Password IS NULL THEN NULL ELSE CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @Password + ISNULL(@NormalizedUser,'')), 2) END;
    DECLARE @Now DATETIME = SYSUTCDATETIME();

    BEGIN TRY
        IF @Operation = 'GetUserByUsername'
        BEGIN
            SELECT TOP 1 [UserID], [Email], [UserFullName], [CompanyID], [CategoryID], [UserType], [UserName], [UserImage], [UserImageMeta], [Mobile], [RequireNewPassword], [UserStatus], [BirthDate], [ExpiryDate], [FailedLoginAttempts], [LockUntil], [LastLoginAt]
            FROM [dbo].[User]
            WHERE [UserName] = @UserName AND [UserStatus] = 1;
            RETURN;
        END

        IF @Operation = 'GetUserByEmail'
        BEGIN
            SELECT TOP 1 [UserID], [Email], [UserFullName], [CompanyID], [CategoryID], [UserType], [UserName], [UserImage], [UserImageMeta], [Mobile], [RequireNewPassword], [UserStatus], [BirthDate], [ExpiryDate], [FailedLoginAttempts], [LockUntil], [LastLoginAt]
            FROM [dbo].[User]
            WHERE [Email] = @Email AND [UserStatus] = 1;
            RETURN;
        END

        IF @Operation = 'ValidatePassword'
        BEGIN
            DECLARE @StoredHash NVARCHAR(256);
            DECLARE @AccountLocked BIT = 0;
            SELECT TOP 1 @StoredHash = [Password], @UserID = [UserID], @Email = [Email], @LockUntil = [LockUntil]
            FROM [dbo].[User]
            WHERE ([UserName] = @UserName OR [Email] = @Email) AND [UserStatus] = 1;

            IF @StoredHash IS NULL
            BEGIN
                SELECT CAST(0 AS BIT) AS [IsValid], CAST(1 AS BIT) AS [IsLocked], CAST(0 AS BIT) AS [UserExists];
                RETURN;
            END

            IF @LockUntil IS NOT NULL AND @LockUntil > @Now
            BEGIN
                SET @AccountLocked = 1;
            END

            DECLARE @IsValid BIT = CASE WHEN @StoredHash = @PasswordHash THEN 1 ELSE 0 END;
            SELECT @IsValid AS [IsValid], @AccountLocked AS [IsLocked], CAST(1 AS BIT) AS [UserExists];
            RETURN;
        END

        IF @Operation = 'RegisterLoginAttempt'
        BEGIN
            INSERT INTO [dbo].[LoginAuditHistory] ([UserID], [UserName], [IsSuccess], [FailureReason], [IPAddress], [UserAgent])
            VALUES (@UserID, @NormalizedUser, CASE WHEN @PasswordHash IS NULL THEN 0 ELSE 1 END, @NewPassword, @IPAddress, @UserAgent);
            RETURN;
        END

        IF @Operation = 'LockAccount'
        BEGIN
            UPDATE [dbo].[User]
            SET [FailedLoginAttempts] = ISNULL([FailedLoginAttempts],0) + 1,
                [LockUntil] = CASE WHEN ISNULL([FailedLoginAttempts],0) + 1 >= @MaxFailedAttempts THEN DATEADD(MINUTE, @LockDurationMinutes, @Now) ELSE [LockUntil] END
            WHERE [UserID] = @UserID;
            RETURN;
        END

        IF @Operation = 'ResetFailedAttempts'
        BEGIN
            UPDATE [dbo].[User]
            SET [FailedLoginAttempts] = 0,
                [LockUntil] = NULL
            WHERE [UserID] = @UserID;
            RETURN;
        END

        IF @Operation = 'CreateResetToken'
        BEGIN
            DECLARE @ResetToken UNIQUEIDENTIFIER = NEWID();
            INSERT INTO [dbo].[PasswordResetToken]([UserID], [Token], [ExpiresAt])
            VALUES (@UserID, @ResetToken, DATEADD(MINUTE, @TokenExpiryMinutes, @Now));
            SELECT @ResetToken AS [Token];
            RETURN;
        END

        IF @Operation = 'ValidateResetToken'
        BEGIN
            SELECT TOP 1 t.[TokenID], t.[UserID], t.[Token], t.[ExpiresAt], t.[IsUsed], t.[CreatedAt], u.[UserName], u.[Email]
            FROM [dbo].[PasswordResetToken] t
            INNER JOIN [dbo].[User] u ON u.[UserID] = t.[UserID]
            WHERE t.[Token] = @Token AND t.[IsUsed] = 0 AND t.[ExpiresAt] > @Now;
            RETURN;
        END

        IF @Operation = 'ResetPassword'
        BEGIN
            DECLARE @NewHash NVARCHAR(256) = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @NewPassword + ISNULL(@NormalizedUser,'')), 2);
            UPDATE U
            SET [Password] = @NewHash,
                [FailedLoginAttempts] = 0,
                [LockUntil] = NULL,
                [UpdateDateTime] = @Now
            FROM [dbo].[User] U
            WHERE U.[UserID] = @UserID;

            UPDATE [dbo].[PasswordResetToken]
            SET [IsUsed] = 1
            WHERE [Token] = @Token;
            RETURN;
        END

        IF @Operation = 'UpdateLastLogin'
        BEGIN
            UPDATE [dbo].[User]
            SET [LastLoginAt] = @Now,
                [FailedLoginAttempts] = 0,
                [LockUntil] = NULL
            WHERE [UserID] = @UserID;
            RETURN;
        END

        IF @Operation = 'Login'
        BEGIN
            DECLARE @UserTable TABLE([UserID] INT, [Email] NVARCHAR(150), [UserFullName] NVARCHAR(150), [CompanyID] INT, [CategoryID] INT, [UserType] INT, [UserName] NVARCHAR(20), [UserStatus] INT, [FailedLoginAttempts] INT, [LockUntil] DATETIME, [LastLoginAt] DATETIME);

            INSERT INTO @UserTable([UserID], [Email], [UserFullName], [CompanyID], [CategoryID], [UserType], [UserName], [UserStatus], [FailedLoginAttempts], [LockUntil], [LastLoginAt])
            SELECT [UserID], [Email], [UserFullName], [CompanyID], [CategoryID], [UserType], [UserName], [UserStatus], ISNULL([FailedLoginAttempts],0), [LockUntil], [LastLoginAt]
            FROM [dbo].[User]
            WHERE ([UserName] = @UserName OR [Email] = @Email);

            DECLARE @LoginUserID INT, @LoginFailed INT, @LoginLock DATETIME, @LoginStatus INT;
            SELECT TOP 1 @LoginUserID = [UserID], @LoginFailed = [FailedLoginAttempts], @LoginLock = [LockUntil], @LoginStatus = [UserStatus]
            FROM @UserTable;

            IF @LoginUserID IS NULL OR @LoginStatus <> 1
            BEGIN
                INSERT INTO [dbo].[LoginAuditHistory]([UserID], [UserName], [IsSuccess], [FailureReason], [IPAddress], [UserAgent])
                VALUES (NULL, @NormalizedUser, 0, 'UserNotFound', @IPAddress, @UserAgent);
                SELECT CAST(0 AS BIT) AS [IsAuthenticated], 'UserNotFound' AS [Reason];
                RETURN;
            END

            IF @LoginLock IS NOT NULL AND @LoginLock > @Now
            BEGIN
                INSERT INTO [dbo].[LoginAuditHistory]([UserID], [UserName], [IsSuccess], [FailureReason], [IPAddress], [UserAgent])
                VALUES (@LoginUserID, @NormalizedUser, 0, 'AccountLocked', @IPAddress, @UserAgent);
                SELECT CAST(0 AS BIT) AS [IsAuthenticated], 'AccountLocked' AS [Reason];
                RETURN;
            END

            DECLARE @StoredLoginHash NVARCHAR(256);
            SELECT TOP 1 @StoredLoginHash = [Password]
            FROM [dbo].[User]
            WHERE [UserID] = @LoginUserID;

            IF @StoredLoginHash <> @PasswordHash
            BEGIN
                UPDATE [dbo].[User]
                SET [FailedLoginAttempts] = ISNULL([FailedLoginAttempts],0) + 1,
                    [LockUntil] = CASE WHEN ISNULL([FailedLoginAttempts],0) + 1 >= @MaxFailedAttempts THEN DATEADD(MINUTE, @LockDurationMinutes, @Now) ELSE [LockUntil] END
                WHERE [UserID] = @LoginUserID;

                INSERT INTO [dbo].[LoginAuditHistory]([UserID], [UserName], [IsSuccess], [FailureReason], [IPAddress], [UserAgent])
                VALUES (@LoginUserID, @NormalizedUser, 0, 'InvalidCredentials', @IPAddress, @UserAgent);

                SELECT CAST(0 AS BIT) AS [IsAuthenticated], 'InvalidCredentials' AS [Reason];
                RETURN;
            END

            UPDATE [dbo].[User]
            SET [FailedLoginAttempts] = 0,
                [LockUntil] = NULL,
                [LastLoginAt] = @Now
            WHERE [UserID] = @LoginUserID;

            INSERT INTO [dbo].[LoginAuditHistory]([UserID], [UserName], [IsSuccess], [FailureReason], [IPAddress], [UserAgent])
            VALUES (@LoginUserID, @NormalizedUser, 1, NULL, @IPAddress, @UserAgent);

            SELECT TOP 1 CAST(1 AS BIT) AS [IsAuthenticated], 'Success' AS [Reason], [UserID], [Email], [UserFullName], [CompanyID], [CategoryID], [UserType], [UserName], [LastLoginAt]
            FROM @UserTable;
            RETURN;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH
END
GO
