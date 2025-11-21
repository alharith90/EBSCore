using EBSCore.AdoClass.Security;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;

namespace EBSCore.Web.AppCode
{
    public static class RbacHelper
    {
        public static bool HasPermission(IHttpContextAccessor accessor, IConfiguration configuration, string menuCode, string actionCode)
        {
            var context = accessor.HttpContext;
            var user = context.Session.GetObject<User>("User");
            if (user == null)
            {
                return false;
            }

            var securitySP = new DBSecuritySP(configuration);
            var permissions = securitySP.RtvUserEffectivePermissions(user.UserID, user.UserID);
            var menus = securitySP.RtvMenuItemList(user.UserID);
            var actions = securitySP.RtvActionList(user.UserID);

            var menuRow = menus.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("MenuCode"), menuCode, StringComparison.OrdinalIgnoreCase));
            var actionRow = actions.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("ActionCode"), actionCode, StringComparison.OrdinalIgnoreCase));

            if (menuRow == null || actionRow == null)
            {
                return false;
            }

            int menuId = menuRow.Field<int>("MenuItemID");
            int actionId = actionRow.Field<int>("ActionID");

            return permissions.AsEnumerable().Any(r => r.Field<int>("MenuItemID") == menuId && r.Field<int>("ActionID") == actionId && r.Field<int>("IsAllowed") == 1);
        }
    }
}
