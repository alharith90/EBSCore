# RBAC schema usage

The `RbacSchema.sql` script provisions a complete role-based access-control model that ties API endpoints, Razor pages, and menu items together. The schema is meant to be executed once in the `EBS` database and can be extended without code changes.

## Objects
- **sec.Users / sec.Groups / sec.Roles**: security principals and memberships.
- **sec.Resources**: catalog of secured URLs or menu items; supports `Api`, `RazorPage`, and `ViewComponent` types with optional `MenuItemId` links to `dbo.MenuItems`.
- **sec.Actions**: reusable action catalog (defaults: `View`, `Create`, `Update`, `Delete`, `Approve`, `Export`).
- **sec.ResourceActions**: links resources to permitted actions (e.g., CRUD for APIs, `View` for Razor pages).
- **sec.RolePermissions / sec.UserOverrides**: allow/deny at action level; overrides take priority.
- **Views & Function**: `sec.vUserEffectiveRoles`, `sec.vUserEffectivePermissions`, `sec.vUserPermissionMatrix`, and `sec.fnUserHasAccess` simplify lookups from C# filters.

## Controller filter pattern
Use an action filter that resolves the calling user id (e.g., `HttpContext.User.Identity.Name`) to `sec.Users.UserName`, then verifies access before executing the controller action:

```csharp
public class RbacAuthorizeAttribute : IAsyncActionFilter
{
    private readonly IDbConnection _connection;

    public RbacAuthorizeAttribute(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userName = context.HttpContext.User.Identity?.Name;
        var controller = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var verb = context.HttpContext.Request.Method;
        var actionKey = verb switch
        {
            "GET" => "View",
            "POST" => actionName?.Contains("Delete") == true ? "Delete" : "Create",
            "PUT" or "PATCH" => "Update",
            _ => "View"
        };

        var sql = "SELECT sec.fnUserHasAccess(u.UserId, @Url, @Controller, @Action, @Area, @ActionKey) FROM sec.Users u WHERE u.UserName = @UserName";
        var allowed = await _connection.ExecuteScalarAsync<bool>(sql, new
        {
            Url = context.HttpContext.Request.Path.Value,
            Controller = controller,
            Action = actionName,
            Area = context.RouteData.Values["area"]?.ToString(),
            ActionKey = actionKey,
            UserName = userName
        });

        if (!allowed)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
```

Apply the attribute at controller or action level and optionally feed custom `ActionKey` values (e.g., `Approve`) when the action represents a domain-specific operation.

## Razor view enforcement
- Load the `sec.vUserPermissionMatrix` rows for the current user into the layout (e.g., via a view component or middleware) and expose them as a claim or `ViewBag` item.
- Hide UI elements by testing for a matching resource/action pair and disabling or removing buttons when `IsAllowed = 0`.
- When showing menu items, filter the tree using `MenuItemId` or `Url` values that map to allowed resources.

## Seeding
`RbacSchema.sql` seeds:
- Default actions (View/Create/Update/Delete/Approve/Export).
- Admin and read-only roles.
- Example resources tied to `/api/users`, `/api/menus`, and the administration menu page.
- Grants: the admin role gets all actions; the reader role gets `View` only.

Extend the seeds by adding rows to `sec.Resources` (with URLs or controller/action names) and new `sec.ResourceActions` entries for each custom action you expose in controllers or Razor pages.
