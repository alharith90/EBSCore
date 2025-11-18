using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using Newtonsoft.Json;
using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;

namespace EBSCore.Web.Services
{
    public class LoggingService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Common _objCommon;

        public LoggingService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _objCommon = new Common();
        }

        public async Task<bool> EnsureLoggedIn()
        {
            // Check for user session
            var currentUser = _httpContextAccessor.HttpContext.Session.GetObject<Models.User>("User");

            if (currentUser == null)
            {
                // Auto Login Settings
                bool isAutoLogin = bool.Parse(_configuration["EnableAutoLogin"]);
                if (isAutoLogin)
                {
                    var objUser = new DBUserSP(_configuration);
                    string autoLoginUser = _configuration["AutoLoginUser"];
                    string autoLoginPassword = _configuration["AutoLoginPassword"];
                    var dsUser = (DataSet)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                        "LoginWithNoHistory", null, null, autoLoginUser, autoLoginPassword);

                    if (!_objCommon.IsEmptyDataSet(dsUser))
                    {
                        var rowUser = dsUser.Tables[0].Rows[0];
                        var user = new Models.User
                        {
                            UserID = rowUser["UserID"].ToString(),
                            Email = rowUser["Email"].ToString(),
                            UserFullName = rowUser["UserFullName"].ToString(),
                            CompanyID = rowUser["CompanyID"].ToString(),
                            CategoryID = rowUser["CategoryID"].ToString(),
                            UserType = (UserType)rowUser["UserType"],
                            UserName = rowUser["UserName"].ToString(),
                            UserImage = rowUser["UserImage"].ToString(),
                            CompanyName = rowUser["CompanyName"].ToString()
                        };

                        _httpContextAccessor.HttpContext.Session.SetObject("User", user);
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public DataTable GetMenuItems(string userId)
        {
            var menuItemSP = new DBMenuItemsSP(_configuration);
            var dsResult = (DataSet)menuItemSP.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                "rtvMenuItems", userId);

            return dsResult.Tables[0];
        }
    }
}
