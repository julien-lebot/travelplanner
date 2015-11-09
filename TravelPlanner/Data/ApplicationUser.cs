using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TravelPlanner.Data
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Trip> Trips
        {
            get;
            set;
        }
    }
}