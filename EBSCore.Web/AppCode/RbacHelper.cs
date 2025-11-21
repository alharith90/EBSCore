using EBSCore.AdoClass.Security;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

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
            var permissionsDs = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                Operation: "rtvUserEffectivePermissions",
                CurrentUserID: user.UserID,
                UserID: user.UserID);
            var menusDs = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                Operation: "rtvMenuItemList",
                CurrentUserID: user.UserID);
            var actionsDs = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                Operation: "rtvActionList",
                CurrentUserID: user.UserID);

            var permissions = permissionsDs?.Tables.Count > 0 ? permissionsDs.Tables[0] : new DataTable();
            var menus = menusDs?.Tables.Count > 0 ? menusDs.Tables[0] : new DataTable();
            var actions = actionsDs?.Tables.Count > 0 ? actionsDs.Tables[0] : new DataTable();

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
