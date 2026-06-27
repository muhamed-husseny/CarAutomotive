namespace CarAutomotive.Core.DTOs.MechanicDtos
{
    public class CreateMechanicProfileDtoValidator : AbstractValidator<CreateMechanicProfileDto>
    {
        public CreateMechanicProfileDtoValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The mechanic's name is required")
                .MaximumLength(100).WithMessage("The name must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("The phone number is invalid");

            
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

   
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
        }
    }
}
