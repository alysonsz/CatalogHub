namespace CatalogHub.Application.DTOs.Product;

public class UpdateProductRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public bool RemoveImage { get; set; }
}
