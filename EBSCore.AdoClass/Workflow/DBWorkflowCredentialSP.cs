using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBWorkflowCredentialSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField CredentialID = new TableField("CredentialID", SqlDbType.Int);
        public TableField CredentialName = new TableField("CredentialName", SqlDbType.NVarChar);
        public TableField CredentialType = new TableField("CredentialType", SqlDbType.NVarChar);
        public TableField DataJson = new TableField("DataJson", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);

        public DBWorkflowCredentialSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SCredentialSP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string CredentialID = "",
            string CredentialName = "",
            string CredentialType = "",
            string DataJson = "",
            string Notes = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.CredentialID.SetValue(CredentialID, ref FieldsArrayList);
            this.CredentialName.SetValue(CredentialName, ref FieldsArrayList);
            this.CredentialType.SetValue(CredentialType, ref FieldsArrayList);
            this.DataJson.SetValue(DataJson, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
