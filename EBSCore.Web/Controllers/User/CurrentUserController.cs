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
        private readonly DBAuthSP authSP;
        private readonly DBSecuritySP securitySP;
        private readonly Common common;
        private readonly MinAlakherTools.MinAlakherEncryption encryption;

        public CurrentUserController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
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
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                DataRow rowUser = dsUser.Tables[0].Rows[0];
                var user = MapUser(rowUser);
                HttpContext.Session.SetObject("User", user);
                SavePermissions(user.UserID);
                WriteAuthCookie(user, request.KeepSignedIn);

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

                if (common.IsEmptyDataSet(dsToken))
                {
                    return BadRequest(new { message = "Reset token could not be created" });
                }

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

                return Ok();
            }
            catch (Exception ex)
            {
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
