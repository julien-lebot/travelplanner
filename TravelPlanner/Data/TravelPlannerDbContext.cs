using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TravelPlanner.Data
{
    public class Trip
    {
        [Key]
        public string Id
        {
            get;
            set;
        }

        public Trip()
        {
            Id = Guid.NewGuid().ToString();
        }

        public DateTimeOffset StartDate
        {
            get;
            set;
        }

        public DateTimeOffset EndDate
        {
            get;
            set;
        }

        public string Destination
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }

        public virtual IdentityUser User
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public string UserId
        {
            get;
            set;
        }
    }

    public class TravelPlannerDbContext : IdentityDbContext<IdentityUser>
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

        private class Initializer : CreateDatabaseIfNotExists<TravelPlannerDbContext>
        {
            private void AddUser(TravelPlannerDbContext context, string userName, string password, string roleId)
            {
                IdentityUser user = new IdentityUser(userName)
                {
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                user.Roles.Add(new IdentityUserRole { RoleId = roleId, UserId = user.Id});
                user.Claims.Add(new IdentityUserClaim
                {
                    UserId = user.Id,
                    ClaimType = "hasRegistered",
                    ClaimValue = "true"
                });

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

            protected override void Seed(TravelPlannerDbContext context)
            {
                IdentityRole userRole = context.Roles.Add(new IdentityRole("User"));
                IdentityRole userManagerRole = context.Roles.Add(new IdentityRole("UserManager"));
                IdentityRole adminRole = context.Roles.Add(new IdentityRole("Admin"));

                AddUser(context, "TestUser1", "test", userRole.Id);
                AddUser(context, "TestUserManager1", "test", userManagerRole.Id);
                AddUser(context, "TestAdmin1", "test", adminRole.Id);

                base.Seed(context);
            }
        }
    }
}