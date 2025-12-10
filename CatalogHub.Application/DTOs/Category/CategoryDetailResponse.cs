using CatalogHub.Application.DTOs.Product;

namespace CatalogHub.Application.DTOs.Category;

public class CategoryDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public List<NestedProductResponse> Products { get; set; } = new();
}