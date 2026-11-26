namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class MechanicSearchDtoValidator : AbstractValidator<MechanicSearchDto>
    {
        public MechanicSearchDtoValidator()
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("The latitude is incorrect");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("The longitude is incorrect");

            RuleFor(x => x.RadiusInKilometers)
                .GreaterThan(0).WithMessage("The search range must be greater than zero")
                .LessThanOrEqualTo(50).WithMessage("Sorry, the maximum search range is 50 kilometers to maintain system speed");
        }

    }
}
