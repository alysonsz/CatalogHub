using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.DTOs.Category;

namespace CatalogHub.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryResponse?> CreateAsync(CreateCategoryRequest dto);
    Task<CategoryResponse?> UpdateAsync(UpdateCategoryRequest dto);
    Task<CategoryResponse?> DeleteAsync(Guid id);
    Task<PagedResponse<CategoryResponse>> GetAllAsync(PaginationRequest paginationRequest);
    Task<CategoryDetailResponse?> GetByIdAsync(Guid id);
}
