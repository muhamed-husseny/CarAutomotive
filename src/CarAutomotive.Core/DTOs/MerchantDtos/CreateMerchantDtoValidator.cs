using FluentValidation;

namespace CarAutomotive.Core.DTOs.MerchantDtos
{
    public class CreateMerchantDtoValidator : AbstractValidator<CreateMerchantDto>
    {
        public CreateMerchantDtoValidator()
        {
            RuleFor(x => x.ShopName)
                .NotEmpty().WithMessage("Shop name is required")
                .MaximumLength(100).WithMessage("Shop name must not exceed 100 characters");

            RuleFor(x => x.CommercialRegister)
                .NotEmpty().WithMessage("Commercial register is required")
                .MaximumLength(50).WithMessage("Commercial register is invalid");
        }
    }
}