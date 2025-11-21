using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Security
{
    public class DBS7SUserAuthSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);
        public TableField UserName = new TableField("UserName", SqlDbType.NVarChar);
        public TableField Email = new TableField("Email", SqlDbType.NVarChar);
        public TableField Password = new TableField("Password", SqlDbType.NVarChar);
        public TableField NewPassword = new TableField("NewPassword", SqlDbType.NVarChar);
        public TableField Token = new TableField("Token", SqlDbType.UniqueIdentifier);
        public TableField TokenExpiryMinutes = new TableField("TokenExpiryMinutes", SqlDbType.Int);
        public TableField IPAddress = new TableField("IPAddress", SqlDbType.NVarChar);
        public TableField UserAgent = new TableField("UserAgent", SqlDbType.NVarChar);
        public TableField LockDurationMinutes = new TableField("LockDurationMinutes", SqlDbType.Int);
        public TableField MaxFailedAttempts = new TableField("MaxFailedAttempts", SqlDbType.Int);

        public DBS7SUserAuthSP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SUserAuthSP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
            string Operation = "",
            string UserID = "",
            string UserName = "",
            string Email = "",
            string Password = "",
            string NewPassword = "",
            string Token = "",
            string TokenExpiryMinutes = "",
            string IPAddress = "",
            string UserAgent = "",
            string LockDurationMinutes = "",
            string MaxFailedAttempts = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.UserName.SetValue(UserName, ref FieldsArrayList);
            this.Email.SetValue(Email, ref FieldsArrayList);
            this.Password.SetValue(Password, ref FieldsArrayList);
            this.NewPassword.SetValue(NewPassword, ref FieldsArrayList);
            this.Token.SetValue(Token, ref FieldsArrayList);
            this.TokenExpiryMinutes.SetValue(TokenExpiryMinutes, ref FieldsArrayList);
            this.IPAddress.SetValue(IPAddress, ref FieldsArrayList);
            this.UserAgent.SetValue(UserAgent, ref FieldsArrayList);
            this.LockDurationMinutes.SetValue(LockDurationMinutes, ref FieldsArrayList);
            this.MaxFailedAttempts.SetValue(MaxFailedAttempts, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
