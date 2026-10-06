namespace AspNetEmptyApp.Feature.Inventory.DTOs;

public class CreateProductionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Guid CategoryId { get; set; }
}
