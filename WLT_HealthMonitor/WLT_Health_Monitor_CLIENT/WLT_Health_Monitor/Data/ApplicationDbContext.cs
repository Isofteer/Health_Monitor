using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WLT_Health_Monitor.Models;

namespace WLT_Health_Monitor.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

      
        public DbSet<WLT_HealthMonitor.Models.wlt_HealthNotificationHistory> wlt_HealthNotificationHistory { get; set; }
        public DbSet<WLT_HealthMonitor.Models.wlt_ServerLogs> wlt_ServerLogs { get; set; }
        public DbSet<WLT_HealthMonitor.Models.wlt_ThreshholdSettings> wlt_ThreshholdSettings { get; set; }

        public DbSet<WLT_HealthMonitor.Models.wlt_Connections> wlt_Connections { get; set; }

        public DbSet<WLT_HealthMonitor.Models.wlt_Configurations> wlt_Configurations { get; set; }


        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("wlt_Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
