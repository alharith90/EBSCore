

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using Newtonsoft.Json;
using System.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Fillters
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = (string)context.RouteData.Values["action"].ToString();
            var controllerName = (string)context.RouteData.Values["controller"];

            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var publicActions = configuration.GetSection("PublicActions").Get<List<PublicAction>>();
            var objCommon = new Common();
            if (publicActions != null && publicActions.Any(pa => pa.Controller.Equals(controllerName, StringComparison.OrdinalIgnoreCase) && pa.Action.Equals(actionName, StringComparison.OrdinalIgnoreCase)))
            {
                // This action is public, do nothing.
                return;
            }


            // Auto Login Settings
            bool isAutoLogin = bool.Parse(configuration["EnableAutoLogin"]);
            if (isAutoLogin)
            {
                var objUser = new DBUserSP(configuration);
                var user = new Models.User();

                string autoLoginUser = configuration["AutoLoginUser"];
                string autoLoginPassword = configuration["AutoLoginPassword"];
                var dsUser = (DataSet)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    "LoginWithNoHistory", null, null, autoLoginUser, autoLoginPassword);

                if (!objCommon.IsEmptyDataSet(dsUser))
                {
                    var rowUser = dsUser.Tables[0].Rows[0];
                    user.UserID = rowUser["UserID"].ToString();
                    user.Email = rowUser["Email"].ToString();
                    user.UserFullName = rowUser["UserFullName"].ToString();
                    user.CompanyID = rowUser["CompanyID"].ToString();
                    user.CategoryID = rowUser["CategoryID"].ToString();
                    user.UserType = (UserType)rowUser["UserType"];
                    user.UserName = rowUser["UserName"].ToString();
                    user.UserImage = rowUser["UserImage"].ToString();
                    user.CompanyName = rowUser["CompanyName"].ToString();
                    context.HttpContext.Session.SetObject("User", user);
                }
            }

            // Check for current user session
            var currentUser = context.HttpContext.Session.GetObject<Models.User>("User");
            if (currentUser == null)
            {
                if (objCommon.IsApiController(context))
                {
                    // Throw an exception if it's an API controller
                    throw new UnauthorizedAccessException("User is not authorized.");
                }
                else
                {
                    ReturnToView(context, "Login", "User");
                }
            }
            else
            {
                DBMenuItemsSP MenuItemSP = new DBMenuItemsSP(configuration);
                DataSet DSResult = (DataSet)MenuItemSP.QueryDatabase(SqlQueryType.FillDataset,
                Operation: "rtvMenuItems",
                UserID: currentUser.UserID);

                    var controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
                    if (controller != null)
                    {
                        controller.ViewBag.MenuItems = DSResult.Tables[0];
                    }

            }
        }

        protected void ReturnToView(ActionExecutingContext context, string viewName, string controller)
        {
            context.RouteData.Values["controller"] = controller;
            context.RouteData.Values["action"] = viewName;
            context.Result = new ViewResult();
        }
    }

    public class PublicAction
    {
        public string Controller { get; set; }
        public string Action { get; set; }
    }


}
