using EBSCore.AdoClass.Security;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text.Json;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CurrentUserController : BaseController
    {
        private readonly ILogger<CurrentUserController> logger;
        private readonly IConfiguration configuration;
        private readonly Common common = new Common();
        private readonly DBS7SUserAuthSP authSP;
        private readonly DBSecuritySP securitySP;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MinAlakherTools.MinAlakherEncryption encryption = new MinAlakherTools.MinAlakherEncryption();
        private const int DefaultLockMinutes = 15;
        private const int DefaultMaxAttempts = 5;

        public CurrentUserController(ILogger<CurrentUserController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            authSP = new DBS7SUserAuthSP(configuration);
            securitySP = new DBSecuritySP(configuration);
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                common.LogInfo("Login GET", $"Session:{HttpContext?.Session?.Id}");
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.Login GET Session:{HttpContext?.Session?.Id}");
                return BadRequest("Login unavailable");
            }
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid login request");
                }

                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var ip = common.getIPAddres(HttpContext);
                common.LogInfo("Login POST", $"User:{request.UserName} Session:{HttpContext?.Session?.Id} IP:{ip}");

                var loginTable = authSP.Login(request.UserName, request.Password, request.UserName, ip, userAgent, DefaultLockMinutes, DefaultMaxAttempts);
                if (loginTable.Rows.Count == 0)
                {
                    return Unauthorized(LocalizerString("InvalidCredentials"));
                }

                var loginRow = loginTable.Rows[0];
                var isAuthenticated = loginRow.Table.Columns.Contains("IsAuthenticated") && Convert.ToBoolean(loginRow["IsAuthenticated"]);
                var reason = loginRow.Table.Columns.Contains("Reason") ? loginRow["Reason"].ToString() : string.Empty;

                if (!isAuthenticated)
                {
                    if (string.Equals(reason, "AccountLocked", StringComparison.OrdinalIgnoreCase))
                    {
                        common.LogInfo("Login locked", $"User:{request.UserName} Reason:{reason}");
                        return Unauthorized(LocalizerString("AccountLocked"));
                    }

                    common.LogInfo("Login failed", $"User:{request.UserName} Reason:{reason}");
                    return Unauthorized(LocalizerString("InvalidCredentials"));
                }

                var user = new User
                {
                    UserID = loginRow["UserID"].ToString(),
                    Email = loginRow["Email"].ToString(),
                    UserFullName = loginRow["UserFullName"].ToString(),
                    CompanyID = loginRow["CompanyID"].ToString(),
                    CategoryID = loginRow["CategoryID"].ToString(),
                    UserType = (UserType)Convert.ToInt32(loginRow["UserType"]),
                    UserName = loginRow["UserName"].ToString(),
                    LastLoginAt = loginRow.Table.Columns.Contains("LastLoginAt") ? loginRow["LastLoginAt"].ToString() : null
                };

                HttpContext.Session.SetString("User", JsonSerializer.Serialize(user));

                var payload = JsonSerializer.Serialize(new { user.UserID, user.UserName, issued = DateTime.UtcNow });
                var encryptedTicket = encryption.Encrypt(payload, common.getEncryptionPassword());
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict
                };
                if (request.KeepMeSignedIn)
                {
                    options.Expires = DateTimeOffset.UtcNow.AddDays(30);
                }

                Response.Cookies.Append("AppAuth", encryptedTicket, options);

                authSP.UpdateLastLogin(Convert.ToInt32(user.UserID));

                var permissions = securitySP.RtvUserEffectivePermissions(Convert.ToInt32(user.UserID), Convert.ToInt32(user.UserID));
                HttpContext.Session.SetString("Permissions", JsonConvert.SerializeObject(permissions));

                common.LogInfo("Login succeeded", $"User:{request.UserName} UserID:{user.UserID}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.Login POST User:{request?.UserName}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Login failed");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                common.LogInfo("Logout", $"Session:{HttpContext?.Session?.Id}");
                HttpContext.Session.Clear();
                Response.Cookies.Delete("AppAuth");
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.Logout Session:{HttpContext?.Session?.Id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Logout failed");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            try
            {
                common.LogInfo("ResetPassword GET", $"Session:{HttpContext?.Session?.Id}");
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, "CurrentUserController.ResetPassword GET");
                return BadRequest("Reset password unavailable");
            }
        }

        [HttpPost]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid request");
                }

                common.LogInfo("ResetPassword POST", $"Identifier:{request.UserNameOrEmail}");
                var lookupTable = authSP.GetUserByUsername(request.UserNameOrEmail);
                if (lookupTable.Rows.Count == 0)
                {
                    lookupTable = authSP.GetUserByEmail(request.UserNameOrEmail);
                }

                if (lookupTable.Rows.Count == 0)
                {
                    return NotFound(LocalizerString("UserNotFound"));
                }

                var row = lookupTable.Rows[0];
                var userId = Convert.ToInt32(row["UserID"]);
                var tokenTable = authSP.CreateResetToken(userId, 60);
                var token = tokenTable.Rows.Count > 0 ? tokenTable.Rows[0]["Token"].ToString() : string.Empty;

                common.LogInfo("Reset token generated", $"UserID:{userId} Token:{token}");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.ResetPassword POST Identifier:{request?.UserNameOrEmail}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Reset password failed");
            }
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm([FromQuery] Guid token)
        {
            try
            {
                common.LogInfo("ResetPasswordConfirm GET", $"Token:{token}");
                var tokenTable = authSP.ValidateResetToken(token);
                if (tokenTable.Rows.Count == 0)
                {
                    return BadRequest(LocalizerString("InvalidOrExpiredToken"));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.ResetPasswordConfirm GET Token:{token}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Token validation failed");
            }
        }

        [HttpPost]
        public IActionResult ResetPasswordConfirm([FromBody] ResetPasswordConfirmRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid password request");
                }

                common.LogInfo("ResetPasswordConfirm POST", $"Token:{request.Token}");
                var tokenTable = authSP.ValidateResetToken(request.Token);
                if (tokenTable.Rows.Count == 0)
                {
                    return BadRequest(LocalizerString("InvalidOrExpiredToken"));
                }

                var tokenRow = tokenTable.Rows[0];
                var userId = Convert.ToInt32(tokenRow["UserID"]);
                var userName = tokenRow.Table.Columns.Contains("UserName") ? tokenRow["UserName"].ToString() : string.Empty;
                var email = tokenRow.Table.Columns.Contains("Email") ? tokenRow["Email"].ToString() : string.Empty;

                authSP.ResetPassword(userId, request.Token, request.NewPassword, string.IsNullOrWhiteSpace(userName) ? email : userName);
                authSP.UpdateLastLogin(userId);

                common.LogInfo("Password reset", $"UserID:{userId}");
                return Ok(LocalizerString("PasswordUpdatedSuccessfully"));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"CurrentUserController.ResetPasswordConfirm POST Token:{request?.Token}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Password reset failed");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            try
            {
                common.LogInfo("AccessDenied", $"Session:{HttpContext?.Session?.Id}");
                return Forbid();
            }
            catch (Exception ex)
            {
                common.LogError(ex, "CurrentUserController.AccessDenied");
                return StatusCode(StatusCodes.Status500InternalServerError, "Access denied");
            }
        }

        private string LocalizerString(string key)
        {
            try
            {
                var factory = HttpContext?.RequestServices.GetService(typeof(IStringLocalizerFactory)) as IStringLocalizerFactory;
                var localizer = factory?.Create("Common", string.Empty);
                return localizer?[key] ?? key;
            }
            catch
            {
                return key;
            }
        }
    }
}
