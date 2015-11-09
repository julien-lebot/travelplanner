using System.Collections.Generic;

namespace TravelPlanner.Models
{
    [FluentValidation.Attributes.Validator(typeof(CreateUserValidator))]
    public class CreateUser : User
    {
        public string Password
        {
            get;
            set;
        }
    }

    public class PatchUser
    {
        public string Password
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