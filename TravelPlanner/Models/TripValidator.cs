using FluentValidation;

namespace TravelPlanner.Models
{
    public class TripValidator : AbstractValidator<Trip>
    {
        public TripValidator()
        {
            RuleFor(x => x.StartDate).NotNull().LessThan(x => x.EndDate);
            RuleFor(x => x.EndDate).NotNull()/*.GreaterThan(x => x.StartDate)*/;
            RuleFor(x => x.Destination).NotNull();
            RuleFor(x => x.Comment).NotNull();
        }
    }

    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Password).NotNull().Length(6, 64);
            RuleFor(x => x.UserName).NotNull().Length(6, 12);
        }
    }
}