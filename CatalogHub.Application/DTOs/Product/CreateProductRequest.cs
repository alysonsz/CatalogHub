namespace CatalogHub.Application.DTOs.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
}
