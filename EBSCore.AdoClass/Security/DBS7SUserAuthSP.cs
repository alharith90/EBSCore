using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Security
{
    public class DBS7SUserAuthSP : DBParentStoredProcedureClass
    {
        private readonly IConfiguration configuration;

        private readonly TableField operation = new TableField("Operation", SqlDbType.NVarChar);
        private readonly TableField userId = new TableField("UserID", SqlDbType.Int);
        private readonly TableField userName = new TableField("UserName", SqlDbType.NVarChar);
        private readonly TableField email = new TableField("Email", SqlDbType.NVarChar);
        private readonly TableField password = new TableField("Password", SqlDbType.NVarChar);
        private readonly TableField newPassword = new TableField("NewPassword", SqlDbType.NVarChar);
        private readonly TableField token = new TableField("Token", SqlDbType.UniqueIdentifier);
        private readonly TableField tokenExpiryMinutes = new TableField("TokenExpiryMinutes", SqlDbType.Int);
        private readonly TableField ipAddress = new TableField("IPAddress", SqlDbType.NVarChar);
        private readonly TableField userAgent = new TableField("UserAgent", SqlDbType.NVarChar);
        private readonly TableField lockDurationMinutes = new TableField("LockDurationMinutes", SqlDbType.Int);
        private readonly TableField maxFailedAttempts = new TableField("MaxFailedAttempts", SqlDbType.Int);

        public DBS7SUserAuthSP(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
            SPName = "S7SUserAuthSP";
        }

        private void Reset()
        {
            FieldsArrayList = new ArrayList();
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(configuration);
                handler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: ex.Message,
                    Form: context,
                    Source: ex.Source,
                    TargetSite: ex.TargetSite?.Name,
                    StackTrace: ex.StackTrace);
            }
            catch
            {
                // ignore logging exceptions
            }
        }

        private void LogInfo(string message, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(configuration);
                handler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: message,
                    Form: context,
                    Source: "INFO",
                    TargetSite: "INFO",
                    StackTrace: string.Empty);
            }
            catch
            {
                // ignore logging exceptions
            }
        }

        public DataTable Login(string userNameValue, string passwordValue, string emailValue, string ip, string agent, int? lockMinutes = null, int? maxAttempts = null)
        {
            try
            {
                Reset();
                operation.SetValue("Login", ref FieldsArrayList);
                userName.SetValue(userNameValue, ref FieldsArrayList);
                email.SetValue(emailValue, ref FieldsArrayList);
                password.SetValue(passwordValue, ref FieldsArrayList);
                ipAddress.SetValue(ip, ref FieldsArrayList);
                userAgent.SetValue(agent, ref FieldsArrayList);
                lockDurationMinutes.SetValue(lockMinutes?.ToString() ?? string.Empty, ref FieldsArrayList);
                maxFailedAttempts.SetValue(maxAttempts?.ToString() ?? string.Empty, ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("Login operation executed", $"User:{userNameValue ?? emailValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.Login user:{userNameValue ?? emailValue}");
                return new DataTable();
            }
        }

        public DataTable ValidatePassword(string userNameValue, string emailValue, string passwordValue)
        {
            try
            {
                Reset();
                operation.SetValue("ValidatePassword", ref FieldsArrayList);
                userName.SetValue(userNameValue, ref FieldsArrayList);
                email.SetValue(emailValue, ref FieldsArrayList);
                password.SetValue(passwordValue, ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("ValidatePassword executed", $"User:{userNameValue ?? emailValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.ValidatePassword user:{userNameValue ?? emailValue}");
                return new DataTable();
            }
        }

        public void RegisterLoginAttempt(int? userIdValue, string userNameValue, bool isSuccess, string failureReason, string ip, string agent)
        {
            try
            {
                Reset();
                operation.SetValue("RegisterLoginAttempt", ref FieldsArrayList);
                userId.SetValue(userIdValue?.ToString() ?? string.Empty, ref FieldsArrayList);
                userName.SetValue(userNameValue, ref FieldsArrayList);
                password.SetValue(isSuccess ? "" : "0", ref FieldsArrayList);
                newPassword.SetValue(failureReason, ref FieldsArrayList);
                ipAddress.SetValue(ip, ref FieldsArrayList);
                userAgent.SetValue(agent, ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
                LogInfo("RegisterLoginAttempt executed", $"User:{userNameValue} Success:{isSuccess}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.RegisterLoginAttempt user:{userNameValue}");
            }
        }

        public void LockAccount(int userIdValue, int? lockMinutes = null, int? maxAttempts = null)
        {
            try
            {
                Reset();
                operation.SetValue("LockAccount", ref FieldsArrayList);
                userId.SetValue(userIdValue.ToString(), ref FieldsArrayList);
                lockDurationMinutes.SetValue(lockMinutes?.ToString() ?? string.Empty, ref FieldsArrayList);
                maxFailedAttempts.SetValue(maxAttempts?.ToString() ?? string.Empty, ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
                LogInfo("LockAccount executed", $"UserID:{userIdValue}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.LockAccount userId:{userIdValue}");
            }
        }

        public void ResetFailedAttempts(int userIdValue)
        {
            try
            {
                Reset();
                operation.SetValue("ResetFailedAttempts", ref FieldsArrayList);
                userId.SetValue(userIdValue.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
                LogInfo("ResetFailedAttempts executed", $"UserID:{userIdValue}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.ResetFailedAttempts userId:{userIdValue}");
            }
        }

        public DataTable CreateResetToken(int userIdValue, int? expiryMinutes = null)
        {
            try
            {
                Reset();
                operation.SetValue("CreateResetToken", ref FieldsArrayList);
                userId.SetValue(userIdValue.ToString(), ref FieldsArrayList);
                tokenExpiryMinutes.SetValue(expiryMinutes?.ToString() ?? string.Empty, ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("CreateResetToken executed", $"UserID:{userIdValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.CreateResetToken userId:{userIdValue}");
                return new DataTable();
            }
        }

        public DataTable ValidateResetToken(Guid tokenValue)
        {
            try
            {
                Reset();
                operation.SetValue("ValidateResetToken", ref FieldsArrayList);
                token.SetValue(tokenValue.ToString(), ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("ValidateResetToken executed", $"Token:{tokenValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.ValidateResetToken token:{tokenValue}");
                return new DataTable();
            }
        }

        public void ResetPassword(int userIdValue, Guid tokenValue, string newPasswordValue, string userNameValue)
        {
            try
            {
                Reset();
                operation.SetValue("ResetPassword", ref FieldsArrayList);
                userId.SetValue(userIdValue.ToString(), ref FieldsArrayList);
                token.SetValue(tokenValue.ToString(), ref FieldsArrayList);
                newPassword.SetValue(newPasswordValue, ref FieldsArrayList);
                userName.SetValue(userNameValue, ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
                LogInfo("ResetPassword executed", $"UserID:{userIdValue}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.ResetPassword userId:{userIdValue}");
            }
        }

        public void UpdateLastLogin(int userIdValue)
        {
            try
            {
                Reset();
                operation.SetValue("UpdateLastLogin", ref FieldsArrayList);
                userId.SetValue(userIdValue.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
                LogInfo("UpdateLastLogin executed", $"UserID:{userIdValue}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.UpdateLastLogin userId:{userIdValue}");
            }
        }

        public DataTable GetUserByUsername(string userNameValue)
        {
            try
            {
                Reset();
                operation.SetValue("GetUserByUsername", ref FieldsArrayList);
                userName.SetValue(userNameValue, ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("GetUserByUsername executed", $"User:{userNameValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.GetUserByUsername user:{userNameValue}");
                return new DataTable();
            }
        }

        public DataTable GetUserByEmail(string emailValue)
        {
            try
            {
                Reset();
                operation.SetValue("GetUserByEmail", ref FieldsArrayList);
                email.SetValue(emailValue, ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                LogInfo("GetUserByEmail executed", $"Email:{emailValue}");
                return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBS7SUserAuthSP.GetUserByEmail email:{emailValue}");
                return new DataTable();
            }
        }
    }
}
