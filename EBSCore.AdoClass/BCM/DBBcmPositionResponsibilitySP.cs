using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBBcmPositionResponsibilitySP : DBParentStoredProcedureClass
    {
        public TableField Operation = new("Operation", SqlDbType.NVarChar);
        public TableField UserID = new("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new("CompanyID", SqlDbType.Int);
        public TableField UnitID = new("UnitID", SqlDbType.BigInt);
        public TableField PositionID = new("PositionID", SqlDbType.Int);
        public TableField PositionTitle = new("PositionTitle", SqlDbType.NVarChar);
        public TableField PositionCode = new("PositionCode", SqlDbType.NVarChar);
        public TableField Responsibilities = new("Responsibilities", SqlDbType.NVarChar);
        public TableField AuthorityLevel = new("AuthorityLevel", SqlDbType.NVarChar);
        public TableField ContactDetails = new("ContactDetails", SqlDbType.NVarChar);
        public TableField Status = new("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new("UpdatedAt", SqlDbType.DateTime);

        public DBBcmPositionResponsibilitySP(IConfiguration configuration) : base(configuration)
        {
            SPName = "BCMPositionResponsibilitySP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string PositionID = "",
            string PositionTitle = "",
            string PositionCode = "",
            string Responsibilities = "",
            string AuthorityLevel = "",
            string ContactDetails = "",
            string Status = "",
            string CreatedBy = "",
            string ModifiedBy = "",
            string CreatedAt = "",
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.PositionID.SetValue(PositionID, ref FieldsArrayList);
            this.PositionTitle.SetValue(PositionTitle, ref FieldsArrayList);
            this.PositionCode.SetValue(PositionCode, ref FieldsArrayList);
            this.Responsibilities.SetValue(Responsibilities, ref FieldsArrayList);
            this.AuthorityLevel.SetValue(AuthorityLevel, ref FieldsArrayList);
            this.ContactDetails.SetValue(ContactDetails, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
