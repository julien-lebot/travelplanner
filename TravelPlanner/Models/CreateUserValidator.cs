using FluentValidation;

namespace TravelPlanner.Models
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Password).NotNull().Length(6, 64);
            RuleFor(x => x.UserName).NotNull().Length(6, 12);
        }
    }
}