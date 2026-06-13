namespace CarAutomotive.Application.Validators
{
    public class CreateOrderDtoValidator
        : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CartId)
                .NotEmpty();

            RuleFor(x => x.ShippingAddress)
                .NotNull();

            RuleFor(x => x.ShippingAddress.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ShippingAddress.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.ShippingAddress.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ShippingAddress.Street)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}