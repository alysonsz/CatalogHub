namespace CatalogHub.Application.DTOs.Category;

public class CreateCategoryRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
