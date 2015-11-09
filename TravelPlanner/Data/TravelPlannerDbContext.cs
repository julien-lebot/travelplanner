using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TravelPlanner.Data
{
    public class TravelPlannerDbContext : IdentityDbContext<ApplicationUser>
    {
        static TravelPlannerDbContext()
        {
            Database.SetInitializer(new Initializer());
        }

        public IDbSet<Trip> Trips
        {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Trip>()
                .HasRequired(x => x.User)
                .WithMany(x => x.Trips)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(true);
        }

        private static void AddUser(TravelPlannerDbContext context, string userName, string password, string roleId)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                // IdentityManager requires an e-mail, even if we don't use it
                Email = userName + "@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.Roles.Add(new IdentityUserRole { RoleId = roleId, UserId = user.Id });

            user.PasswordHash = new PasswordHasher().HashPassword(password);
            context.Users.Add(user);
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw;
            }
        }

        public static void DoSeed(TravelPlannerDbContext context)
        {
            var userRole = context.Roles.Add(new IdentityRole("User"));
            var userManagerRole = context.Roles.Add(new IdentityRole("UserManager"));
            var adminRole = context.Roles.Add(new IdentityRole("Admin"));

            AddUser(context, "TestUser1", "test", userRole.Id);
            AddUser(context, "TestUserManager1", "test", userManagerRole.Id);
            AddUser(context, "TestAdmin1", "test", adminRole.Id);
        }

        private class Initializer : CreateDatabaseIfNotExists<TravelPlannerDbContext>
        {
            protected override void Seed(TravelPlannerDbContext context)
            {
                DoSeed(context);
                base.Seed(context);
            }
        }
    }


}