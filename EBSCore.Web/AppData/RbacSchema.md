# RBAC schema usage

The `RbacSchema.sql` script provisions a database-driven RBAC model with menu-based resources, reusable actions, and unified CRUD logic in `sec.SecuritySP`. Run it once against the application database.

## Objects
- **sec.Role / sec.Group / sec.UserGroup / sec.RoleGroup / sec.RoleUser**: principal hierarchy with soft-delete and audit metadata.
- **sec.MenuItem / sec.Action / sec.MenuItemAction**: menu and endpoint catalog plus reusable actions.
- **sec.RolePermission / sec.UserPermissionOverride / sec.PermissionAudit**: allow/deny matrices, per-user overrides, and audit log entries.

## Stored procedure
`sec.SecuritySP` supports operations such as `rtvRoleList`, `rtvRoleDetails`, `saveRole`, `rtvGroupList`, `saveGroup`, `rtvMenuItemList`, `saveAction`, `assignRoleToGroup`, `assignRoleToUser`, `assignUserToGroup`, `saveRolePermission`, `rtvRolePermissions`, `rtvMenuActionsForRole`, `rtvMenuItemsForUser`, and `rtvUserEffectivePermissions`. Each operation lists columns explicitly and respects soft deletes.

## Seeding
The script seeds default actions (View, Add, Edit, Delete, Approve, Export, Custom1, Custom2), an Admin role/group, a sample Security menu tree, and grants the Admin role full access to all seeded menu/action pairs.

## Runtime enforcement
Use the `DbAuthorizeAttribute` (see code) on MVC/API controllers, or call `DBSecuritySP.RtvUserEffectivePermissions` to resolve whether a user can perform a given `MenuCode` + `ActionCode`. Razor components and menu rendering can call the same helper to hide or disable forbidden actions.
