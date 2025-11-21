using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EBSCore.AdoClass;
using EBSCore.AdoClass.Security;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CurrentUserController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DBAuthSP authSP;
        private readonly DBSecuritySP securitySP;
        private readonly Common common;
        private readonly MinAlakherTools.MinAlakherEncryption encryption;

        public CurrentUserController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            authSP = new DBAuthSP(configuration);
            securitySP = new DBSecuritySP(configuration);
            common = new Common();
            encryption = new MinAlakherTools.MinAlakherEncryption();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok();
        }

<<<<<<< ours
        public object UserActions(string Operation, User user) {

            try
            {
                objCommon.LogInfo("User action start", $"Operation:{Operation} User:{user?.Email} Session:{HttpContext?.Session?.Id}");
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
=======
        [HttpPost]
        public object Login([FromBody] LoginRequest request)
        {
            try
            {
                var hashedPassword = HashPassword(request.Password);
                var dsUser = (DataSet)authSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "Login",
                    UserName: request.UserName,
                    Password: hashedPassword,
                    KeepSignedIn: request.KeepSignedIn.ToString());

                if (common.IsEmptyDataSet(dsUser))
>>>>>>> theirs
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }
<<<<<<< ours
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
                        objCommon.LogInfo("Login succeeded", $"User:{user.Email} UserID:{user.UserID} Attempts:reset");
                    }
                    else
                    {
                        objCommon.LogInfo("Login failed", $"User:{user.Email} Reason:Wrong Email Or Password");
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
                        objCommon.LogInfo("User profile updated", $"User:{user.Email} UserID:{user.UserID}");
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
=======

                DataRow rowUser = dsUser.Tables[0].Rows[0];
                var user = MapUser(rowUser);
                HttpContext.Session.SetObject("User", user);
                SavePermissions(user.UserID);
                WriteAuthCookie(user, request.KeepSignedIn);
>>>>>>> theirs

                return Ok(user);
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public object Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("AppAuth");
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public object ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var expires = DateTime.UtcNow.AddHours(1);
                var dsToken = (DataSet)authSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "CreateResetToken",
                    UserName: request.UserNameOrEmail,
                    ExpiresAt: expires.ToString("o"));

<<<<<<< ours
                        //App_Code.Email objEmail = new App_Code.Email();
                        //objEmail.Send(user.Email.Split(','), "Reset Password", EmailBody);
                        objCommon.LogInfo("Reset password token created", $"User:{user.Email} Token:{ResetPasswordKey}");
                    }
                    else
                    {
                        objCommon.LogInfo("Forget password failed", $"User:{user.Email} Reason:Email Not Exist");
                        throw new Exception("Email Not Exist");
=======
                if (common.IsEmptyDataSet(dsToken))
                {
                    return BadRequest(new { message = "Reset token could not be created" });
                }
>>>>>>> theirs

                var token = dsToken.Tables[0].Rows[0]["Token"].ToString();
                var resetLink = Url.Action("ResetPasswordConfirm", "Account", new { token }, Request.Scheme);
                return Ok(new { token, resetLink });
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public object ResetPasswordConfirm([FromBody] ResetPasswordConfirmRequest request)
        {
            try
            {
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return BadRequest(new { message = "Passwords do not match" });
                }

                var validation = (DataSet)authSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "ValidateResetToken",
                    Token: request.Token);

                if (common.IsEmptyDataSet(validation))
                {
                    return BadRequest(new { message = "Invalid or expired token" });
                }

                var hashedPassword = HashPassword(request.NewPassword);
                authSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "ResetPassword",
                    Token: request.Token,
                    ResetPassword: hashedPassword);

<<<<<<< ours
                    objCommon.LogInfo("Password reset completed", $"User:{user.Email}");

                }
                return user.UserType;
=======
                return Ok();
>>>>>>> theirs
            }
            catch (Exception ex)
            {
<<<<<<< ours
                objCommon.LogError(ex, Request);
                return BadRequest(new { message = "Call System Administrator" });
                //  throw new HttpResponseException(HttpContext.CreateErrorResponse(HttpStatusCode.BadRequest, "Call System Administrator"));
=======
                common.LogError(ex, Request);
                return BadRequest(new { message = ex.Message });
            }
        }

        private void SavePermissions(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            DataTable permissions = securitySP.RtvUserEffectivePermissions(int.Parse(userId), int.Parse(userId));
            HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(permissions));
        }

        private void WriteAuthCookie(User user, bool keepSignedIn)
        {
            var authObject = new
            {
                UserID = user.UserID,
                UserName = user.UserName,
                FullName = user.UserFullName,
                CompanyID = user.CompanyID,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = keepSignedIn ? DateTime.UtcNow.AddDays(14) : (DateTime?)null
            };

            string json = JsonSerializer.Serialize(authObject);
            string encrypted = encryption.Encrypt(json, common.getEncryptionPassword());

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            };

            if (keepSignedIn)
            {
                options.Expires = DateTimeOffset.UtcNow.AddDays(14);
>>>>>>> theirs
            }

            Response.Cookies.Append("AppAuth", encrypted, options);
        }

        private string HashPassword(string password)
        {
            var salt = configuration["Security:PasswordSalt"] ?? "EBSCoreSalt";
            var data = Encoding.UTF8.GetBytes(password + salt);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }

        private User MapUser(DataRow rowUser)
        {
            return new User
            {
                UserID = rowUser["UserID"].ToString(),
                Email = rowUser["Email"].ToString(),
                UserFullName = rowUser["UserFullName"].ToString(),
                CompanyID = rowUser["CompanyID"].ToString(),
                CategoryID = rowUser["CategoryID"].ToString(),
                UserType = (UserType)Convert.ToInt32(rowUser["UserType"]),
                UserName = rowUser["UserName"].ToString(),
                UserImage = rowUser["UserImage"].ToString(),
                CompanyName = rowUser["CompanyName"].ToString(),
                LastLoginAt = rowUser.IsNull("LastLoginAt") ? null : Convert.ToDateTime(rowUser["LastLoginAt"]),
                FailedLoginAttempts = rowUser.IsNull("FailedLoginAttempts") ? 0 : Convert.ToInt32(rowUser["FailedLoginAttempts"]),
                LockUntil = rowUser.IsNull("LockUntil") ? null : Convert.ToDateTime(rowUser["LockUntil"]),
                StatusID = rowUser.IsNull("StatusID") ? null : Convert.ToInt32(rowUser["StatusID"]),
                IsDeleted = !rowUser.IsNull("IsDeleted") && Convert.ToBoolean(rowUser["IsDeleted"])
            };
        }
    }
}
