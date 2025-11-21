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
                DataTable roles = securitySP.RtvRoleList(currentUser.UserID);
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
                DataTable menuItems = securitySP.RtvMenuItemList(currentUser.UserID);
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
                DataTable actions = securitySP.RtvActionList(currentUser.UserID);
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
                int roleId = securitySP.SaveRole(currentUser.UserID, role.RoleID, role.RoleName, role.RoleCode, role.Description, role.StatusID);
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
                securitySP.DeleteRole(currentUser.UserID, role.RoleID);
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
                DataTable groups = securitySP.RtvGroupList(currentUser.UserID);
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
                int groupId = securitySP.SaveGroup(currentUser.UserID, group.GroupID, group.GroupName, group.GroupCode, group.Description, group.StatusID);
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
                securitySP.DeleteGroup(currentUser.UserID, group.GroupID);
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
                securitySP.AssignRoleToGroup(currentUser.UserID, model.RoleID, model.GroupID, model.StatusID);
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
                securitySP.AssignRoleToUser(currentUser.UserID, model.RoleID, model.UserID, model.StatusID);
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
                securitySP.AssignUserToGroup(currentUser.UserID, model.UserID, model.GroupID, model.StatusID);
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
                DataTable permissions = securitySP.RtvMenuActionsForRole(currentUser.UserID, roleId);
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
                securitySP.SaveRolePermission(currentUser.UserID, roleId, menuItemId, actionId, isAllowed);
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
                DataTable permissions = securitySP.RtvUserEffectivePermissions(currentUser.UserID, userId);
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
