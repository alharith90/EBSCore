IF OBJECT_ID('dbo.PasswordResetToken','U') IS NULL
BEGIN
    CREATE TABLE dbo.PasswordResetToken (
        TokenID BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserID BIGINT NOT NULL,
        Token NVARCHAR(200) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        IsUsed BIT NOT NULL DEFAULT(0),
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME())
    );
END
GO
