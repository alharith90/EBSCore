using EBSCore.AdoClass;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System;
using System.Configuration;
using EBSCore.AdoClass.Common;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using EBSCore.Web.Models;

namespace EBSCore.Web.AppCode
{

    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public  class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>()
        {
            if (_serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(_serviceProvider), "Service provider is not set.");
            }
            return _serviceProvider.GetRequiredService<T>();
        }
    }
    public class Common
    {
        IHttpContextAccessor httpContextAccessor = ServiceLocator.GetService<IHttpContextAccessor>();
        IConfiguration configuration = ServiceLocator.GetService<IConfiguration>();
        User currentUser;
        public Common()
        {
            currentUser = httpContextAccessor?.HttpContext?.Session?.GetObject<User>("User");
        }


        public bool IsApiController(ActionExecutingContext context)
        {
            var controllerType = context.Controller.GetType();
            return controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), true).Any();
        }
        //    DBGeneralFunctionsSP objGeneral = new DBGeneralFunctionsSP();
        // DBFileSP DBFile = new DBFileSP();

        // Check If Exists Users in Group For First Save 
        public bool IsEmptyDataSet(DataSet ds)
        {
            if ((ds == null))
            {
                return true;
            }
            if (ds.Tables.Count < 1)
            {
                return true;
            }
            if (ds.Tables[0].Rows.Count < 1)
            {
                return true;
            }
            return false;
        }
        public async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var bodyAsText = string.Empty;
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyAsText = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            var formattedRequest = $"HttpRequest Information:{Environment.NewLine}" +
                                   $"Schema:{request.Scheme} " +
                                   $"Host: {request.Host} " +
                                   $"Path: {request.Path} " +
                                   $"QueryString: {request.QueryString} " +
                                   $"Request Body: {bodyAsText}";

            return formattedRequest;
        }
        public async void LogError(Exception ex, HttpRequest Request)
        {
            try
            {
                ErrorHandler objErrorHandler = new ErrorHandler(configuration);
                objErrorHandler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    ExDate: "",
                    Message: ex.Message,
                    Form: Request != null ? await FormatRequest(Request) : "Request context unavailable",
                    Source: ex.Source,
                    TargetSite: ex.TargetSite?.Name,
                    StackTrace: ex.StackTrace,
                    UserName: currentUser?.UserName);
            }
            catch
            {
                // Explicitly ignore logging failures to avoid breaking the caller.
            }
        }

        public async void LogError(Exception ex, string context)
        {
            try
            {
                ErrorHandler objErrorHandler = new ErrorHandler(configuration);
                objErrorHandler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    ExDate: "",
                    Message: ex.Message,
                    Form: context,
                    Source: ex.Source,
                    TargetSite: ex.TargetSite?.Name,
                    StackTrace: ex.StackTrace,
                    UserName: currentUser?.UserName);
            }
            catch
            {
                // Explicitly ignore logging failures to avoid breaking the caller.
            }
        }

        public async void LogInfo(string message, string context)
        {
            try
            {
                ErrorHandler objErrorHandler = new ErrorHandler(configuration);
                objErrorHandler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    ExDate: "",
                    Message: message,
                    Form: context,
                    Source: "INFO",
                    TargetSite: "INFO",
                    StackTrace: string.Empty,
                    UserName: currentUser?.UserName);
            }
            catch
            {
                // Explicitly ignore logging failures to avoid breaking the caller.
            }
        }

        //public string SaveFile(string FileString)
        //{
        //    Models.User currentUser = (Models.User)System.Web.HttpContext.Current.Session["User"];

        //    byte[] FileData = null;
        //    string FileMeta = "";
        //    if (FileString != "")
        //    {
        //        FileData = Convert.FromBase64String(FileString.Split(',')[1]);
        //        FileMeta = FileString.Split(',')[0];
        //        return
        //         DBFile.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar, "SaveFile",
        //         currentUser.UserID, null, FileData, null, FileMeta).ToString();
        //    }
        //    else
        //    {
        //        return "";
        //    }

        //}
        //public string getCurrentHost()
        //{
        //    string strHost = "";
        //    if (HttpContext.Current.Request.Url.IsDefaultPort)
        //    {
        //        strHost = HttpContext.Current.Request.Url.Host;

        //    }
        //    else
        //    {
        //        strHost = HttpContext.Current.Request.Url.Host.ToString() + ":" + HttpContext.Current.Request.Url.Port.ToString();
        //    }

        //    string ApplicationPath = HttpContext.Current.Request.ApplicationPath;
        //    if (ApplicationPath != "/" && ApplicationPath != "")
        //    {
        //        strHost += ApplicationPath;
        //    }
        //    var http = "http://";
        //    if (HttpContext.Current.Request.Url.AbsoluteUri.ToString().ToLower().Contains("https"))
        //    {
        //        http = "https://";
        //    }
        //    string URL = http + strHost;
        //    return URL;

        //}
        public string getEncryptionPassword()
        {
            return "sH@r1nJ_a17";
        }
        public string getIPAddres(HttpContext CurrentContext)
        {
            try
            {
                return CurrentContext.Connection.RemoteIpAddress?.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string GetDefaultExtension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }

    }
}
