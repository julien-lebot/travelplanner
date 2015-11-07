using System.Collections.Generic;

namespace TravelPlanner.Models
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
}