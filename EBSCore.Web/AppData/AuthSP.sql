CREATE OR ALTER PROCEDURE [dbo].[AuthSP]
    @Operation NVARCHAR(100),
    @UserName NVARCHAR(150) = NULL,
    @Password NVARCHAR(200) = NULL,
    @Token UNIQUEIDENTIFIER = NULL,
    @ExpiresAt DATETIME = NULL,
    @ResetPassword NVARCHAR(200) = NULL,
    @KeepSignedIn BIT = NULL,
    @UserID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'Login'
    BEGIN
        DECLARE @FoundUserID INT;
        SELECT @FoundUserID = UserID FROM [User] WHERE (Email = @UserName OR UserName = @UserName) AND IsDeleted = 0;

        IF @FoundUserID IS NULL
        BEGIN
            THROW 51000, 'Invalid credentials', 1;
        END

        DECLARE @LockUntil DATETIME = (SELECT LockUntil FROM [User] WHERE UserID = @FoundUserID);
        IF @LockUntil IS NOT NULL AND @LockUntil > GETUTCDATE()
        BEGIN
            THROW 51000, 'Account locked', 1;
        END

        DECLARE @StoredPassword NVARCHAR(200) = (SELECT [Password] FROM [User] WHERE UserID = @FoundUserID);
        IF @StoredPassword COLLATE SQL_Latin1_General_CP1_CS_AS <> @Password COLLATE SQL_Latin1_General_CP1_CS_AS
        BEGIN
            UPDATE [User]
                SET FailedLoginAttempts = FailedLoginAttempts + 1,
                    LockUntil = CASE WHEN FailedLoginAttempts + 1 >= 5 THEN DATEADD(MINUTE, 15, GETUTCDATE()) ELSE LockUntil END,
                    UpdateDateTime = GETUTCDATE()
            WHERE UserID = @FoundUserID;

            THROW 51000, 'Invalid credentials', 1;
        END

        IF EXISTS(SELECT 1 FROM [User] WHERE UserID = @FoundUserID AND StatusID <> 1)
        BEGIN
            THROW 51000, 'Inactive user', 1;
        END

        UPDATE [User]
            SET FailedLoginAttempts = 0,
                LockUntil = NULL,
                LastLoginAt = GETUTCDATE(),
                UpdateDateTime = GETUTCDATE()
        WHERE UserID = @FoundUserID;

        SELECT U.UserID,
               U.Email,
               U.UserFullName,
               U.UserName,
               U.CompanyID,
               U.CategoryID,
               U.UserType,
               U.UserImage,
               U.UserImageMeta,
               U.UserStatus,
               U.LastLoginAt,
               U.FailedLoginAttempts,
               U.LockUntil,
               U.StatusID,
               U.IsDeleted,
               C.Name AS CompanyName
        FROM [User] U
        LEFT JOIN Company C ON C.CompanyID = U.CompanyID
        WHERE U.UserID = @FoundUserID;
    END
    ELSE IF @Operation = 'CreateResetToken'
    BEGIN
        DECLARE @ResetUserID INT = (SELECT TOP 1 UserID FROM [User] WHERE (Email = @UserName OR UserName = @UserName) AND IsDeleted = 0);
        IF @ResetUserID IS NULL
        BEGIN
            THROW 51000, 'User not found', 1;
        END

        DECLARE @NewToken UNIQUEIDENTIFIER = ISNULL(@Token, NEWID());
        INSERT INTO PasswordResetToken (UserID, Token, ExpiresAt, IsUsed)
        VALUES (@ResetUserID, @NewToken, @ExpiresAt, 0);

        SELECT @NewToken AS Token, @ResetUserID AS UserID;
    END
    ELSE IF @Operation = 'ValidateResetToken'
    BEGIN
        SELECT TokenID,
               UserID,
               Token,
               ExpiresAt,
               IsUsed
        FROM PasswordResetToken
        WHERE Token = @Token
          AND IsUsed = 0
          AND ExpiresAt > GETUTCDATE();
    END
    ELSE IF @Operation = 'ResetPassword'
    BEGIN
        DECLARE @ResetTargetUserID INT = (SELECT UserID FROM PasswordResetToken WHERE Token = @Token AND IsUsed = 0 AND ExpiresAt > GETUTCDATE());
        IF @ResetTargetUserID IS NULL
        BEGIN
            THROW 51000, 'Invalid token', 1;
        END

        UPDATE [User]
            SET [Password] = @ResetPassword,
                FailedLoginAttempts = 0,
                LockUntil = NULL,
                UpdateDateTime = GETUTCDATE()
        WHERE UserID = @ResetTargetUserID;

        UPDATE PasswordResetToken
            SET IsUsed = 1
        WHERE Token = @Token;

        SELECT @ResetTargetUserID AS UserID;
    END
END
GO
