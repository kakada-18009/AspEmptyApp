namespace AspNetEmptyApp.Feature.Inventory.Command;

public record CreateCategoryCommand(string Name, string? Description, Guid CreatedBy);
