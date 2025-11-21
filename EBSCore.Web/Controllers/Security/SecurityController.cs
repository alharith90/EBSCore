using EBSCore.AdoClass.Security;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DBSecuritySP securitySP;
        private readonly Common common;
        private readonly User currentUser;

        public SecurityController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            securitySP = new DBSecuritySP(configuration);
            currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            common = new Common();
        }

        [HttpGet]
        public object IndexRoles()
        {
            try
            {
                common.LogInfo("IndexRoles requested", $"Controller:Security CurrentUser:{currentUser?.UserID}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvRoleList",
                    CurrentUserID: currentUser.UserID);
                DataTable roles = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(roles));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.IndexRoles CurrentUser:{currentUser?.UserID}");
                return BadRequest("Error retrieving roles");
            }
        }

        [HttpGet]
        public object MenuItems()
        {
            try
            {
                common.LogInfo("MenuItems requested", $"Controller:Security CurrentUser:{currentUser?.UserID}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvMenuItemList",
                    CurrentUserID: currentUser.UserID);
                DataTable menuItems = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(menuItems));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.MenuItems CurrentUser:{currentUser?.UserID}");
                return BadRequest("Error retrieving menu items");
            }
        }

        [HttpGet]
        public object Actions()
        {
            try
            {
                common.LogInfo("Actions requested", $"Controller:Security CurrentUser:{currentUser?.UserID}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvActionList",
                    CurrentUserID: currentUser.UserID);
                DataTable actions = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(actions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.Actions CurrentUser:{currentUser?.UserID}");
                return BadRequest("Error retrieving actions");
            }
        }

        [HttpPost]
        public object SaveRole(Role role)
        {
            try
            {
                common.LogInfo("SaveRole invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{role.RoleID} Code:{role.RoleCode}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "saveRole",
                    CurrentUserID: currentUser.UserID,
                    RoleID: role.RoleID.ToString(),
                    RoleName: role.RoleName,
                    RoleCode: role.RoleCode,
                    RoleDescription: role.Description,
                    StatusID: role.StatusID.ToString());

                int roleId = ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
                    ? Convert.ToInt32(ds.Tables[0].Rows[0]["RoleID"])
                    : 0;

                return Ok(roleId);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.SaveRole CurrentUser:{currentUser?.UserID} Role:{role?.RoleID} Code:{role?.RoleCode}");
                return BadRequest("Error saving role");
            }
        }

        [HttpPost]
        public object DeleteRole(Role role)
        {
            try
            {
                common.LogInfo("DeleteRole invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{role.RoleID}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "deleteRole",
                    CurrentUserID: currentUser.UserID,
                    RoleID: role.RoleID.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.DeleteRole CurrentUser:{currentUser?.UserID} Role:{role?.RoleID}");
                return BadRequest("Error deleting role");
            }
        }

        [HttpGet]
        public object IndexGroups()
        {
            try
            {
                common.LogInfo("IndexGroups requested", $"Controller:Security CurrentUser:{currentUser?.UserID}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvGroupList",
                    CurrentUserID: currentUser.UserID);
                DataTable groups = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(groups));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.IndexGroups CurrentUser:{currentUser?.UserID}");
                return BadRequest("Error retrieving groups");
            }
        }

        [HttpPost]
        public object SaveGroup(Group group)
        {
            try
            {
                common.LogInfo("SaveGroup invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Group:{group.GroupID} Code:{group.GroupCode}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "saveGroup",
                    CurrentUserID: currentUser.UserID,
                    GroupID: group.GroupID.ToString(),
                    GroupName: group.GroupName,
                    GroupCode: group.GroupCode,
                    GroupDescription: group.Description,
                    StatusID: group.StatusID.ToString());

                int groupId = ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
                    ? Convert.ToInt32(ds.Tables[0].Rows[0]["GroupID"])
                    : 0;

                return Ok(groupId);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.SaveGroup CurrentUser:{currentUser?.UserID} Group:{group?.GroupID} Code:{group?.GroupCode}");
                return BadRequest("Error saving group");
            }
        }

        [HttpPost]
        public object DeleteGroup(Group group)
        {
            try
            {
                common.LogInfo("DeleteGroup invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Group:{group.GroupID}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "deleteGroup",
                    CurrentUserID: currentUser.UserID,
                    GroupID: group.GroupID.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.DeleteGroup CurrentUser:{currentUser?.UserID} Group:{group?.GroupID}");
                return BadRequest("Error deleting group");
            }
        }

        [HttpPost]
        public object AssignRolesToGroup(RoleGroup model)
        {
            try
            {
                common.LogInfo("AssignRolesToGroup invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{model.RoleID} Group:{model.GroupID}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "assignRoleToGroup",
                    CurrentUserID: currentUser.UserID,
                    RoleID: model.RoleID.ToString(),
                    GroupID: model.GroupID.ToString(),
                    StatusID: model.StatusID.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.AssignRolesToGroup CurrentUser:{currentUser?.UserID} Role:{model?.RoleID} Group:{model?.GroupID}");
                return BadRequest("Error assigning role to group");
            }
        }

        [HttpPost]
        public object AssignRolesToUser(RoleUser model)
        {
            try
            {
                common.LogInfo("AssignRolesToUser invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{model.RoleID} User:{model.UserID}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "assignRoleToUser",
                    CurrentUserID: currentUser.UserID,
                    RoleID: model.RoleID.ToString(),
                    UserID: model.UserID.ToString(),
                    StatusID: model.StatusID.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.AssignRolesToUser CurrentUser:{currentUser?.UserID} Role:{model?.RoleID} User:{model?.UserID}");
                return BadRequest("Error assigning role to user");
            }
        }

        [HttpPost]
        public object AssignUsersToGroup(UserGroup model)
        {
            try
            {
                common.LogInfo("AssignUsersToGroup invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} User:{model.UserID} Group:{model.GroupID}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "assignUserToGroup",
                    CurrentUserID: currentUser.UserID,
                    UserID: model.UserID.ToString(),
                    GroupID: model.GroupID.ToString(),
                    StatusID: model.StatusID.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.AssignUsersToGroup CurrentUser:{currentUser?.UserID} User:{model?.UserID} Group:{model?.GroupID}");
                return BadRequest("Error assigning user to group");
            }
        }

        [HttpGet]
        public object RolePermissions(int roleId)
        {
            try
            {
                common.LogInfo("RolePermissions requested", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{roleId}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvMenuActionsForRole",
                    CurrentUserID: currentUser.UserID,
                    RoleID: roleId.ToString());
                DataTable permissions = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(permissions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.RolePermissions CurrentUser:{currentUser?.UserID} Role:{roleId}");
                return BadRequest("Error retrieving role permissions");
            }
        }

        [HttpPost]
        public object SaveRolePermission(int roleId, int menuItemId, int actionId, bool isAllowed)
        {
            try
            {
                common.LogInfo("SaveRolePermission invoked", $"Controller:Security CurrentUser:{currentUser?.UserID} Role:{roleId} Menu:{menuItemId} Action:{actionId} Allowed:{isAllowed}");
                securitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "saveRolePermission",
                    CurrentUserID: currentUser.UserID,
                    RoleID: roleId.ToString(),
                    MenuItemID: menuItemId.ToString(),
                    ActionID: actionId.ToString(),
                    IsAllowed: isAllowed.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.SaveRolePermission CurrentUser:{currentUser?.UserID} Role:{roleId} Menu:{menuItemId} Action:{actionId} Allowed:{isAllowed}");
                return BadRequest("Error saving role permission");
            }
        }

        [HttpGet]
        public object UserPermissions(int userId)
        {
            try
            {
                common.LogInfo("UserPermissions requested", $"Controller:Security CurrentUser:{currentUser?.UserID} User:{userId}");
                DataSet ds = (DataSet)securitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvUserEffectivePermissions",
                    CurrentUserID: currentUser.UserID,
                    UserID: userId.ToString());
                DataTable permissions = ds?.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
                return Ok(JsonConvert.SerializeObject(permissions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"SecurityController.UserPermissions CurrentUser:{currentUser?.UserID} User:{userId}");
                return BadRequest("Error retrieving user permissions");
            }
        }
    }
}
