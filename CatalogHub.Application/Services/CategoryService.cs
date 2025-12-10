using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.DTOs.Category;
using CatalogHub.Application.DTOs.Product;
using CatalogHub.Application.Interfaces.Services;
using CatalogHub.Application.Validators;
using CatalogHub.Domain.Interfaces.Repository;
using CatalogHub.Domain.Models;

namespace CatalogHub.Application.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<CategoryResponse?> CreateAsync(CreateCategoryRequest dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description
        };

        CategoryValidator.ValidateForCreate(category);

        var created = await repository.CreateAsync(category);

        return created is not null ? MapToResponse(created) : null;
    }

    public async Task<CategoryResponse?> UpdateAsync(UpdateCategoryRequest dto)
    {
        var existing = await repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        existing.Name = dto.Name.Trim();
        existing.Description = dto.Description;

        CategoryValidator.ValidateForUpdate(existing);

        var updated = await repository.UpdateAsync(existing);

        return updated is not null ? MapToResponse(updated) : null;
    }

    public async Task<CategoryResponse?> DeleteAsync(Guid id)
    {
        var existing = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        if (existing.Products != null && existing.Products.Any())
        {
            throw new InvalidOperationException("Não é possível excluir uma categoria que possui produtos associados.");
        }

        var deleted = await repository.DeleteAsync(existing)
            ?? throw new InvalidOperationException("Falha ao deletar a categoria.");

        return MapToResponse(deleted);
    }

    public async Task<PagedResponse<CategoryResponse>> GetAllAsync(PaginationRequest paginationRequest)
    {
        var (categories, totalCount) = await repository.GetAllAsync(paginationRequest.PageNumber, paginationRequest.PageSize);

        var responseList = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return new PagedResponse<CategoryResponse>(responseList, totalCount, paginationRequest.PageNumber, paginationRequest.PageSize);
    }

    public async Task<CategoryDetailResponse?> GetByIdAsync(Guid id)
    {
        var category = await repository.GetByIdAsync(id);
        return category is not null ?
            MapToDetailResponse(category) :
            null;
    }

    private static CategoryResponse MapToResponse(Category c) =>
        new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        };

    private CategoryDetailResponse MapToDetailResponse(Category category)
    {
        return new CategoryDetailResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Products = category.Products?.Select(p => new NestedProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                ImageUrl = p.ImageUrl
            }).ToList() ?? new List<NestedProductResponse>()
        };
    }
}
