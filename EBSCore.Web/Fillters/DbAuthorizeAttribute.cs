using EBSCore.AdoClass.Security;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EBSCore.Web.Fillters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class DbAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public string MenuCode { get; set; }
        public string ActionCode { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var configuration = httpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var securitySP = new DBSecuritySP(configuration);
            var currentUser = httpContext.Session.GetObject<User>("User");

            if (currentUser == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var targetMenuCode = ResolveMenuCode(httpContext, MenuCode);
            var targetActionCode = string.IsNullOrWhiteSpace(ActionCode) ? "View" : ActionCode;

            DataTable menuTable = securitySP.RtvMenuItemList(currentUser.UserID);
            DataTable actionTable = securitySP.RtvActionList(currentUser.UserID);
            DataTable permissions = securitySP.RtvUserEffectivePermissions(currentUser.UserID, currentUser.UserID);

            var menuRow = menuTable.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("MenuCode"), targetMenuCode, StringComparison.OrdinalIgnoreCase));
            var actionRow = actionTable.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("ActionCode"), targetActionCode, StringComparison.OrdinalIgnoreCase));

            if (menuRow == null || actionRow == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            int menuId = menuRow.Field<int>("MenuItemID");
            int actionId = actionRow.Field<int>("ActionID");

            bool isAllowed = permissions
                .AsEnumerable()
                .Any(r => r.Field<int>("MenuItemID") == menuId && r.Field<int>("ActionID") == actionId && r.Field<int>("IsAllowed") == 1);

            if (!isAllowed)
            {
                if (context.Controller is ControllerBase)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    context.Result = new RedirectResult("/AccessDenied");
                }
                return;
            }

            await next();
        }

        private string ResolveMenuCode(Microsoft.AspNetCore.Http.HttpContext context, string menuCode)
        {
            if (!string.IsNullOrWhiteSpace(menuCode))
            {
                return menuCode;
            }

            var controller = context.Request.RouteValues["controller"]?.ToString();
            var action = context.Request.RouteValues["action"]?.ToString();
            return $"{controller}.{action}";
        }
    }
}
