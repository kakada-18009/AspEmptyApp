namespace AspNetEmptyApp.Feature.Inventory.Command;

public record CreateProductionCommand(
    string Name,
    string? Description,
    decimal Price,
    int Quantity,
    Guid CategoryId,
    Guid CreatedBy
);
