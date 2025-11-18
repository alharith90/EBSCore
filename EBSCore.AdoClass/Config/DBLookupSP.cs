using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBLookupSP : DBParentStoredProcedureClass
    {
        // Define fields matching stored procedure parameters
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);
        public TableField LookupID = new TableField("LookupID", SqlDbType.Int);
        public TableField LookupType = new TableField("LookupType", SqlDbType.NVarChar);
        public TableField LookupDescriptionAr = new TableField("LookupDescriptionAr", SqlDbType.NVarChar);
        public TableField LookupDescriptionEn = new TableField("LookupDescriptionEn", SqlDbType.NVarChar);
        public TableField ParentID = new TableField("ParentID", SqlDbType.Int);
        public TableField Level = new TableField("Level", SqlDbType.Int);
        public TableField Status = new TableField("Status", SqlDbType.Bit);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        // Constructor to set the stored procedure name
        public DBLookupSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "LookupSP";
        }

        // Method to query the database
        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "",
            string UserID = "",
            string LookupID = "",
            string LookupType = "",
            string LookupDescriptionAr = "",
            string LookupDescriptionEn = "",
            string ParentID = "",
            string Level = "",
            string Status = "",
            string CreatedBy = "",
            string CreatedAt = "",
            string UpdatedBy = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();

            // Set parameter values
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.LookupID.SetValue(LookupID, ref FieldsArrayList);
            this.LookupType.SetValue(LookupType, ref FieldsArrayList);
            this.LookupDescriptionAr.SetValue(LookupDescriptionAr, ref FieldsArrayList);
            this.LookupDescriptionEn.SetValue(LookupDescriptionEn, ref FieldsArrayList);
            this.ParentID.SetValue(ParentID, ref FieldsArrayList);
            this.Level.SetValue(Level, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            // Execute the query
            return base.QueryDatabase(QueryType);
        }
    }
}
