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
}