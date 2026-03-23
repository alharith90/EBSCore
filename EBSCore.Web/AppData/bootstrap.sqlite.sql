-- EBSCore SQLite bootstrap (idempotent)
PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS AppUser (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserName TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    UserFullName TEXT NOT NULL,
    Password TEXT NOT NULL,
    CompanyID INTEGER NOT NULL DEFAULT 1,
    CategoryID INTEGER NOT NULL DEFAULT 1,
    UserType INTEGER NOT NULL DEFAULT 1,
    UserImage TEXT NULL,
    CompanyName TEXT NULL,
    UserStatus INTEGER NOT NULL DEFAULT 1,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    LastLoginAt TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS PasswordResetToken (
    TokenID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    Token TEXT NOT NULL UNIQUE,
    ExpiresAt TEXT NOT NULL,
    IsUsed INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES AppUser(UserID)
);

CREATE TABLE IF NOT EXISTS LoginAuditHistory (
    AuditID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NULL,
    UserName TEXT NULL,
    IsSuccess INTEGER NOT NULL,
    FailureReason TEXT NULL,
    IPAddress TEXT NULL,
    UserAgent TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS MenuItems (
    MenuItemID INTEGER PRIMARY KEY,
    ParentID INTEGER NULL,
    LabelAR TEXT NOT NULL,
    LabelEN TEXT NOT NULL,
    DescriptionAR TEXT NULL,
    DescriptionEn TEXT NULL,
    Url TEXT NULL,
    Icon TEXT NULL,
    [Order] INTEGER NOT NULL DEFAULT 1,
    IsActive INTEGER NOT NULL DEFAULT 1,
    Permission TEXT NULL,
    Type TEXT NULL,
    CreatedBy INTEGER NOT NULL DEFAULT 1,
    UpdatedBy INTEGER NULL,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TEXT NULL,
    FOREIGN KEY (ParentID) REFERENCES MenuItems(MenuItemID),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(UserID),
    FOREIGN KEY (UpdatedBy) REFERENCES AppUser(UserID)
);

CREATE INDEX IF NOT EXISTS IX_MenuItems_ParentID ON MenuItems(ParentID);
CREATE INDEX IF NOT EXISTS IX_MenuItems_IsActive ON MenuItems(IsActive);
CREATE INDEX IF NOT EXISTS IX_PasswordResetToken_UserID ON PasswordResetToken(UserID);
CREATE INDEX IF NOT EXISTS IX_PasswordResetToken_ExpiresAt ON PasswordResetToken(ExpiresAt);
CREATE INDEX IF NOT EXISTS IX_LoginAuditHistory_UserID ON LoginAuditHistory(UserID);

INSERT INTO AppUser (UserID, UserName, Email, UserFullName, Password, CompanyID, CategoryID, UserType, CompanyName, UserStatus, IsDeleted)
SELECT 1, 'admin', 'admin@ebscore.local', 'System Administrator', 'admin123', 1, 1, 1, 'EBS Demo', 1, 0
WHERE NOT EXISTS (SELECT 1 FROM AppUser WHERE UserID = 1);

INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy)
SELECT 1, NULL, 'لوحة التحكم', 'Dashboard', '/', 'fa-solid fa-square-poll-vertical', 1, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 1);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 2, NULL, 'استمرارية الأعمال', 'Business Continuity', NULL, 2, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 2);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 3, 2, 'تحليل أثر الأعمال', 'BIA', '/BCM/BIA', 1, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 3);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 4, 2, 'الحوادث', 'Incidents', '/BCM/Incidents', 2, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 4);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 5, 2, 'الموردون', 'Suppliers', '/BCM/Suppliers', 3, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 5);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 6, NULL, 'الضبط', 'Configuration', NULL, 3, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 6);
INSERT INTO MenuItems (MenuItemID, ParentID, LabelAR, LabelEN, Url, [Order], IsActive, CreatedBy)
SELECT 7, 6, 'الموظفون', 'Employees', '/Config/Employees', 1, 1, 1
WHERE NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemID = 7);

COMMIT;
