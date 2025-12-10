using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.DTOs.Product;

namespace CatalogHub.Application.Interfaces.Services;

public interface IProductService
{
    Task<ProductResponse?> CreateAsync(CreateProductRequest dto, Stream? imageFile, string? fileName, string? contentType);
    Task<ProductResponse?> UpdateAsync(UpdateProductRequest dto, Stream? imageFile, string? fileName, string? contentType);
    Task<ProductResponse?> DeleteAsync(Guid id);
    Task<PagedResponse<ProductResponse>> GetAllAsync(PaginationRequest paginationRequest);
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<List<ProductResponse>> GetByFiltersAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, bool? isActive);
}
