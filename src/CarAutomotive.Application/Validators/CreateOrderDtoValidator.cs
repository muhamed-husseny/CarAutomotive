using CarAutomotive.Core.DTOs;
using FluentValidation;

namespace CarAutomotive.Application.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.DeliveryType)
                .NotEmpty()
                .Must(x => x.Equals("DIRECT_DELIVERY", StringComparison.OrdinalIgnoreCase) || 
                           x.Equals("WORKSHOP_INSTALLATION", StringComparison.OrdinalIgnoreCase))
                .WithMessage("DeliveryType must be either 'DIRECT_DELIVERY' or 'WORKSHOP_INSTALLATION'.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId is required.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });

            When(x => x.DeliveryType.Equals("DIRECT_DELIVERY", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.ShippingAddress)
                    .NotNull()
                    .WithMessage("ShippingAddress is required for direct delivery.");

                RuleFor(x => x.ShippingAddress!.Street)
                    .NotEmpty()
                    .WithMessage("Street is required for direct delivery.");

                RuleFor(x => x.ShippingAddress!.City)
                    .NotEmpty()
                    .WithMessage("City is required for direct delivery.");

                RuleFor(x => x.ShippingAddress!.Governorate)
                    .NotEmpty()
                    .WithMessage("Governorate is required for direct delivery.");
            });

            When(x => x.DeliveryType.Equals("WORKSHOP_INSTALLATION", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.PartnerWorkshopId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("PartnerWorkshopId is required for workshop installation.");
            });
        }
    }
}