using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
using TravelPlanner.Filters.BasicAuthentication;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    /// <summary>
    /// Controller to CRUD users.
    /// </summary>
    [Authorize]
    [IdentityBasicAuthentication]
    public class UsersController : ApiController
    {
        private readonly UserManager<ApplicationUser> _usrMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public UsersController(UserManager<ApplicationUser> usrMgr, RoleManager<IdentityRole> roleMgr)
        {
            _usrMgr = usrMgr;
            _roleMgr = roleMgr;
        }

        [EnableQuery]
        [AuthorizedRoles(Roles.UserManager, Roles.Admin)]
        public IQueryable<UserWithId> Get()
        {
            var userId = User.Identity.GetUserId();
            // Return all users, except current
            return _usrMgr.Users.Where(u => u.Id != userId).Select(u => new UserWithId
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = (from role in _roleMgr.Roles
                             join userRole in u.Roles on role.Id equals userRole.RoleId
                             select role.Name).ToList()
                });
        }

        [Route("api/users/current")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            var userId = User.Identity.GetUserId();
            var idUser = await _usrMgr.FindByIdAsync(userId);
            if (idUser == null)
            {
                return NotFound();
            }
            return Ok(new UserWithId
                {
                    Id = idUser.Id,
                    UserName = idUser.UserName,
                    Roles = _usrMgr.GetRoles(userId).ToList()
                });
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(CreateUser user)
        {
            if (user.Roles != null)
            {
                var nonExistentRoles = user.Roles.Where(role => !_roleMgr.RoleExists(role)).ToList();
                if (nonExistentRoles.Any())
                {
#if DEBUG
                    return BadRequest(string.Format("Roles {0} do not exist", string.Join(", ", nonExistentRoles)));
#else
                // In production environment, don't reveal roles
                return BadRequest();
#endif
                }
            }

            // Force user role for new users
            // Also ensures there is always a role
            if (!User.Identity.IsAuthenticated)
            {
                user.Roles = null;
            }
            else if (user.Roles != null)
            {
                // UserManager and User can only create users
                var userId = User.Identity.GetUserId();
                if (await _usrMgr.IsInRoleAsync(userId, Roles.User) ||
                    await _usrMgr.IsInRoleAsync(userId, Roles.UserManager))
                {
                    user.Roles.RemoveAll(r => r != Roles.User);
                }
            }

            if (user.Roles == null || user.Roles.Count == 0)
            {
                user.Roles = new List<string> { Roles.User };
            }

            var idUser = new ApplicationUser { UserName = user.UserName, Email = user.UserName + "@travel.com" };
            var result = await _usrMgr.CreateAsync(idUser, user.Password);
            if (result.Succeeded)
            {
                result = await _usrMgr.AddToRolesAsync(idUser.Id, user.Roles.ToArray());
                if (result.Succeeded)
                {
                    return Created(new Uri(string.Format("/api/users/{0}", idUser.Id), UriKind.Relative), new UserWithId
                    {
                        Id = idUser.Id,
                        UserName = idUser.UserName,
                        Roles = _usrMgr.GetRoles(idUser.Id).ToList()
                    });
                }
            }
            foreach (var err in result.Errors)
            {
                if (err.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ModelState.AddModelError("user.Password", err);
                }
                else if (err.IndexOf("username", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ModelState.AddModelError("user.UserName", err);
                }
                else
                {
                    ModelState.AddModelError("user", err);
                }
            }
            return BadRequest(ModelState);
        }

        public async Task<IHttpActionResult> Patch(string id, PatchUser patchUser)
        {
            var userId = User.Identity.GetUserId();
            var roles = await _usrMgr.GetRolesAsync(userId);

            if (!(roles.Contains(Roles.UserManager) ||
                  roles.Contains(Roles.Admin)))
            {
                // User can only update themselves
                if (userId != id)
                {
                    ModelState.AddModelError("userId", "Permission denied");
                }

                // Users cannot change their roles
                if (patchUser.Roles != null)
                {
                    patchUser.Roles = new List<string>
                    {
                        "User"
                    };
                }
            }

            var targetRoles = await _usrMgr.GetRolesAsync(id);
            if (roles.Contains(Roles.UserManager))
            {
                // User manager cannot change admin
                // User manager cannot upgrade anybody to admin
                if (targetRoles.Contains(Roles.Admin) ||
                    patchUser.Roles != null && patchUser.Roles.Contains(Roles.Admin))
                {
                    ModelState.AddModelError("userId", "Permission denied");
                }
            }

            IdentityResult result = null;
            if (ModelState.IsValid)
            {
                var usr = await _usrMgr.FindByIdAsync(id);

                if (!string.IsNullOrEmpty(patchUser.Password))
                {
                    result = await _usrMgr.RemovePasswordAsync(id);
                    if (result.Succeeded)
                    {
                        result = await _usrMgr.AddPasswordAsync(id, patchUser.Password);
                    }
                }
                if (result == null || result.Succeeded)
                {
                    if (patchUser.Roles != null)
                    {
                        var currRoles = await _usrMgr.GetRolesAsync(id);
                        result = await _usrMgr.RemoveFromRolesAsync(id, currRoles.ToArray());
                        if (result.Succeeded)
                        {
                            result = await _usrMgr.AddToRolesAsync(id, patchUser.Roles.ToArray());
                        }
                    }
                }

                if (result == null || result.Succeeded)
                {
                    return Ok(
                        new UserWithId
                        {
                            Id = id,
                            UserName = usr.UserName,
                            Roles = _usrMgr.GetRoles(id).ToList()
                        });
                }
            }

            if (result != null)
            {
                foreach (var err in result.Errors)
                {
                    if (err.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ModelState.AddModelError("user.Password", err);
                    }
                    else if (err.IndexOf("username", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ModelState.AddModelError("user.UserName", err);
                    }
                    else
                    {
                        ModelState.AddModelError("user", err);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [AuthorizedRoles(Roles.UserManager, Roles.Admin)]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var user = await _usrMgr.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _usrMgr.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(string.Join(", ", result.Errors));
                }
            }
            else
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
