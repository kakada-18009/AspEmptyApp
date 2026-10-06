namespace AspNetEmptyApp.Feature.Inventory.Handler;

using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Inventory.Command;
using AspNetEmptyApp.Feature.Inventory.DTOs;
using AspNetEmptyApp.Feature.Inventory.Model;

public class CreateProductionHandler
{
    private readonly AppDbContext _context;

    public CreateProductionHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductionResponseDto> HandleAsync(CreateProductionCommand command)
    {
        var production = new Production
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Quantity = command.Quantity,
            CategoryId = command.CategoryId,
            CreatedBy = command.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        // Command uses EF Core
        _context.Productions.Add(production);
        await _context.SaveChangesAsync();

        return new ProductionResponseDto
        {
            Id = production.Id,
            Name = production.Name,
            Description = production.Description,
            Price = production.Price,
            Quantity = production.Quantity,
            CategoryId = production.CategoryId,
            CreatedBy = production.CreatedBy,
            CreatedAt = production.CreatedAt
        };
    }
}
