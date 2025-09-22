using Microsoft.EntityFrameworkCore;
using CoreWebApi.Models;

namespace CoreWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<SysUser> Users => Set<SysUser>();
        public DbSet<SysRole> Roles => Set<SysRole>();
        public DbSet<SysMenu> Menus => Set<SysMenu>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<RoleMenu>().HasKey(rm => new { rm.RoleId, rm.MenuId });
            modelBuilder.Entity<SysUser>().HasIndex(x => x.UserName).IsUnique();
            modelBuilder.Entity<SysRole>().HasIndex(x => x.Name).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }

    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            if (!db.Roles.Any())
            {
                db.Roles.Add(new SysRole { Id = Guid.NewGuid(), Name = "admin", DisplayName = "Administrator" });
                db.SaveChanges();
            }
            if (!db.Users.Any())
            {
                var admin = new SysUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    DisplayName = "Super Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                };
                db.Users.Add(admin);
                var adminRole = db.Roles.First(x => x.Name == "admin");
                db.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
                db.SaveChanges();
            }
            if (!db.Menus.Any())
            {
                var menus = new[]
                {
                    new SysMenu{ Id = Guid.NewGuid(), Name="Dashboard", Path="/dashboard", Icon="DashboardOutlined", Sort=1},
                    new SysMenu{ Id = Guid.NewGuid(), Name="Users", Path="/users", Icon="UserOutlined", Sort=2},
                    new SysMenu{ Id = Guid.NewGuid(), Name="Roles", Path="/roles", Icon="TeamOutlined", Sort=3},
                    new SysMenu{ Id = Guid.NewGuid(), Name="Menus", Path="/menus", Icon="AppstoreOutlined", Sort=4}
                };
                db.Menus.AddRange(menus);
                var adminRole = db.Roles.First(x => x.Name == "admin");
                foreach (var m in menus) db.RoleMenus.Add(new RoleMenu { RoleId = adminRole.Id, MenuId = m.Id });
                db.SaveChanges();
            }
        }
    }
}