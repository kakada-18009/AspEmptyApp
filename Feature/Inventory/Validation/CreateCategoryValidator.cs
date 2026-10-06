namespace AspNetEmptyApp.Feature.Inventory.Validation;

using AspNetEmptyApp.Feature.Inventory.DTOs;
using FluentValidation;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .Length(2, 100).WithMessage("Category name must be between 2 and 100 characters.");
    }
}
