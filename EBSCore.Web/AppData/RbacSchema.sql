/*
    RBAC schema for EBS that aligns API/Razor enforcement with MenuItems
    Run the script once on the EBS database. It creates a dedicated [sec] schema,
    tables for users/groups/roles, a permission catalog tied to URLs or menu items,
    and helper views for evaluating effective permissions per user.
*/
GO

-- Safety: create schema
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'sec')
    EXEC('CREATE SCHEMA sec');
GO

/* === Core identities === */
IF OBJECT_ID('sec.Users') IS NOT NULL DROP TABLE sec.Users;
CREATE TABLE sec.Users
(
    UserId          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID() CONSTRAINT PK_secUsers PRIMARY KEY,
    UserName        NVARCHAR(100)       NOT NULL UNIQUE,
    DisplayName     NVARCHAR(150)       NULL,
    Email           NVARCHAR(256)       NULL,
    IsActive        BIT                 NOT NULL DEFAULT(1),
    CreatedAt       DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

IF OBJECT_ID('sec.Groups') IS NOT NULL DROP TABLE sec.Groups;
CREATE TABLE sec.Groups
(
    GroupId     INT IDENTITY(1,1)   NOT NULL CONSTRAINT PK_secGroups PRIMARY KEY,
    GroupName   NVARCHAR(100)       NOT NULL UNIQUE,
    Description NVARCHAR(250)       NULL,
    IsActive    BIT                 NOT NULL DEFAULT(1),
    CreatedAt   DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

IF OBJECT_ID('sec.Roles') IS NOT NULL DROP TABLE sec.Roles;
CREATE TABLE sec.Roles
(
    RoleId      INT IDENTITY(1,1)   NOT NULL CONSTRAINT PK_secRoles PRIMARY KEY,
    RoleName    NVARCHAR(100)       NOT NULL UNIQUE,
    Description NVARCHAR(250)       NULL,
    IsActive    BIT                 NOT NULL DEFAULT(1),
    CreatedAt   DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt   DATETIME2(0)        NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/* === Membership === */
IF OBJECT_ID('sec.UserGroups') IS NOT NULL DROP TABLE sec.UserGroups;
CREATE TABLE sec.UserGroups
(
    GroupId     INT                NOT NULL CONSTRAINT FK_UserGroups_Group REFERENCES sec.Groups(GroupId),
    UserId      UNIQUEIDENTIFIER   NOT NULL CONSTRAINT FK_UserGroups_User  REFERENCES sec.Users(UserId),
    AddedAt     DATETIME2(0)       NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_secUserGroups PRIMARY KEY (GroupId, UserId)
);
GO

IF OBJECT_ID('sec.UserRoles') IS NOT NULL DROP TABLE sec.UserRoles;
CREATE TABLE sec.UserRoles
(
    RoleId      INT                NOT NULL CONSTRAINT FK_UserRoles_Role REFERENCES sec.Roles(RoleId),
    UserId      UNIQUEIDENTIFIER   NOT NULL CONSTRAINT FK_UserRoles_User REFERENCES sec.Users(UserId),
    AddedAt     DATETIME2(0)       NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_secUserRoles PRIMARY KEY (RoleId, UserId)
);
GO

IF OBJECT_ID('sec.GroupRoles') IS NOT NULL DROP TABLE sec.GroupRoles;
CREATE TABLE sec.GroupRoles
(
    GroupId     INT NOT NULL CONSTRAINT FK_GroupRoles_Group REFERENCES sec.Groups(GroupId),
    RoleId      INT NOT NULL CONSTRAINT FK_GroupRoles_Role  REFERENCES sec.Roles(RoleId),
    AddedAt     DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_secGroupRoles PRIMARY KEY (GroupId, RoleId)
);
GO

/* === Permission catalog === */
IF OBJECT_ID('sec.Actions') IS NOT NULL DROP TABLE sec.Actions;
CREATE TABLE sec.Actions
(
    ActionId        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_secActions PRIMARY KEY,
    ActionKey       NVARCHAR(50)      NOT NULL UNIQUE, -- e.g., View, Create, Update
    HttpMethod      NVARCHAR(10)      NULL,            -- Optional verb hint for API filters
    Description     NVARCHAR(200)     NULL,
    SortOrder       INT               NOT NULL DEFAULT(1)
);
GO

IF OBJECT_ID('sec.Resources') IS NOT NULL DROP TABLE sec.Resources;
CREATE TABLE sec.Resources
(
    ResourceId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_secResources PRIMARY KEY,
    MenuItemId      BIGINT            NULL, -- optional link to dbo.MenuItems.MenuItemID
    Url             NVARCHAR(400)     NULL,
    ControllerName  NVARCHAR(150)     NULL,
    ActionName      NVARCHAR(150)     NULL,
    Area            NVARCHAR(100)     NULL,
    ResourceType    NVARCHAR(30)      NOT NULL DEFAULT('Api'), -- Api | RazorPage | ViewComponent
    Description     NVARCHAR(250)     NULL,
    IsActive        BIT               NOT NULL DEFAULT(1),
    CONSTRAINT UQ_secResources UNIQUE (ISNULL(Url,''), ISNULL(ControllerName,''), ISNULL(ActionName,''), ISNULL(Area,''), ResourceType)
);
GO

IF OBJECT_ID('sec.ResourceActions') IS NOT NULL DROP TABLE sec.ResourceActions;
CREATE TABLE sec.ResourceActions
(
    ResourceActionId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_secResourceActions PRIMARY KEY,
    ResourceId       INT               NOT NULL CONSTRAINT FK_ResourceActions_Resource REFERENCES sec.Resources(ResourceId),
    ActionId         INT               NOT NULL CONSTRAINT FK_ResourceActions_Action REFERENCES sec.Actions(ActionId),
    CustomKey        NVARCHAR(100)     NULL, -- optional app-specific action id for Razor
    IsActive         BIT               NOT NULL DEFAULT(1),
    CONSTRAINT UQ_secResourceActions UNIQUE(ResourceId, ActionId, ISNULL(CustomKey,''))
);
GO

IF OBJECT_ID('sec.RolePermissions') IS NOT NULL DROP TABLE sec.RolePermissions;
CREATE TABLE sec.RolePermissions
(
    RolePermissionId  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_secRolePermissions PRIMARY KEY,
    RoleId            INT               NOT NULL CONSTRAINT FK_RolePermissions_Role REFERENCES sec.Roles(RoleId),
    ResourceActionId  INT               NOT NULL CONSTRAINT FK_RolePermissions_ResourceAction REFERENCES sec.ResourceActions(ResourceActionId),
    Effect            CHAR(1)           NOT NULL DEFAULT('A') CHECK (Effect IN ('A','D')), -- A=Allow, D=Deny
    CreatedAt         DATETIME2(0)      NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_secRolePermissions UNIQUE(RoleId, ResourceActionId)
);
GO

IF OBJECT_ID('sec.UserOverrides') IS NOT NULL DROP TABLE sec.UserOverrides;
CREATE TABLE sec.UserOverrides
(
    UserOverrideId    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_secUserOverrides PRIMARY KEY,
    UserId            UNIQUEIDENTIFIER  NOT NULL CONSTRAINT FK_UserOverrides_User REFERENCES sec.Users(UserId),
    ResourceActionId  INT               NOT NULL CONSTRAINT FK_UserOverrides_ResourceAction REFERENCES sec.ResourceActions(ResourceActionId),
    Effect            CHAR(1)           NOT NULL CHECK (Effect IN ('A','D')),
    CreatedAt         DATETIME2(0)      NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_secUserOverrides UNIQUE(UserId, ResourceActionId)
);
GO

/* === Helper views === */
IF OBJECT_ID('sec.vUserEffectiveRoles') IS NOT NULL DROP VIEW sec.vUserEffectiveRoles;
GO
CREATE VIEW sec.vUserEffectiveRoles
AS
    SELECT ur.UserId, ur.RoleId FROM sec.UserRoles ur
    UNION
    SELECT ug.UserId, gr.RoleId
    FROM sec.UserGroups ug
    INNER JOIN sec.GroupRoles gr ON ug.GroupId = gr.GroupId;
GO

IF OBJECT_ID('sec.vUserEffectivePermissions') IS NOT NULL DROP VIEW sec.vUserEffectivePermissions;
GO
CREATE VIEW sec.vUserEffectivePermissions
AS
    -- Effective role grants/denies
    SELECT
        er.UserId,
        ra.ResourceId,
        ra.ActionId,
        COALESCE(MAX(CASE WHEN rp.Effect = 'D' THEN 0 WHEN rp.Effect = 'A' THEN 1 END), 0) AS RoleAllow
    FROM sec.vUserEffectiveRoles er
    INNER JOIN sec.RolePermissions rp ON er.RoleId = rp.RoleId
    INNER JOIN sec.ResourceActions ra ON rp.ResourceActionId = ra.ResourceActionId
    WHERE rp.Effect IN ('A','D') AND ra.IsActive = 1
    GROUP BY er.UserId, ra.ResourceId, ra.ActionId
    UNION
    -- User overrides trump role results; deny has priority when both exist
    SELECT
        uo.UserId,
        ra.ResourceId,
        ra.ActionId,
        CASE WHEN MIN(uo.Effect) = 'D' THEN 0 ELSE 1 END AS RoleAllow
    FROM sec.UserOverrides uo
    INNER JOIN sec.ResourceActions ra ON uo.ResourceActionId = ra.ResourceActionId
    WHERE ra.IsActive = 1
    GROUP BY uo.UserId, ra.ResourceId, ra.ActionId;
GO

/* === Default catalog data === */
TRUNCATE TABLE sec.Actions;
INSERT INTO sec.Actions (ActionKey, HttpMethod, Description, SortOrder) VALUES
    ('View',   'GET',    'Read/list records', 1),
    ('Create', 'POST',   'Create new record', 2),
    ('Update', 'PUT',    'Update existing record', 3),
    ('Delete', 'DELETE', 'Delete record', 4),
    ('Approve','POST',   'Approve or finalize', 5),
    ('Export', 'GET',    'Export data', 6);
GO

-- Example core identities
TRUNCATE TABLE sec.Groups;
TRUNCATE TABLE sec.Roles;
TRUNCATE TABLE sec.Users;
INSERT INTO sec.Users (UserName, DisplayName, Email) VALUES
    ('admin', 'System Administrator', 'admin@example.com'),
    ('auditor', 'Audit Viewer', 'auditor@example.com');

INSERT INTO sec.Groups (GroupName, Description) VALUES
    ('IT', 'Information Technology'),
    ('Audit', 'Read-only auditing group');

INSERT INTO sec.Roles (RoleName, Description) VALUES
    ('System Admin', 'Full system control'),
    ('Reader', 'View-only access');

INSERT INTO sec.UserGroups (GroupId, UserId)
SELECT g.GroupId, u.UserId FROM sec.Groups g CROSS APPLY (SELECT UserId FROM sec.Users WHERE UserName IN ('admin','auditor')) u WHERE g.GroupName = 'IT';
INSERT INTO sec.UserGroups (GroupId, UserId)
SELECT g.GroupId, u.UserId FROM sec.Groups g CROSS APPLY (SELECT UserId FROM sec.Users WHERE UserName = 'auditor') u WHERE g.GroupName = 'Audit';

INSERT INTO sec.UserRoles (RoleId, UserId)
SELECT r.RoleId, u.UserId FROM sec.Roles r CROSS JOIN sec.Users u WHERE r.RoleName = 'System Admin' AND u.UserName = 'admin';
INSERT INTO sec.GroupRoles (GroupId, RoleId)
SELECT g.GroupId, r.RoleId FROM sec.Groups g CROSS JOIN sec.Roles r WHERE g.GroupName = 'Audit' AND r.RoleName = 'Reader';
GO

/* === Resource catalog seeded with MenuItems and APIs === */
-- Example resources (adjust MenuItemId/Url per environment)
TRUNCATE TABLE sec.Resources;
INSERT INTO sec.Resources (MenuItemId, Url, ControllerName, ActionName, Area, ResourceType, Description) VALUES
    (NULL, '/api/users', 'Users', 'Get', NULL, 'Api', 'List users'),
    (NULL, '/api/users', 'Users', 'Post', NULL, 'Api', 'Create user'),
    (NULL, '/api/menus', 'MenuItems', 'Get', NULL, 'Api', 'Read menu items'),
    (1, '/Administration/Menus', NULL, NULL, NULL, 'RazorPage', 'Menu maintenance page');
GO

-- Map default actions to resources (align with ActionKey catalog)
TRUNCATE TABLE sec.ResourceActions;
INSERT INTO sec.ResourceActions (ResourceId, ActionId, CustomKey)
SELECT r.ResourceId, a.ActionId, NULL
FROM sec.Resources r
CROSS APPLY (SELECT ActionId, ActionKey FROM sec.Actions) a
WHERE (r.ResourceType = 'Api' AND a.ActionKey IN ('View','Create','Update','Delete'))
   OR (r.ResourceType <> 'Api' AND a.ActionKey = 'View');
GO

-- Permissions per role
TRUNCATE TABLE sec.RolePermissions;
INSERT INTO sec.RolePermissions (RoleId, ResourceActionId, Effect)
SELECT r.RoleId, ra.ResourceActionId, 'A'
FROM sec.Roles r
CROSS JOIN sec.ResourceActions ra
WHERE r.RoleName = 'System Admin';

INSERT INTO sec.RolePermissions (RoleId, ResourceActionId, Effect)
SELECT r.RoleId, ra.ResourceActionId, 'A'
FROM sec.Roles r
INNER JOIN sec.ResourceActions ra ON ra.ActionId = (SELECT ActionId FROM sec.Actions WHERE ActionKey = 'View')
WHERE r.RoleName = 'Reader';
GO

/* === Helper function to evaluate access by URL/action === */
IF OBJECT_ID('sec.fnUserHasAccess') IS NOT NULL DROP FUNCTION sec.fnUserHasAccess;
GO
CREATE FUNCTION sec.fnUserHasAccess
(
    @UserId UNIQUEIDENTIFIER,
    @Url NVARCHAR(400) = NULL,
    @Controller NVARCHAR(150) = NULL,
    @Action NVARCHAR(150) = NULL,
    @Area NVARCHAR(100) = NULL,
    @ActionKey NVARCHAR(50) = 'View'
)
RETURNS BIT
AS
BEGIN
    DECLARE @result BIT = 0;

    DECLARE @resourceId INT = (
        SELECT TOP (1) ResourceId
        FROM sec.Resources r
        WHERE r.IsActive = 1
          AND (@Url IS NULL OR r.Url = @Url)
          AND (ISNULL(@Controller,'') = '' OR r.ControllerName = @Controller)
          AND (ISNULL(@Action,'') = '' OR r.ActionName = @Action)
          AND (ISNULL(@Area,'') = '' OR r.Area = @Area)
        ORDER BY ResourceId
    );

    IF @resourceId IS NULL RETURN 0;

    SELECT @result = MAX(CASE WHEN p.RoleAllow = 1 THEN 1 ELSE 0 END)
    FROM sec.vUserEffectivePermissions p
    INNER JOIN sec.ResourceActions ra ON ra.ResourceId = p.ResourceId AND ra.ActionId = (SELECT ActionId FROM sec.Actions WHERE ActionKey = @ActionKey)
    WHERE p.UserId = @UserId AND p.ResourceId = @resourceId;

    RETURN ISNULL(@result, 0);
END;
GO

/* === View for UI to list effective permissions per user === */
IF OBJECT_ID('sec.vUserPermissionMatrix') IS NOT NULL DROP VIEW sec.vUserPermissionMatrix;
GO
CREATE VIEW sec.vUserPermissionMatrix
AS
SELECT
    u.UserName,
    rsc.ResourceId,
    rsc.ResourceType,
    rsc.Url,
    rsc.ControllerName,
    rsc.ActionName,
    act.ActionKey,
    MAX(CASE WHEN p.RoleAllow = 1 THEN 1 ELSE 0 END) AS IsAllowed
FROM sec.Users u
CROSS JOIN sec.ResourceActions ra
INNER JOIN sec.Resources rsc ON ra.ResourceId = rsc.ResourceId
INNER JOIN sec.Actions act ON ra.ActionId = act.ActionId
LEFT JOIN sec.vUserEffectivePermissions p ON p.UserId = u.UserId AND p.ResourceId = rsc.ResourceId AND p.ActionId = ra.ActionId
GROUP BY u.UserName, rsc.ResourceId, rsc.ResourceType, rsc.Url, rsc.ControllerName, rsc.ActionName, act.ActionKey;
GO
