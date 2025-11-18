using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace EBSCore.AdoClass
{
    public class DBMenuItemsSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField MenuItemID = new TableField("MenuItemID", SqlDbType.Int);
        public TableField ParentID = new TableField("ParentID", SqlDbType.Int);
        public TableField LabelAR = new TableField("LabelAR", SqlDbType.NVarChar);
        public TableField LabelEN = new TableField("LabelEN", SqlDbType.NVarChar);
        public TableField DescriptionAR = new TableField("DescriptionAR", SqlDbType.NVarChar);
        public TableField DescriptionEn = new TableField("DescriptionEn", SqlDbType.NVarChar);
        public TableField Url = new TableField("Url", SqlDbType.NVarChar);
        public TableField Icon = new TableField("Icon", SqlDbType.NVarChar);
        public TableField Order = new TableField("Order", SqlDbType.Int);
        public TableField IsActive = new TableField("IsActive", SqlDbType.Bit);
        public TableField Permission = new TableField("Permission", SqlDbType.NVarChar);
        public TableField Type = new TableField("Type", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBMenuItemsSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "MenuItemsSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "",string UserID = "", string MenuItemID = "", string ParentID = "", string LabelAR = "", string LabelEN = "",
            string DescriptionAR = "", string DescriptionEn = "", string Url = "", string Icon = "", string Order = "",
            string IsActive = "", string Permission = "", string Type = "", string CreatedBy = "", string UpdatedBy = "",
            string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.MenuItemID.SetValue(MenuItemID, ref FieldsArrayList);
            this.ParentID.SetValue(ParentID, ref FieldsArrayList);
            this.LabelAR.SetValue(LabelAR, ref FieldsArrayList);
            this.LabelEN.SetValue(LabelEN, ref FieldsArrayList);
            this.DescriptionAR.SetValue(DescriptionAR, ref FieldsArrayList);
            this.DescriptionEn.SetValue(DescriptionEn, ref FieldsArrayList);
            this.Url.SetValue(Url, ref FieldsArrayList);
            this.Icon.SetValue(Icon, ref FieldsArrayList);
            this.Order.SetValue(Order, ref FieldsArrayList);
            this.IsActive.SetValue(IsActive, ref FieldsArrayList);
            this.Permission.SetValue(Permission, ref FieldsArrayList);
            this.Type.SetValue(Type, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);


            return base.QueryDatabase(QueryType);
        }
    }
}
