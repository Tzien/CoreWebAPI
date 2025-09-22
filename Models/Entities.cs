namespace CoreWebApi.Models
{
    public class SysUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string PasswordHash { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class SysRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? DisplayName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }

    public class SysMenu
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Path { get; set; } = default!;
        public string? Icon { get; set; }
        public Guid? ParentId { get; set; }
        public int Sort { get; set; } = 0;
        public bool IsHidden { get; set; } = false;
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }

    public class UserRole { public Guid UserId { get; set; } public Guid RoleId { get; set; } }
    public class RoleMenu { public Guid RoleId { get; set; } public Guid MenuId { get; set; } }
}