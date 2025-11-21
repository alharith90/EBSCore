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
                DataTable roles = securitySP.RtvRoleList(currentUser.UserID);
                return Ok(JsonConvert.SerializeObject(roles));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving roles");
            }
        }

        [HttpGet]
        public object MenuItems()
        {
            try
            {
                DataTable menuItems = securitySP.RtvMenuItemList(currentUser.UserID);
                return Ok(JsonConvert.SerializeObject(menuItems));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving menu items");
            }
        }

        [HttpGet]
        public object Actions()
        {
            try
            {
                DataTable actions = securitySP.RtvActionList(currentUser.UserID);
                return Ok(JsonConvert.SerializeObject(actions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving actions");
            }
        }

        [HttpPost]
        public object SaveRole(Role role)
        {
            try
            {
                int roleId = securitySP.SaveRole(currentUser.UserID, role.RoleID, role.RoleName, role.RoleCode, role.Description, role.StatusID);
                return Ok(roleId);
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving role");
            }
        }

        [HttpPost]
        public object DeleteRole(Role role)
        {
            try
            {
                securitySP.DeleteRole(currentUser.UserID, role.RoleID);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error deleting role");
            }
        }

        [HttpGet]
        public object IndexGroups()
        {
            try
            {
                DataTable groups = securitySP.RtvGroupList(currentUser.UserID);
                return Ok(JsonConvert.SerializeObject(groups));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving groups");
            }
        }

        [HttpPost]
        public object SaveGroup(Group group)
        {
            try
            {
                int groupId = securitySP.SaveGroup(currentUser.UserID, group.GroupID, group.GroupName, group.GroupCode, group.Description, group.StatusID);
                return Ok(groupId);
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving group");
            }
        }

        [HttpPost]
        public object DeleteGroup(Group group)
        {
            try
            {
                securitySP.DeleteGroup(currentUser.UserID, group.GroupID);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error deleting group");
            }
        }

        [HttpPost]
        public object AssignRolesToGroup(RoleGroup model)
        {
            try
            {
                securitySP.AssignRoleToGroup(currentUser.UserID, model.RoleID, model.GroupID, model.StatusID);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error assigning role to group");
            }
        }

        [HttpPost]
        public object AssignRolesToUser(RoleUser model)
        {
            try
            {
                securitySP.AssignRoleToUser(currentUser.UserID, model.RoleID, model.UserID, model.StatusID);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error assigning role to user");
            }
        }

        [HttpPost]
        public object AssignUsersToGroup(UserGroup model)
        {
            try
            {
                securitySP.AssignUserToGroup(currentUser.UserID, model.UserID, model.GroupID, model.StatusID);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error assigning user to group");
            }
        }

        [HttpGet]
        public object RolePermissions(int roleId)
        {
            try
            {
                DataTable permissions = securitySP.RtvMenuActionsForRole(currentUser.UserID, roleId);
                return Ok(JsonConvert.SerializeObject(permissions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving role permissions");
            }
        }

        [HttpPost]
        public object SaveRolePermission(int roleId, int menuItemId, int actionId, bool isAllowed)
        {
            try
            {
                securitySP.SaveRolePermission(currentUser.UserID, roleId, menuItemId, actionId, isAllowed);
                return Ok();
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error saving role permission");
            }
        }

        [HttpGet]
        public object UserPermissions(int userId)
        {
            try
            {
                DataTable permissions = securitySP.RtvUserEffectivePermissions(currentUser.UserID, userId);
                return Ok(JsonConvert.SerializeObject(permissions));
            }
            catch (Exception ex)
            {
                common.LogError(ex, Request);
                return BadRequest("Error retrieving user permissions");
            }
        }
    }
}
