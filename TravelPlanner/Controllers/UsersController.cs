﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TravelPlanner.Data;
using TravelPlanner.Filters;

namespace TravelPlanner.Controllers
{
    public class AuthorizedRolesAttribute : AuthorizeAttribute
    {
        public AuthorizedRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }

    namespace Dto
    {
        public class User
        {
            public string UserName
            {
                get;
                set;
            }

            public List<string> Roles
            {
                get;
                set;
            }
        }

        public class CreateUser : User
        {
            public string Password
            {
                get;
                set;
            }
        }

        public class UserWithId : User
        {
            public string Id
            {
                get;
                set;
            }
        }
    }

    /// <summary>
    /// Controller to CRUD users.
    /// </summary>
    [Authorize]
    [IdentityBasicAuthentication]
    public class UsersController : ApiController
    {
        [EnableQuery]
        [AuthorizedRoles("UserManager, Admin")]
        public IQueryable<Dto.UserWithId> Get()
        {
            var ctx = new TravelPlannerDbContext();
            var userMgr = CreateUserManager(ctx);
            var roleMgr = CreateRoleManager(ctx);
            return userMgr.Users.Select(u => new Dto.UserWithId
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = (from role in roleMgr.Roles
                             join userRole in u.Roles on role.Id equals userRole.RoleId
                             select role.Name).ToList()
                });
        }

        [Route("api/users/current")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            var userId = User.Identity.GetUserId();
            var userMgr = CreateUserManager();
            var idUser = await userMgr.FindByIdAsync(userId);
            if (idUser == null)
            {
                return NotFound();
            }
            return Ok(new Dto.UserWithId
                {
                    Id = idUser.Id,
                    UserName = idUser.UserName,
                    Roles = userMgr.GetRoles(userId).ToList()
                });
        }

        public async Task<IHttpActionResult> Post(Dto.CreateUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ctx = new TravelPlannerDbContext();
            var userMgr = CreateUserManager(ctx);
            var roleMgr = CreateRoleManager(ctx);
            var nonExistentRoles = user.Roles.Where(role => !roleMgr.RoleExists(role));
            if (nonExistentRoles.Any())
            {
                return BadRequest(string.Format("Roles {0} do not exist", string.Join(", ", nonExistentRoles)));
            }
            var idUser = new IdentityUser { UserName = user.UserName };
            var result = await userMgr.CreateAsync(idUser, user.Password);
            if (result.Succeeded)
            {
                result = await userMgr.AddToRolesAsync(idUser.Id, user.Roles.ToArray());
                if (result.Succeeded)
                {
                    return Created(new Uri(string.Format("/api/users/{0}", idUser.Id), UriKind.Relative), new Dto.UserWithId
                    {
                        Id = idUser.Id,
                        UserName = idUser.UserName,
                        Roles = userMgr.GetRoles(idUser.Id).ToList()
                    });
                }
            }
            return BadRequest(string.Join(", ", result.Errors));
        }

        public async Task<IHttpActionResult> Delete(string id)
        {

        }

        private static UserManager<IdentityUser> CreateUserManager()
        {
            return CreateUserManager(new TravelPlannerDbContext());
        }

        private static UserManager<IdentityUser> CreateUserManager(TravelPlannerDbContext context)
        {
            return new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
        }

        private static RoleManager<IdentityRole> CreateRoleManager()
        {
            return CreateRoleManager(new TravelPlannerDbContext());
        }

        private static RoleManager<IdentityRole> CreateRoleManager(TravelPlannerDbContext context)
        {
            return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }
    }
}