namespace AspNetEmptyApp.Feature.Inventory.Validation;

using AspNetEmptyApp.Feature.Inventory.DTOs;
using FluentValidation;

public class CreateProductionValidator : AbstractValidator<CreateProductionDto>
{
    public CreateProductionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .Length(2, 200).WithMessage("Product name must be between 2 and 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required and must not be empty.");
    }
}
