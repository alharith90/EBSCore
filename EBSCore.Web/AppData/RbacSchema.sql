/*
    RBAC schema for EBS
    Run once on the target database. The script provisions the full data model,
    seeds default actions, menus, and admin permissions, and creates the unified
    SecuritySP stored procedure for all CRUD and permission lookups.
*/
GO

/* === Core tables === */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'sec')
    EXEC('CREATE SCHEMA sec');
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Role' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.Role
    (
        RoleID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Role PRIMARY KEY,
        RoleName NVARCHAR(200) NOT NULL,
        RoleCode NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NULL,
        StatusID INT NOT NULL CONSTRAINT DF_Role_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Role_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Role_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_Role_RoleName UNIQUE(RoleName),
        CONSTRAINT UQ_Role_RoleCode UNIQUE(RoleCode)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Group' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.[Group]
    (
        GroupID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Group PRIMARY KEY,
        GroupName NVARCHAR(200) NOT NULL,
        GroupCode NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NULL,
        StatusID INT NOT NULL CONSTRAINT DF_Group_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Group_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Group_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_Group_GroupName UNIQUE(GroupName),
        CONSTRAINT UQ_Group_GroupCode UNIQUE(GroupCode)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MenuItem' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.MenuItem
    (
        MenuItemID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MenuItem PRIMARY KEY,
        ParentMenuItemID INT NULL,
        MenuCode NVARCHAR(100) NOT NULL,
        MenuName NVARCHAR(200) NOT NULL,
        MenuNameKey NVARCHAR(200) NOT NULL,
        Url NVARCHAR(500) NULL,
        ApiRoute NVARCHAR(500) NULL,
        IconCssClass NVARCHAR(200) NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_MenuItem_DisplayOrder DEFAULT(1),
        IsVisible BIT NOT NULL CONSTRAINT DF_MenuItem_IsVisible DEFAULT(1),
        StatusID INT NOT NULL CONSTRAINT DF_MenuItem_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_MenuItem_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_MenuItem_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_MenuItem_MenuCode UNIQUE(MenuCode),
        CONSTRAINT FK_MenuItem_Parent FOREIGN KEY (ParentMenuItemID) REFERENCES sec.MenuItem(MenuItemID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Action' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.Action
    (
        ActionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Action PRIMARY KEY,
        ActionCode NVARCHAR(100) NOT NULL,
        ActionName NVARCHAR(200) NOT NULL,
        ActionNameKey NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        IsSystemDefault BIT NOT NULL CONSTRAINT DF_Action_IsSystemDefault DEFAULT(0),
        StatusID INT NOT NULL CONSTRAINT DF_Action_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Action_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Action_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_Action_ActionCode UNIQUE(ActionCode)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MenuItemAction' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.MenuItemAction
    (
        MenuItemActionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MenuItemAction PRIMARY KEY,
        MenuItemID INT NOT NULL,
        ActionID INT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_MenuItemAction_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_MenuItemAction_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_MenuItemAction_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_MenuItemAction UNIQUE(MenuItemID, ActionID),
        CONSTRAINT FK_MenuItemAction_MenuItem FOREIGN KEY (MenuItemID) REFERENCES sec.MenuItem(MenuItemID),
        CONSTRAINT FK_MenuItemAction_Action FOREIGN KEY (ActionID) REFERENCES sec.Action(ActionID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RolePermission' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.RolePermission
    (
        RolePermissionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RolePermission PRIMARY KEY,
        RoleID INT NOT NULL,
        MenuItemID INT NOT NULL,
        ActionID INT NOT NULL,
        IsAllowed BIT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_RolePermission_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_RolePermission_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_RolePermission_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_RolePermission UNIQUE(RoleID, MenuItemID, ActionID),
        CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleID) REFERENCES sec.Role(RoleID),
        CONSTRAINT FK_RolePermission_MenuItem FOREIGN KEY (MenuItemID) REFERENCES sec.MenuItem(MenuItemID),
        CONSTRAINT FK_RolePermission_Action FOREIGN KEY (ActionID) REFERENCES sec.Action(ActionID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserGroup' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.UserGroup
    (
        UserGroupID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserGroup PRIMARY KEY,
        UserID INT NOT NULL,
        GroupID INT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_UserGroup_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_UserGroup_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_UserGroup_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_UserGroup UNIQUE(UserID, GroupID),
        CONSTRAINT FK_UserGroup_Group FOREIGN KEY (GroupID) REFERENCES sec.[Group](GroupID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RoleGroup' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.RoleGroup
    (
        RoleGroupID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RoleGroup PRIMARY KEY,
        RoleID INT NOT NULL,
        GroupID INT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_RoleGroup_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_RoleGroup_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_RoleGroup_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_RoleGroup UNIQUE(RoleID, GroupID),
        CONSTRAINT FK_RoleGroup_Role FOREIGN KEY (RoleID) REFERENCES sec.Role(RoleID),
        CONSTRAINT FK_RoleGroup_Group FOREIGN KEY (GroupID) REFERENCES sec.[Group](GroupID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RoleUser' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.RoleUser
    (
        RoleUserID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RoleUser PRIMARY KEY,
        RoleID INT NOT NULL,
        UserID INT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_RoleUser_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_RoleUser_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_RoleUser_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_RoleUser UNIQUE(RoleID, UserID),
        CONSTRAINT FK_RoleUser_Role FOREIGN KEY (RoleID) REFERENCES sec.Role(RoleID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserPermissionOverride' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.UserPermissionOverride
    (
        UserPermissionOverrideID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserPermissionOverride PRIMARY KEY,
        UserID INT NOT NULL,
        MenuItemID INT NOT NULL,
        ActionID INT NOT NULL,
        IsAllowed BIT NOT NULL,
        StatusID INT NOT NULL CONSTRAINT DF_UserPermissionOverride_StatusID DEFAULT(1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_UserPermissionOverride_IsDeleted DEFAULT(0),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_UserPermissionOverride_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT UQ_UserPermissionOverride UNIQUE(UserID, MenuItemID, ActionID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PermissionAudit' AND schema_id = SCHEMA_ID('sec'))
BEGIN
    CREATE TABLE sec.PermissionAudit
    (
        PermissionAuditID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PermissionAudit PRIMARY KEY,
        ChangedBy INT NOT NULL,
        ChangedAt DATETIME2(0) NOT NULL CONSTRAINT DF_PermissionAudit_ChangedAt DEFAULT SYSUTCDATETIME(),
        EntityType NVARCHAR(100) NOT NULL,
        EntityID INT NOT NULL,
        ActionTaken NVARCHAR(200) NOT NULL,
        Details NVARCHAR(MAX) NULL
    );
END
GO

/* === Indexes === */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MenuItemAction_MenuItemID_ActionID' AND object_id = OBJECT_ID('sec.MenuItemAction'))
    CREATE NONCLUSTERED INDEX IX_MenuItemAction_MenuItemID_ActionID ON sec.MenuItemAction(MenuItemID, ActionID);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RolePermission_Role_Menu_Action' AND object_id = OBJECT_ID('sec.RolePermission'))
    CREATE NONCLUSTERED INDEX IX_RolePermission_Role_Menu_Action ON sec.RolePermission(RoleID, MenuItemID, ActionID);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserGroup_User_Group' AND object_id = OBJECT_ID('sec.UserGroup'))
    CREATE NONCLUSTERED INDEX IX_UserGroup_User_Group ON sec.UserGroup(UserID, GroupID);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RoleGroup_Role_Group' AND object_id = OBJECT_ID('sec.RoleGroup'))
    CREATE NONCLUSTERED INDEX IX_RoleGroup_Role_Group ON sec.RoleGroup(RoleID, GroupID);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RoleUser_Role_User' AND object_id = OBJECT_ID('sec.RoleUser'))
    CREATE NONCLUSTERED INDEX IX_RoleUser_Role_User ON sec.RoleUser(RoleID, UserID);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserPermissionOverride_User_Menu_Action' AND object_id = OBJECT_ID('sec.UserPermissionOverride'))
    CREATE NONCLUSTERED INDEX IX_UserPermissionOverride_User_Menu_Action ON sec.UserPermissionOverride(UserID, MenuItemID, ActionID);
GO

/* === Stored procedure === */
IF OBJECT_ID('sec.SecuritySP') IS NOT NULL
    DROP PROCEDURE sec.SecuritySP;
GO

CREATE PROCEDURE sec.SecuritySP
    @Operation NVARCHAR(100),
    @CurrentUserID INT,
    @RoleID INT = NULL,
    @GroupID INT = NULL,
    @UserID INT = NULL,
    @MenuItemID INT = NULL,
    @ActionID INT = NULL,
    @IsAllowed BIT = NULL,
    @StatusID INT = NULL,
    @IsDeleted BIT = NULL,
    @RoleName NVARCHAR(200) = NULL,
    @RoleCode NVARCHAR(100) = NULL,
    @RoleDescription NVARCHAR(1000) = NULL,
    @GroupName NVARCHAR(200) = NULL,
    @GroupCode NVARCHAR(100) = NULL,
    @GroupDescription NVARCHAR(1000) = NULL,
    @MenuCode NVARCHAR(100) = NULL,
    @MenuName NVARCHAR(200) = NULL,
    @MenuNameKey NVARCHAR(200) = NULL,
    @Url NVARCHAR(500) = NULL,
    @ApiRoute NVARCHAR(500) = NULL,
    @IconCssClass NVARCHAR(200) = NULL,
    @DisplayOrder INT = NULL,
    @IsVisible BIT = NULL,
    @ParentMenuItemID INT = NULL,
    @ActionCode NVARCHAR(100) = NULL,
    @ActionName NVARCHAR(200) = NULL,
    @ActionNameKey NVARCHAR(200) = NULL,
    @ActionDescription NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'rtvRoleList'
    BEGIN
        SELECT RoleID, RoleName, RoleCode, Description, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.Role
        WHERE IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'rtvRoleDetails'
    BEGIN
        SELECT RoleID, RoleName, RoleCode, Description, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.Role
        WHERE RoleID = @RoleID AND IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'saveRole'
    BEGIN
        IF EXISTS (SELECT 1 FROM sec.Role WHERE RoleID = @RoleID)
        BEGIN
            UPDATE sec.Role
            SET RoleName = @RoleName,
                RoleCode = @RoleCode,
                Description = @RoleDescription,
                StatusID = ISNULL(@StatusID, StatusID),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE RoleID = @RoleID;
        END
        ELSE
        BEGIN
            INSERT INTO sec.Role(RoleName, RoleCode, Description, StatusID, IsDeleted, CreatedBy)
            VALUES(@RoleName, @RoleCode, @RoleDescription, ISNULL(@StatusID,1), 0, @CurrentUserID);
            SET @RoleID = SCOPE_IDENTITY();
        END

        SELECT @RoleID AS RoleID;
        RETURN;
    END

    IF @Operation = 'deleteRole'
    BEGIN
        UPDATE sec.Role
        SET IsDeleted = 1, UpdatedBy = @CurrentUserID, UpdatedAt = SYSUTCDATETIME()
        WHERE RoleID = @RoleID;
        RETURN;
    END

    IF @Operation = 'rtvGroupList'
    BEGIN
        SELECT GroupID, GroupName, GroupCode, Description, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.[Group]
        WHERE IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'rtvGroupDetails'
    BEGIN
        SELECT GroupID, GroupName, GroupCode, Description, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.[Group]
        WHERE GroupID = @GroupID AND IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'saveGroup'
    BEGIN
        IF EXISTS (SELECT 1 FROM sec.[Group] WHERE GroupID = @GroupID)
        BEGIN
            UPDATE sec.[Group]
            SET GroupName = @GroupName,
                GroupCode = @GroupCode,
                Description = @GroupDescription,
                StatusID = ISNULL(@StatusID, StatusID),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE GroupID = @GroupID;
        END
        ELSE
        BEGIN
            INSERT INTO sec.[Group](GroupName, GroupCode, Description, StatusID, IsDeleted, CreatedBy)
            VALUES(@GroupName, @GroupCode, @GroupDescription, ISNULL(@StatusID,1), 0, @CurrentUserID);
            SET @GroupID = SCOPE_IDENTITY();
        END

        SELECT @GroupID AS GroupID;
        RETURN;
    END

    IF @Operation = 'deleteGroup'
    BEGIN
        UPDATE sec.[Group]
        SET IsDeleted = 1, UpdatedBy = @CurrentUserID, UpdatedAt = SYSUTCDATETIME()
        WHERE GroupID = @GroupID;
        RETURN;
    END

    IF @Operation = 'rtvMenuItemList'
    BEGIN
        SELECT MenuItemID, ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.MenuItem
        WHERE IsDeleted = 0
        ORDER BY DisplayOrder;
        RETURN;
    END

    IF @Operation = 'saveMenuItem'
    BEGIN
        IF EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuItemID = @MenuItemID)
        BEGIN
            UPDATE sec.MenuItem
            SET ParentMenuItemID = @ParentMenuItemID,
                MenuCode = @MenuCode,
                MenuName = @MenuName,
                MenuNameKey = @MenuNameKey,
                Url = @Url,
                ApiRoute = @ApiRoute,
                IconCssClass = @IconCssClass,
                DisplayOrder = ISNULL(@DisplayOrder, DisplayOrder),
                IsVisible = ISNULL(@IsVisible, IsVisible),
                StatusID = ISNULL(@StatusID, StatusID),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE MenuItemID = @MenuItemID;
        END
        ELSE
        BEGIN
            INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
            VALUES(@ParentMenuItemID, @MenuCode, @MenuName, @MenuNameKey, @Url, @ApiRoute, @IconCssClass, ISNULL(@DisplayOrder,1), ISNULL(@IsVisible,1), ISNULL(@StatusID,1), 0, @CurrentUserID);
            SET @MenuItemID = SCOPE_IDENTITY();
        END

        SELECT @MenuItemID AS MenuItemID;
        RETURN;
    END

    IF @Operation = 'rtvActionList'
    BEGIN
        SELECT ActionID, ActionCode, ActionName, ActionNameKey, Description, IsSystemDefault, StatusID, IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
        FROM sec.Action
        WHERE IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'saveAction'
    BEGIN
        IF EXISTS (SELECT 1 FROM sec.Action WHERE ActionID = @ActionID)
        BEGIN
            UPDATE sec.Action
            SET ActionCode = @ActionCode,
                ActionName = @ActionName,
                ActionNameKey = @ActionNameKey,
                Description = @ActionDescription,
                StatusID = ISNULL(@StatusID, StatusID),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE ActionID = @ActionID;
        END
        ELSE
        BEGIN
            INSERT INTO sec.Action(ActionCode, ActionName, ActionNameKey, Description, IsSystemDefault, StatusID, IsDeleted, CreatedBy)
            VALUES(@ActionCode, @ActionName, @ActionNameKey, @ActionDescription, 0, ISNULL(@StatusID,1), 0, @CurrentUserID);
            SET @ActionID = SCOPE_IDENTITY();
        END

        SELECT @ActionID AS ActionID;
        RETURN;
    END

    IF @Operation = 'assignRoleToGroup'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sec.RoleGroup WHERE RoleID = @RoleID AND GroupID = @GroupID)
        BEGIN
            INSERT INTO sec.RoleGroup(RoleID, GroupID, StatusID, IsDeleted, CreatedBy)
            VALUES(@RoleID, @GroupID, ISNULL(@StatusID,1), 0, @CurrentUserID);
        END
        ELSE
        BEGIN
            UPDATE sec.RoleGroup
            SET StatusID = ISNULL(@StatusID, StatusID),
                IsDeleted = ISNULL(@IsDeleted, IsDeleted),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE RoleID = @RoleID AND GroupID = @GroupID;
        END
        RETURN;
    END

    IF @Operation = 'assignRoleToUser'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sec.RoleUser WHERE RoleID = @RoleID AND UserID = @UserID)
        BEGIN
            INSERT INTO sec.RoleUser(RoleID, UserID, StatusID, IsDeleted, CreatedBy)
            VALUES(@RoleID, @UserID, ISNULL(@StatusID,1), 0, @CurrentUserID);
        END
        ELSE
        BEGIN
            UPDATE sec.RoleUser
            SET StatusID = ISNULL(@StatusID, StatusID),
                IsDeleted = ISNULL(@IsDeleted, IsDeleted),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE RoleID = @RoleID AND UserID = @UserID;
        END
        RETURN;
    END

    IF @Operation = 'assignUserToGroup'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sec.UserGroup WHERE GroupID = @GroupID AND UserID = @UserID)
        BEGIN
            INSERT INTO sec.UserGroup(UserID, GroupID, StatusID, IsDeleted, CreatedBy)
            VALUES(@UserID, @GroupID, ISNULL(@StatusID,1), 0, @CurrentUserID);
        END
        ELSE
        BEGIN
            UPDATE sec.UserGroup
            SET StatusID = ISNULL(@StatusID, StatusID),
                IsDeleted = ISNULL(@IsDeleted, IsDeleted),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE GroupID = @GroupID AND UserID = @UserID;
        END
        RETURN;
    END

    IF @Operation = 'saveRolePermission'
    BEGIN
        DECLARE @RolePermissionKey INT;
        IF NOT EXISTS (SELECT 1 FROM sec.RolePermission WHERE RoleID = @RoleID AND MenuItemID = @MenuItemID AND ActionID = @ActionID)
        BEGIN
            INSERT INTO sec.RolePermission(RoleID, MenuItemID, ActionID, IsAllowed, StatusID, IsDeleted, CreatedBy)
            VALUES(@RoleID, @MenuItemID, @ActionID, @IsAllowed, ISNULL(@StatusID,1), 0, @CurrentUserID);
            SET @RolePermissionKey = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE sec.RolePermission
            SET IsAllowed = @IsAllowed,
                StatusID = ISNULL(@StatusID, StatusID),
                IsDeleted = ISNULL(@IsDeleted, IsDeleted),
                UpdatedBy = @CurrentUserID,
                UpdatedAt = SYSUTCDATETIME()
            WHERE RoleID = @RoleID AND MenuItemID = @MenuItemID AND ActionID = @ActionID;
            SELECT @RolePermissionKey = RolePermissionID FROM sec.RolePermission WHERE RoleID = @RoleID AND MenuItemID = @MenuItemID AND ActionID = @ActionID;
        END

        INSERT INTO sec.PermissionAudit(ChangedBy, EntityType, EntityID, ActionTaken, Details)
        VALUES(@CurrentUserID, 'RolePermission', @RolePermissionKey, 'saveRolePermission', CONCAT('Role=', @RoleID, ';Menu=', @MenuItemID, ';Action=', @ActionID, ';Allow=', @IsAllowed));
        RETURN;
    END

    IF @Operation = 'rtvRolePermissions'
    BEGIN
        SELECT rp.RolePermissionID, rp.RoleID, rp.MenuItemID, rp.ActionID, rp.IsAllowed, rp.StatusID, rp.IsDeleted, rp.CreatedBy, rp.CreatedAt, rp.UpdatedBy, rp.UpdatedAt
        FROM sec.RolePermission rp
        WHERE rp.RoleID = @RoleID AND rp.IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'rtvMenuActionsForRole'
    BEGIN
        SELECT mia.MenuItemActionID, mia.MenuItemID, mia.ActionID, ISNULL(rp.IsAllowed, 0) AS IsAllowed
        FROM sec.MenuItemAction mia
        LEFT JOIN sec.RolePermission rp ON rp.RoleID = @RoleID AND rp.MenuItemID = mia.MenuItemID AND rp.ActionID = mia.ActionID AND rp.IsDeleted = 0
        WHERE mia.IsDeleted = 0;
        RETURN;
    END

    IF @Operation = 'rtvMenuItemsForUser'
    BEGIN
        ;WITH UserRoles AS
        (
            SELECT ru.RoleID FROM sec.RoleUser ru WHERE ru.UserID = @UserID AND ru.IsDeleted = 0 AND ru.StatusID = 1
            UNION
            SELECT rg.RoleID FROM sec.RoleGroup rg INNER JOIN sec.UserGroup ug ON ug.GroupID = rg.GroupID WHERE ug.UserID = @UserID AND rg.IsDeleted = 0 AND rg.StatusID = 1 AND ug.IsDeleted = 0 AND ug.StatusID = 1
        ),
        AllowedMenus AS
        (
            SELECT DISTINCT rp.MenuItemID
            FROM sec.RolePermission rp
            INNER JOIN UserRoles ur ON ur.RoleID = rp.RoleID
            WHERE rp.IsAllowed = 1 AND rp.IsDeleted = 0 AND rp.StatusID = 1
        )
        SELECT mi.MenuItemID, mi.ParentMenuItemID, mi.MenuCode, mi.MenuName, mi.MenuNameKey, mi.Url, mi.ApiRoute, mi.IconCssClass, mi.DisplayOrder, mi.IsVisible, mi.StatusID, mi.IsDeleted, mi.CreatedBy, mi.CreatedAt, mi.UpdatedBy, mi.UpdatedAt
        FROM sec.MenuItem mi
        WHERE mi.IsDeleted = 0 AND mi.MenuItemID IN (SELECT MenuItemID FROM AllowedMenus)
        ORDER BY mi.DisplayOrder;
        RETURN;
    END

    IF @Operation = 'rtvUserEffectivePermissions'
    BEGIN
        ;WITH UserRoles AS
        (
            SELECT ru.RoleID FROM sec.RoleUser ru WHERE ru.UserID = @UserID AND ru.IsDeleted = 0 AND ru.StatusID = 1
            UNION
            SELECT rg.RoleID FROM sec.RoleGroup rg INNER JOIN sec.UserGroup ug ON ug.GroupID = rg.GroupID WHERE ug.UserID = @UserID AND rg.IsDeleted = 0 AND rg.StatusID = 1 AND ug.IsDeleted = 0 AND ug.StatusID = 1
        ),
        RoleAllow AS
        (
            SELECT rp.MenuItemID, rp.ActionID, rp.IsAllowed
            FROM sec.RolePermission rp
            INNER JOIN UserRoles ur ON ur.RoleID = rp.RoleID
            WHERE rp.IsDeleted = 0 AND rp.StatusID = 1
        )
        SELECT mia.MenuItemID, mia.ActionID, MAX(CAST(CASE WHEN upo.IsAllowed IS NOT NULL THEN upo.IsAllowed ELSE ra.IsAllowed END AS INT)) AS IsAllowed
        FROM sec.MenuItemAction mia
        LEFT JOIN RoleAllow ra ON ra.MenuItemID = mia.MenuItemID AND ra.ActionID = mia.ActionID
        LEFT JOIN sec.UserPermissionOverride upo ON upo.UserID = @UserID AND upo.MenuItemID = mia.MenuItemID AND upo.ActionID = mia.ActionID AND upo.IsDeleted = 0 AND upo.StatusID = 1
        WHERE mia.IsDeleted = 0
        GROUP BY mia.MenuItemID, mia.ActionID;
        RETURN;
    END
END
GO

/* === Seeds === */
DECLARE @AdminUserID INT = 1; -- adjust to actual admin user id in production

INSERT INTO sec.Action(ActionCode, ActionName, ActionNameKey, Description, IsSystemDefault, StatusID, IsDeleted, CreatedBy)
SELECT v.ActionCode, v.ActionName, v.ActionNameKey, v.Description, 1, 1, 0, @AdminUserID
FROM (VALUES
    ('View', 'View', 'ActionView', 'View records'),
    ('Add', 'Add', 'ActionAdd', 'Create records'),
    ('Edit', 'Edit', 'ActionEdit', 'Update records'),
    ('Delete', 'Delete', 'ActionDelete', 'Delete records'),
    ('Approve', 'Approve', 'ActionApprove', 'Approve changes'),
    ('Export', 'Export', 'ActionExport', 'Export data'),
    ('Custom1', 'Custom1', 'ActionCustom1', 'Custom action 1'),
    ('Custom2', 'Custom2', 'ActionCustom2', 'Custom action 2')
) AS v(ActionCode, ActionName, ActionNameKey, Description)
WHERE NOT EXISTS (SELECT 1 FROM sec.Action a WHERE a.ActionCode = v.ActionCode);

INSERT INTO sec.Action(ActionCode, ActionName, ActionNameKey, Description, StatusID, IsDeleted, CreatedBy)
SELECT v.ActionCode, v.ActionName, v.ActionNameKey, v.Description, 1, 0, @AdminUserID
FROM (VALUES
    ('ManageMatrix', 'Manage Matrix', 'ActionManageMatrix', 'Configure risk matrices and tolerances'),
    ('AddRisk', 'Add Risk', 'ActionAddRisk', 'Create BCM risk records'),
    ('EditRisk', 'Edit Risk', 'ActionEditRisk', 'Update BCM risk records'),
    ('ApproveRisk', 'Approve Risk', 'ActionApproveRisk', 'Approve BCM risks')
) AS v(ActionCode, ActionName, ActionNameKey, Description)
WHERE NOT EXISTS (SELECT 1 FROM sec.Action a WHERE a.ActionCode = v.ActionCode);

IF NOT EXISTS (SELECT 1 FROM sec.Role WHERE RoleCode = 'ADMIN')
BEGIN
    INSERT INTO sec.Role(RoleName, RoleCode, Description, StatusID, IsDeleted, CreatedBy)
    VALUES('Administrator', 'ADMIN', 'Full access role', 1, 0, @AdminUserID);
END

IF NOT EXISTS (SELECT 1 FROM sec.[Group] WHERE GroupCode = 'ADMINS')
BEGIN
    INSERT INTO sec.[Group](GroupName, GroupCode, Description, StatusID, IsDeleted, CreatedBy)
    VALUES('Administrators', 'ADMINS', 'Admin group', 1, 0, @AdminUserID);
END

DECLARE @RootMenuID INT;
DECLARE @SecurityMenuID INT;
DECLARE @BCMMenuID INT;
DECLARE @RiskMenuID INT;
DECLARE @RiskMatrixMenuID INT;

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Dashboard')
BEGIN
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(NULL, 'Dashboard', 'Dashboard', 'MenuDashboard', '/dashboard', NULL, 'fas fa-home', 1, 1, 1, 0, @AdminUserID);
END

SELECT @RootMenuID = MenuItemID FROM sec.MenuItem WHERE MenuCode = 'Dashboard';

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Security')
BEGIN
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@RootMenuID, 'Security', 'Security Management', 'MenuSecurity', '/Security', NULL, 'fas fa-shield-alt', 2, 1, 1, 0, @AdminUserID);
END

SELECT @SecurityMenuID = MenuItemID FROM sec.MenuItem WHERE MenuCode = 'Security';

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'BCM')
BEGIN
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@RootMenuID, 'BCM', 'Business Continuity', 'MenuBCM', '/BCM', NULL, 'fas fa-life-ring', 3, 1, 1, 0, @AdminUserID);
END

SELECT @BCMMenuID = MenuItemID FROM sec.MenuItem WHERE MenuCode = 'BCM';

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'BCM.Risk')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@BCMMenuID, 'BCM.Risk', 'Risk Register', 'RiskRegister', '/BCM/RiskIndex', '/api/S7SRisk/Get', 'fas fa-fire', 1, 1, 1, 0, @AdminUserID);

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'BCM.Risk.Matrix')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@BCMMenuID, 'BCM.Risk.Matrix', 'Risk Matrix', 'RiskMatrixConfiguration', '/BCM/RiskMatrixConfiguration', '/api/S7SRisk/MatrixConfig', 'fas fa-th-large', 2, 1, 1, 0, @AdminUserID);

SELECT @RiskMenuID = MenuItemID FROM sec.MenuItem WHERE MenuCode = 'BCM.Risk';
SELECT @RiskMatrixMenuID = MenuItemID FROM sec.MenuItem WHERE MenuCode = 'BCM.Risk.Matrix';

IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Security.Roles')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@SecurityMenuID, 'Security.Roles', 'Roles', 'MenuRoles', '/Security/Roles', '/api/Security/IndexRoles', 'fas fa-user-shield', 1, 1, 1, 0, @AdminUserID);
IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Security.Groups')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@SecurityMenuID, 'Security.Groups', 'Groups', 'MenuGroups', '/Security/Groups', '/api/Security/IndexGroups', 'fas fa-users', 2, 1, 1, 0, @AdminUserID);
IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Security.Permissions')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@SecurityMenuID, 'Security.Permissions', 'Permissions', 'MenuPermissions', '/Security/RolePermissions', '/api/Security/RolePermissions', 'fas fa-lock', 3, 1, 1, 0, @AdminUserID);
IF NOT EXISTS (SELECT 1 FROM sec.MenuItem WHERE MenuCode = 'Security.Assignments')
    INSERT INTO sec.MenuItem(ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy)
    VALUES(@SecurityMenuID, 'Security.Assignments', 'Assignments', 'MenuAssignments', '/Security/Assignments', '/api/Security/Assignments', 'fas fa-link', 4, 1, 1, 0, @AdminUserID);

INSERT INTO sec.MenuItemAction(MenuItemID, ActionID, StatusID, IsDeleted, CreatedBy)
SELECT mi.MenuItemID, a.ActionID, 1, 0, @AdminUserID
FROM sec.MenuItem mi
CROSS JOIN sec.Action a
WHERE mi.MenuCode LIKE 'Security.%'
AND NOT EXISTS (SELECT 1 FROM sec.MenuItemAction mia WHERE mia.MenuItemID = mi.MenuItemID AND mia.ActionID = a.ActionID);

INSERT INTO sec.MenuItemAction(MenuItemID, ActionID, StatusID, IsDeleted, CreatedBy)
SELECT @RiskMenuID, a.ActionID, 1, 0, @AdminUserID
FROM sec.Action a
WHERE a.ActionCode IN ('AddRisk', 'EditRisk', 'ApproveRisk')
AND NOT EXISTS (SELECT 1 FROM sec.MenuItemAction mia WHERE mia.MenuItemID = @RiskMenuID AND mia.ActionID = a.ActionID);

INSERT INTO sec.MenuItemAction(MenuItemID, ActionID, StatusID, IsDeleted, CreatedBy)
SELECT @RiskMatrixMenuID, a.ActionID, 1, 0, @AdminUserID
FROM sec.Action a
WHERE a.ActionCode IN ('ManageMatrix')
AND NOT EXISTS (SELECT 1 FROM sec.MenuItemAction mia WHERE mia.MenuItemID = @RiskMatrixMenuID AND mia.ActionID = a.ActionID);

DECLARE @AdminRoleID INT = (SELECT RoleID FROM sec.Role WHERE RoleCode = 'ADMIN');
DECLARE @AdminGroupID INT = (SELECT GroupID FROM sec.[Group] WHERE GroupCode = 'ADMINS');

IF NOT EXISTS (SELECT 1 FROM sec.RoleGroup WHERE RoleID = @AdminRoleID AND GroupID = @AdminGroupID)
BEGIN
    INSERT INTO sec.RoleGroup(RoleID, GroupID, StatusID, IsDeleted, CreatedBy)
    VALUES(@AdminRoleID, @AdminGroupID, 1, 0, @AdminUserID);
END

INSERT INTO sec.RolePermission(RoleID, MenuItemID, ActionID, IsAllowed, StatusID, IsDeleted, CreatedBy)
SELECT @AdminRoleID, mia.MenuItemID, mia.ActionID, 1, 1, 0, @AdminUserID
FROM sec.MenuItemAction mia
WHERE NOT EXISTS (
    SELECT 1 FROM sec.RolePermission rp WHERE rp.RoleID = @AdminRoleID AND rp.MenuItemID = mia.MenuItemID AND rp.ActionID = mia.ActionID
);

GO
