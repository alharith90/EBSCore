using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass.Security
{
    public class DBSecuritySP : DBParentStoredProcedureClass
    {
        private readonly IConfiguration _configuration;
        private readonly TableField operation = new TableField("Operation", SqlDbType.NVarChar);
        private readonly TableField currentUserID = new TableField("CurrentUserID", SqlDbType.Int);
        private readonly TableField roleID = new TableField("RoleID", SqlDbType.Int);
        private readonly TableField groupID = new TableField("GroupID", SqlDbType.Int);
        private readonly TableField userID = new TableField("UserID", SqlDbType.Int);
        private readonly TableField menuItemID = new TableField("MenuItemID", SqlDbType.Int);
        private readonly TableField actionID = new TableField("ActionID", SqlDbType.Int);
        private readonly TableField isAllowed = new TableField("IsAllowed", SqlDbType.Bit);
        private readonly TableField statusID = new TableField("StatusID", SqlDbType.Int);
        private readonly TableField isDeleted = new TableField("IsDeleted", SqlDbType.Bit);
        private readonly TableField roleName = new TableField("RoleName", SqlDbType.NVarChar);
        private readonly TableField roleCode = new TableField("RoleCode", SqlDbType.NVarChar);
        private readonly TableField roleDescription = new TableField("RoleDescription", SqlDbType.NVarChar);
        private readonly TableField groupName = new TableField("GroupName", SqlDbType.NVarChar);
        private readonly TableField groupCode = new TableField("GroupCode", SqlDbType.NVarChar);
        private readonly TableField groupDescription = new TableField("GroupDescription", SqlDbType.NVarChar);
        private readonly TableField menuCode = new TableField("MenuCode", SqlDbType.NVarChar);
        private readonly TableField menuName = new TableField("MenuName", SqlDbType.NVarChar);
        private readonly TableField menuNameKey = new TableField("MenuNameKey", SqlDbType.NVarChar);
        private readonly TableField url = new TableField("Url", SqlDbType.NVarChar);
        private readonly TableField apiRoute = new TableField("ApiRoute", SqlDbType.NVarChar);
        private readonly TableField iconCssClass = new TableField("IconCssClass", SqlDbType.NVarChar);
        private readonly TableField displayOrder = new TableField("DisplayOrder", SqlDbType.Int);
        private readonly TableField isVisible = new TableField("IsVisible", SqlDbType.Bit);
        private readonly TableField parentMenuItemID = new TableField("ParentMenuItemID", SqlDbType.Int);
        private readonly TableField actionCode = new TableField("ActionCode", SqlDbType.NVarChar);
        private readonly TableField actionName = new TableField("ActionName", SqlDbType.NVarChar);
        private readonly TableField actionNameKey = new TableField("ActionNameKey", SqlDbType.NVarChar);
        private readonly TableField actionDescription = new TableField("ActionDescription", SqlDbType.NVarChar);

        public DBSecuritySP(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            SPName = "sec.SecuritySP";
        }

        private void ResetFields()
        {
            FieldsArrayList = new ArrayList();
        }

        private void SetCommon(string operationName, int currentUser)
        {
            operation.SetValue(operationName, ref FieldsArrayList);
            currentUserID.SetValue(currentUser.ToString(), ref FieldsArrayList);
        }

        public DataTable RtvRoleList(int currentUser)
        {
            try
            {
                ResetFields();
                SetCommon("rtvRoleList", currentUser);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvRoleList currentUser:{currentUser}");
                return new DataTable();
            }
        }

        public DataTable RtvGroupList(int currentUser)
        {
            try
            {
                ResetFields();
                SetCommon("rtvGroupList", currentUser);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvGroupList currentUser:{currentUser}");
                return new DataTable();
            }
        }

        public DataTable RtvMenuItemList(int currentUser)
        {
            try
            {
                ResetFields();
                SetCommon("rtvMenuItemList", currentUser);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvMenuItemList currentUser:{currentUser}");
                return new DataTable();
            }
        }

        public DataTable RtvActionList(int currentUser)
        {
            try
            {
                ResetFields();
                SetCommon("rtvActionList", currentUser);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvActionList currentUser:{currentUser}");
                return new DataTable();
            }
        }

        public int SaveRole(int currentUser, int? roleId, string name, string code, string description, int status)
        {
            try
            {
                ResetFields();
                SetCommon("saveRole", currentUser);
                roleID.SetValue(roleId?.ToString() ?? string.Empty, ref FieldsArrayList);
                roleName.SetValue(name ?? string.Empty, ref FieldsArrayList);
                roleCode.SetValue(code ?? string.Empty, ref FieldsArrayList);
                roleDescription.SetValue(description ?? string.Empty, ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                return int.Parse(ds.Tables[0].Rows[0]["RoleID"].ToString());
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.SaveRole currentUser:{currentUser} roleId:{roleId} name:{name} code:{code}");
                return -1;
            }
        }

        public void DeleteRole(int currentUser, int roleId)
        {
            try
            {
                ResetFields();
                SetCommon("deleteRole", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.DeleteRole currentUser:{currentUser} roleId:{roleId}");
            }
        }

        public int SaveGroup(int currentUser, int? groupId, string name, string code, string description, int status)
        {
            try
            {
                ResetFields();
                SetCommon("saveGroup", currentUser);
                groupID.SetValue(groupId?.ToString() ?? string.Empty, ref FieldsArrayList);
                groupName.SetValue(name ?? string.Empty, ref FieldsArrayList);
                groupCode.SetValue(code ?? string.Empty, ref FieldsArrayList);
                groupDescription.SetValue(description ?? string.Empty, ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                return int.Parse(ds.Tables[0].Rows[0]["GroupID"].ToString());
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.SaveGroup currentUser:{currentUser} groupId:{groupId} name:{name} code:{code}");
                return -1;
            }
        }

        public void DeleteGroup(int currentUser, int groupId)
        {
            try
            {
                ResetFields();
                SetCommon("deleteGroup", currentUser);
                groupID.SetValue(groupId.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.DeleteGroup currentUser:{currentUser} groupId:{groupId}");
            }
        }

        public void AssignRoleToGroup(int currentUser, int roleId, int groupId, int status)
        {
            try
            {
                ResetFields();
                SetCommon("assignRoleToGroup", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                groupID.SetValue(groupId.ToString(), ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.AssignRoleToGroup currentUser:{currentUser} roleId:{roleId} groupId:{groupId} status:{status}");
            }
        }

        public void AssignRoleToUser(int currentUser, int roleId, int userId, int status)
        {
            try
            {
                ResetFields();
                SetCommon("assignRoleToUser", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                userID.SetValue(userId.ToString(), ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.AssignRoleToUser currentUser:{currentUser} roleId:{roleId} userId:{userId} status:{status}");
            }
        }

        public void AssignUserToGroup(int currentUser, int userId, int groupId, int status)
        {
            try
            {
                ResetFields();
                SetCommon("assignUserToGroup", currentUser);
                userID.SetValue(userId.ToString(), ref FieldsArrayList);
                groupID.SetValue(groupId.ToString(), ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.AssignUserToGroup currentUser:{currentUser} userId:{userId} groupId:{groupId} status:{status}");
            }
        }

        public DataTable RtvRolePermissions(int currentUser, int roleId)
        {
            try
            {
                ResetFields();
                SetCommon("rtvRolePermissions", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvRolePermissions currentUser:{currentUser} roleId:{roleId}");
                return new DataTable();
            }
        }

        public DataTable RtvMenuActionsForRole(int currentUser, int roleId)
        {
            try
            {
                ResetFields();
                SetCommon("rtvMenuActionsForRole", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvMenuActionsForRole currentUser:{currentUser} roleId:{roleId}");
                return new DataTable();
            }
        }

        public void SaveRolePermission(int currentUser, int roleId, int menuItemId, int actionId, bool allow)
        {
            try
            {
                ResetFields();
                SetCommon("saveRolePermission", currentUser);
                roleID.SetValue(roleId.ToString(), ref FieldsArrayList);
                menuItemID.SetValue(menuItemId.ToString(), ref FieldsArrayList);
                actionID.SetValue(actionId.ToString(), ref FieldsArrayList);
                isAllowed.SetValue(allow.ToString(), ref FieldsArrayList);
                QueryDatabase(SqlQueryType.ExecuteNonQuery);
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.SaveRolePermission currentUser:{currentUser} roleId:{roleId} menuItemId:{menuItemId} actionId:{actionId} allow:{allow}");
            }
        }

        public DataTable RtvMenuItemsForUser(int currentUser, int userId)
        {
            try
            {
                ResetFields();
                SetCommon("rtvMenuItemsForUser", currentUser);
                userID.SetValue(userId.ToString(), ref FieldsArrayList);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvMenuItemsForUser currentUser:{currentUser} userId:{userId}");
                return new DataTable();
            }
        }

        public DataTable RtvUserEffectivePermissions(int currentUser, int userId)
        {
            try
            {
                ResetFields();
                SetCommon("rtvUserEffectivePermissions", currentUser);
                userID.SetValue(userId.ToString(), ref FieldsArrayList);
                return ((DataSet)QueryDatabase(SqlQueryType.FillDataset)).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.RtvUserEffectivePermissions currentUser:{currentUser} userId:{userId}");
                return new DataTable();
            }
        }

        public int SaveMenuItem(int currentUser, int? menuId, int? parentId, string code, string name, string nameKey, string urlValue, string api, string icon, int order, bool isVisibleValue, int status)
        {
            try
            {
                ResetFields();
                SetCommon("saveMenuItem", currentUser);
                menuItemID.SetValue(menuId?.ToString() ?? string.Empty, ref FieldsArrayList);
                parentMenuItemID.SetValue(parentId?.ToString() ?? string.Empty, ref FieldsArrayList);
                menuCode.SetValue(code ?? string.Empty, ref FieldsArrayList);
                menuName.SetValue(name ?? string.Empty, ref FieldsArrayList);
                menuNameKey.SetValue(nameKey ?? string.Empty, ref FieldsArrayList);
                url.SetValue(urlValue ?? string.Empty, ref FieldsArrayList);
                apiRoute.SetValue(api ?? string.Empty, ref FieldsArrayList);
                iconCssClass.SetValue(icon ?? string.Empty, ref FieldsArrayList);
                displayOrder.SetValue(order.ToString(), ref FieldsArrayList);
                isVisible.SetValue(isVisibleValue.ToString(), ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                return int.Parse(ds.Tables[0].Rows[0]["MenuItemID"].ToString());
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.SaveMenuItem currentUser:{currentUser} menuId:{menuId} code:{code} name:{name}");
                return -1;
            }
        }

        public int SaveAction(int currentUser, int? actionId, string code, string name, string nameKey, string description, int status)
        {
            try
            {
                ResetFields();
                SetCommon("saveAction", currentUser);
                this.actionID.SetValue(actionId?.ToString() ?? string.Empty, ref FieldsArrayList);
                actionCode.SetValue(code ?? string.Empty, ref FieldsArrayList);
                actionName.SetValue(name ?? string.Empty, ref FieldsArrayList);
                actionNameKey.SetValue(nameKey ?? string.Empty, ref FieldsArrayList);
                actionDescription.SetValue(description ?? string.Empty, ref FieldsArrayList);
                statusID.SetValue(status.ToString(), ref FieldsArrayList);
                var ds = (DataSet)QueryDatabase(SqlQueryType.FillDataset);
                return int.Parse(ds.Tables[0].Rows[0]["ActionID"].ToString());
            }
            catch (Exception ex)
            {
                LogError(ex, $"DBSecuritySP.SaveAction currentUser:{currentUser} actionId:{actionId} code:{code} name:{name}");
                return -1;
            }
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(_configuration);
                handler.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveErrorHandler",
                    Message: ex.Message,
                    Form: context ?? string.Empty,
                    Source: ex.Source ?? string.Empty,
                    TargetSite: ex.TargetSite?.Name ?? string.Empty,
                    StackTrace: ex.StackTrace ?? string.Empty);
            }
            catch
            {
                // swallow logging exceptions
            }
        }

        private void LogInfo(string message, string context)
        {
            try
            {
                var handler = new EBSCore.AdoClass.Common.ErrorHandler(_configuration);
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
                // swallow logging exceptions
            }
        }
    }
}
