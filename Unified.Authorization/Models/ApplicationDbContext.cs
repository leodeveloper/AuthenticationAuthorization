using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Unified.Authorization.Models
{
    // https://www.yogihosting.com/aspnet-core-identity-roles/#rolemanager
    // https://www.pluralsight.com/guides/configuring-asp-net-identity
    // https://www.c-sharpcorner.com/article/adding-role-authorization-to-a-asp-net-mvc-core-application/
    // https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/crud?view=aspnetcore-5.0

    // Scaffolding database: https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx
    // Using Dapper for CRUD: https://www.learnmvc.in/crud-operation-with-dotnetcore-dapper.php

    /// <summary>
    /// Just an ApplicationDbContext which inherits from 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // The (Role, User, UserClaims, RoleClaims) DbSets are all predefined in the IdentityDbContext
        // However, for any new table, we need to:
        // 1. Create a class 2. Add it to DbContext 3. Define the relationships in OnModelCreating.

        /*        
        public DbSet<HraApplications> HraApps { get; set; }
        public DbSet<Role_Application> Role_Applications { get; set; }
        */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // builder.Entity<Role_Application>().
            //     HasKey(ra => new { ra.ApplicationId, ra.RoleId });
           
            // Linking the Role with Role_Applications
            //builder.Entity<Role_Application>().HasOne(ra => ra.Role)
            // .WithMany(f => f.Role_Applications).HasForeignKey(ra => ra.RoleId);

            // Linking the HraApplication with Role_Applications
            // builder.Entity<Role_Application>().HasOne(ra => ra.Application)
            //   .WithMany(f => f.Role_Applications).HasForeignKey(ra => ra.ApplicationId);

            //builder.Entity<HraApplications>().ToTable("dbo.HraApplications");

        }


    }

}
