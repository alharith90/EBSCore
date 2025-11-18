using EBSCore.AdoClass;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;


namespace EBSCore.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class CurrentUserController : BaseController
    {
        private readonly ILogger<CurrentUserController> _logger;
        private readonly IConfiguration _configuration;
        AppCode.Common objCommon = new AppCode.Common();
        DBUserSP objUser;
        ErrorHandler objErrorHandler;
        User currentUser=null;
        MinAlakherTools.MinAlakherEncryption objEncryption = new MinAlakherTools.MinAlakherEncryption();


        public CurrentUserController(ILogger<CurrentUserController> logger, IConfiguration configuration)
        {
            objErrorHandler = new ErrorHandler(configuration);
            objUser = new DBUserSP(configuration);
            _logger = logger;
            if(HttpContext != null) { 
               currentUser = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("User"));
			}
		}

        [HttpPost]
        public object Login( User user)
        {
            return UserActions("Login", user);
        }

        public object UserActions(string Operation, User user) {
       
            try
            {
                var referURL = "";
                var UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                bool IsMobile = false;
                if (
                               UserAgent.Contains("Mobile") ||
                               UserAgent.Contains("Android") ||
                               UserAgent.Contains("iPhone") ||
                               UserAgent.Contains("iPad") ||
                               UserAgent.Contains("Opera Mini") ||
                               UserAgent.Contains("BlackBerry") ||
                               UserAgent.Contains("IEMobile")
                   )
                {
                    IsMobile = true;

                }
                if (HttpContext.Request.Headers["Referer"].ToString() != null)
                {
                    referURL = HttpContext.Request.Headers["Referer"].ToString();
                }
                if (Operation == "Login")
                {
                    // Encrypt Password


                    user.Password = objEncryption.Encrypt(user.Password, objCommon.getEncryptionPassword());
                    ///admin=EC4B7A5C7A05405DDA20D766D2BD97C8
                    DataSet dsUser = (DataSet)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                       "Login", null, null, user.Email, user.Password, null, null, null, null, null, null, null, null, null, null, null, null, referURL,
                        objCommon.getIPAddres(HttpContext), referURL, UserAgent, UserAgent,
                        UserAgent, IsMobile.ToString(), HttpContext.Session.Id);
                    if (!objCommon.IsEmptyDataSet(dsUser))
                    {

                        DataRow rowUser = dsUser.Tables[0].Rows[0];
                        user.UserID = rowUser["UserID"].ToString();
                        user.Email = rowUser["Email"].ToString();
                        user.UserFullName = rowUser["UserFullName"].ToString();
                        user.CompanyID = rowUser["CompanyID"].ToString();
                        user.CategoryID = rowUser["CategoryID"].ToString();
                        user.UserType = (UserType)rowUser["UserType"];
                        user.UserName = rowUser["UserName"].ToString();
                        user.UserImage = rowUser["UserImage"].ToString();
                        user.CompanyName = rowUser["CompanyName"].ToString();
                        HttpContext.Session.SetString("User", JsonSerializer.Serialize(user));
                    }
                    else
                    {
                        throw new Exception("Wrong Email Or Password !");
                    }
                }
                //else if (Operation == "Register")
                //{
                //    // Encrypt Password
                //    user.Password = objEncryption.Encrypt(user.Password, objCommon.getEncryptionPassword());
                //    DataSet dsUser = (DataSet)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                //       "Register", null, user.Email, user.Password, user.UserFullName);
                //    if (!objCommon.IsEmptyDataSet(dsUser))
                //    {

                //        DataRow rowUser = dsUser.Tables[0].Rows[0];
                //        user.UserID = rowUser["UserID"].ToString();
                //        user.Email = rowUser["Email"].ToString();
                //        user.UserFullName = rowUser["UserFullName"].ToString();
                //        System.Web.HttpContext.Current.Session.Add("User", user);
                //    }
                //    else
                //    {
                //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong User"));

                //    }
                //}
                else if (Operation == "Update")
                {

                    byte[] BytesUseImage = null;
                    string meteUserImage = "";
                    if (user.UserImage != "")
                    {
                        BytesUseImage = Convert.FromBase64String(user.UserImage.Split(',')[1]);
                        meteUserImage = user.UserImage.Split(',')[0];
                    }
                    // Encrypt Password

                    user.Password = objEncryption.Encrypt(user.Password, objCommon.getEncryptionPassword());
                    DataSet dsUser = (DataSet)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                       "UpdateUser", currentUser.UserID, currentUser.UserID, user.Email, user.Password, user.UserFullName, null, null, null,
                        user.UserName, BytesUseImage, meteUserImage, user.Mobile, null, null, null, null, HttpContext.Request.GetDisplayUrl(),
                        objCommon.getIPAddres(HttpContext), referURL, UserAgent, UserAgent,
                        UserAgent, IsMobile.ToString(), HttpContext.Session.Id);
                    if (!objCommon.IsEmptyDataSet(dsUser))
                    {

                        DataRow rowUser = dsUser.Tables[0].Rows[0];
                        user.UserID = rowUser["UserID"].ToString();
                        user.Email = rowUser["Email"].ToString();
                        user.UserFullName = rowUser["UserFullName"].ToString();
                        user.UserName = rowUser["UserName"].ToString();
                        user.UserImage = rowUser["UserImage"].ToString();
                        HttpContext.Session.SetString("User", JsonSerializer.Serialize(user));
                    }
                }
                else if (Operation == "ForgetPassword")
                {
                    // Encrypt Password
                    string ResetPasswordKey = objEncryption.Encrypt(user.Email + "|" + DateTime.UtcNow.ToString(), objCommon.getEncryptionPassword());

                    bool IsExist = (bool)objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                       "IsExistsEmail", null, null, user.Email, user.Password, user.UserFullName, user.UserName, null, null, null,
                       null, null, null, null, null, null, null, referURL,
                        objCommon.getIPAddres(HttpContext), referURL, UserAgent, UserAgent,
                        UserAgent, IsMobile.ToString(), HttpContext.Session.Id, ResetPasswordKey);
                    if (IsExist)
                    {

                        // Send Email with Reset Password Link 


                        //// Get Reset Password Email Template 
                        //DBEmailTemplateSP objEmailTemplate = new DBEmailTemplateSP();
                        //string EmailTemplate = objEmailTemplate.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                        //"rtvByTemplateID", null, "1").ToString();

                        //string EmailBody = String.Format(EmailTemplate, ResetPasswordKey);

                        //App_Code.Email objEmail = new App_Code.Email();
                        //objEmail.Send(user.Email.Split(','), "Reset Password", EmailBody);
                    }
                    else
                    {
                        throw new Exception("Email Not Exist");

                    }

                }
                else if (Operation == "ResetPassword")
                {
                    // Encrypt Password

                    user.Password = objEncryption.Encrypt(user.Password, objCommon.getEncryptionPassword());
                    user.Email = objEncryption.Decrypt(user.ResetPasswordKey, objCommon.getEncryptionPassword()).Split('|')[0];


                    objUser.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                       "ResetPassword", null, null, user.Email, user.Password, user.UserFullName, user.UserName, null,
                        null, null, null, null, null, null, null, null, null, HttpContext.Request.GetDisplayUrl(),
                         objCommon.getIPAddres(HttpContext), referURL, UserAgent, UserAgent,
                           UserAgent, IsMobile.ToString(), HttpContext.Session.Id);

                }
                return user.UserType;
            }
            catch (System.Exception ex)
            {
                // Save Error before throw it


                objErrorHandler.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery, "SaveErrorHandler",
                    null, null, ex.Message, ex.Source,
                    "URL : " + HttpContext.Request.GetDisplayUrl() + " /n Full Request : " + Request.ToString(),
                    ex.TargetSite.ToString(),
                    ex.StackTrace, "FromCodeBehind:UserID:RegScreen");
                return BadRequest(new { message = "Call System Administrator" });
                //  throw new HttpResponseException(HttpContext.CreateErrorResponse(HttpStatusCode.BadRequest, "Call System Administrator"));
            }

        }

    }
}