using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTime_D3.Hubs;

namespace RealTime_D3.Models
{
    public class RealtimeDbContext : IdentityDbContext
    {
        public DbSet<tbllog> Tbllogs { get; set; }
        public RealtimeDbContext(DbContextOptions<RealtimeDbContext> options)
   : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
             new IdentityRole
             {
                 Name = "Monitor",
                 NormalizedName = "MONITEUR",
                 Id = "b5a136a0-dc53-4e4e-b5e0-68d10b70fe02"
             },
             new IdentityRole
             {
                 Name = "Administrator",
                 NormalizedName = "ADMINISTRATOR",
                 Id = "d9e1208e-5301-4fc9-8db0-f2562714a991"
             }
         );

            var hasher = new PasswordHasher<ApiUser>();

            modelBuilder.Entity<ApiUser>().HasData(
                new ApiUser
                {
                    Id = "43c38655-9aa0-48b4-aab1-7cd175500f09",
                    Email = "yicheng.yang@ermo-tech.com",
                    NormalizedEmail = "YICHENG.YANG@ERMO-TECH.COM",
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    FirstName = "System",
                    LastName = "Admin",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")
                },
                new ApiUser
                {
                    Id = "5bda2409-9516-4983-90a3-08363427e744",
                    Email = "user@ermo-tech.com",
                    NormalizedEmail = "USER@ERMO-TECH.COM",
                    UserName = "user",
                    NormalizedUserName = "USER",
                    FirstName = "System",
                    LastName = "User",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "d9e1208e-5301-4fc9-8db0-f2562714a991",
                    UserId = "43c38655-9aa0-48b4-aab1-7cd175500f09"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "b5a136a0-dc53-4e4e-b5e0-68d10b70fe02",
                    UserId = "5bda2409-9516-4983-90a3-08363427e744"
                }
            );
        }
    }
}
