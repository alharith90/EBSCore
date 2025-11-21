using System;

namespace EBSCore.Web.Models.Security
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserGroup
    {
        public int UserGroupID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RoleGroup
    {
        public int RoleGroupID { get; set; }
        public int RoleID { get; set; }
        public int GroupID { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RoleUser
    {
        public int RoleUserID { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public int? ParentMenuItemID { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public string MenuNameKey { get; set; }
        public string Url { get; set; }
        public string ApiRoute { get; set; }
        public string IconCssClass { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Action
    {
        public int ActionID { get; set; }
        public string ActionCode { get; set; }
        public string ActionName { get; set; }
        public string ActionNameKey { get; set; }
        public string Description { get; set; }
        public bool IsSystemDefault { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MenuItemAction
    {
        public int MenuItemActionID { get; set; }
        public int MenuItemID { get; set; }
        public int ActionID { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RolePermission
    {
        public int RolePermissionID { get; set; }
        public int RoleID { get; set; }
        public int MenuItemID { get; set; }
        public int ActionID { get; set; }
        public bool IsAllowed { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserPermissionOverride
    {
        public int UserPermissionOverrideID { get; set; }
        public int UserID { get; set; }
        public int MenuItemID { get; set; }
        public int ActionID { get; set; }
        public bool IsAllowed { get; set; }
        public int StatusID { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
