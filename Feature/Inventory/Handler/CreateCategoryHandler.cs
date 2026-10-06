namespace AspNetEmptyApp.Feature.Inventory.Handler;

using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Inventory.Command;
using AspNetEmptyApp.Feature.Inventory.DTOs;
using AspNetEmptyApp.Feature.Inventory.Model;

public class CreateCategoryHandler
{
    private readonly AppDbContext _context;

    public CreateCategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponseDto> HandleAsync(CreateCategoryCommand command)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            CreatedBy = command.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedBy = category.CreatedBy,
            CreatedAt = category.CreatedAt
        };
    }
}
