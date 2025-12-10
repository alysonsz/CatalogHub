namespace CatalogHub.Application.DTOs.Category;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
