namespace CarAutomotive.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0 Can't Be Negative Value Or Zero.");

            RuleFor(p => p.StockCount)
                .GreaterThanOrEqualTo(0).WithMessage("Stock count cannot be negative.");

            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");

            RuleForEach(p => p.ImageUrls)
                .NotEmpty().WithMessage("Image URL cannot be empty.");
        }
    }
}
