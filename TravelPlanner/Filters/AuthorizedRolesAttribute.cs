using System;
using System.Web.Http;

namespace TravelPlanner.Filters
{
    public class AuthorizedRolesAttribute : AuthorizeAttribute
    {
        public AuthorizedRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}