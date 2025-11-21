using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Security
{
    public class DBSecuritySP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        public TableField RoleID = new TableField("RoleID", SqlDbType.Int);
        public TableField GroupID = new TableField("GroupID", SqlDbType.Int);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);
        public TableField MenuItemID = new TableField("MenuItemID", SqlDbType.Int);
        public TableField ActionID = new TableField("ActionID", SqlDbType.Int);
        public TableField IsAllowed = new TableField("IsAllowed", SqlDbType.Bit);
        public TableField StatusID = new TableField("StatusID", SqlDbType.Int);
        public TableField IsDeleted = new TableField("IsDeleted", SqlDbType.Bit);
        public TableField RoleName = new TableField("RoleName", SqlDbType.NVarChar);
        public TableField RoleCode = new TableField("RoleCode", SqlDbType.NVarChar);
        public TableField RoleDescription = new TableField("RoleDescription", SqlDbType.NVarChar);
        public TableField GroupName = new TableField("GroupName", SqlDbType.NVarChar);
        public TableField GroupCode = new TableField("GroupCode", SqlDbType.NVarChar);
        public TableField GroupDescription = new TableField("GroupDescription", SqlDbType.NVarChar);
        public TableField MenuCode = new TableField("MenuCode", SqlDbType.NVarChar);
        public TableField MenuName = new TableField("MenuName", SqlDbType.NVarChar);
        public TableField MenuNameKey = new TableField("MenuNameKey", SqlDbType.NVarChar);
        public TableField Url = new TableField("Url", SqlDbType.NVarChar);
        public TableField ApiRoute = new TableField("ApiRoute", SqlDbType.NVarChar);
        public TableField IconCssClass = new TableField("IconCssClass", SqlDbType.NVarChar);
        public TableField DisplayOrder = new TableField("DisplayOrder", SqlDbType.Int);
        public TableField IsVisible = new TableField("IsVisible", SqlDbType.Bit);
        public TableField ParentMenuItemID = new TableField("ParentMenuItemID", SqlDbType.Int);
        public TableField ActionCode = new TableField("ActionCode", SqlDbType.NVarChar);
        public TableField ActionName = new TableField("ActionName", SqlDbType.NVarChar);
        public TableField ActionNameKey = new TableField("ActionNameKey", SqlDbType.NVarChar);
        public TableField ActionDescription = new TableField("ActionDescription", SqlDbType.NVarChar);

        public DBSecuritySP(IConfiguration configuration) : base(configuration)
        {
            SPName = "sec.SecuritySP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
            string Operation = "",
            string CurrentUserID = "",
            string RoleID = "",
            string GroupID = "",
            string UserID = "",
            string MenuItemID = "",
            string ActionID = "",
            string IsAllowed = "",
            string StatusID = "",
            string IsDeleted = "",
            string RoleName = "",
            string RoleCode = "",
            string RoleDescription = "",
            string GroupName = "",
            string GroupCode = "",
            string GroupDescription = "",
            string MenuCode = "",
            string MenuName = "",
            string MenuNameKey = "",
            string Url = "",
            string ApiRoute = "",
            string IconCssClass = "",
            string DisplayOrder = "",
            string IsVisible = "",
            string ParentMenuItemID = "",
            string ActionCode = "",
            string ActionName = "",
            string ActionNameKey = "",
            string ActionDescription = "")
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.RoleID.SetValue(RoleID, ref FieldsArrayList);
            this.GroupID.SetValue(GroupID, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.MenuItemID.SetValue(MenuItemID, ref FieldsArrayList);
            this.ActionID.SetValue(ActionID, ref FieldsArrayList);
            this.IsAllowed.SetValue(IsAllowed, ref FieldsArrayList);
            this.StatusID.SetValue(StatusID, ref FieldsArrayList);
            this.IsDeleted.SetValue(IsDeleted, ref FieldsArrayList);
            this.RoleName.SetValue(RoleName, ref FieldsArrayList);
            this.RoleCode.SetValue(RoleCode, ref FieldsArrayList);
            this.RoleDescription.SetValue(RoleDescription, ref FieldsArrayList);
            this.GroupName.SetValue(GroupName, ref FieldsArrayList);
            this.GroupCode.SetValue(GroupCode, ref FieldsArrayList);
            this.GroupDescription.SetValue(GroupDescription, ref FieldsArrayList);
            this.MenuCode.SetValue(MenuCode, ref FieldsArrayList);
            this.MenuName.SetValue(MenuName, ref FieldsArrayList);
            this.MenuNameKey.SetValue(MenuNameKey, ref FieldsArrayList);
            this.Url.SetValue(Url, ref FieldsArrayList);
            this.ApiRoute.SetValue(ApiRoute, ref FieldsArrayList);
            this.IconCssClass.SetValue(IconCssClass, ref FieldsArrayList);
            this.DisplayOrder.SetValue(DisplayOrder, ref FieldsArrayList);
            this.IsVisible.SetValue(IsVisible, ref FieldsArrayList);
            this.ParentMenuItemID.SetValue(ParentMenuItemID, ref FieldsArrayList);
            this.ActionCode.SetValue(ActionCode, ref FieldsArrayList);
            this.ActionName.SetValue(ActionName, ref FieldsArrayList);
            this.ActionNameKey.SetValue(ActionNameKey, ref FieldsArrayList);
            this.ActionDescription.SetValue(ActionDescription, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
