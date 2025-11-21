using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Security
{
    public class DBAuthSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserName = new TableField("UserName", SqlDbType.NVarChar);
        public TableField Password = new TableField("Password", SqlDbType.NVarChar);
        public TableField Token = new TableField("Token", SqlDbType.UniqueIdentifier);
        public TableField ExpiresAt = new TableField("ExpiresAt", SqlDbType.DateTime);
        public TableField ResetPassword = new TableField("ResetPassword", SqlDbType.NVarChar);
        public TableField KeepSignedIn = new TableField("KeepSignedIn", SqlDbType.Bit);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);

        public DBAuthSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "AuthSP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
            string Operation = "",
            string UserName = "",
            string Password = "",
            string Token = "",
            string ExpiresAt = "",
            string ResetPassword = "",
            string KeepSignedIn = "",
            string UserID = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserName.SetValue(UserName, ref FieldsArrayList);
            this.Password.SetValue(Password, ref FieldsArrayList);
            this.Token.SetValue(Token, ref FieldsArrayList);
            this.ExpiresAt.SetValue(ExpiresAt, ref FieldsArrayList);
            this.ResetPassword.SetValue(ResetPassword, ref FieldsArrayList);
            this.KeepSignedIn.SetValue(KeepSignedIn, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
