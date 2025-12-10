using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.DTOs.Product;
using CatalogHub.Application.Interfaces.Services;
using CatalogHub.Application.Validators;
using CatalogHub.Domain.Interfaces.Repository;
using CatalogHub.Domain.Models;

namespace CatalogHub.Application.Services;

public class ProductService(IProductRepository repository, ICategoryRepository categoryRepository,
    IStorageService storage) : IProductService
{
    private static readonly string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

    public async Task<ProductResponse?> CreateAsync(CreateProductRequest dto, Stream? imageFile, 
        string? fileName, string? contentType)
    {
        await ValidateCategory(dto.CategoryId);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description ?? string.Empty,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            ImageUrl = await SaveImageAsync(imageFile, fileName, contentType)
        };

        ProductValidator.ValidateForCreate(product);

        var created = await repository.CreateAsync(product)
            ?? throw new InvalidOperationException("Falha ao criar o produto.");

        return await MapToResponse(created);
    }

    public async Task<ProductResponse?> UpdateAsync(UpdateProductRequest dto, Stream? imageFile, 
        string? fileName, string? contentType)
    {
        var existing = await repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        await ValidateCategory(dto.CategoryId);

        existing.Name = dto.Name.Trim();
        existing.Description = dto.Description ?? string.Empty;
        existing.Price = dto.Price;
        existing.StockQuantity = dto.StockQuantity;
        existing.IsActive = dto.IsActive;
        existing.CategoryId = dto.CategoryId;

        if (imageFile != null)
            existing.ImageUrl = await SaveImageAsync(imageFile, fileName, contentType);

        else if (dto.RemoveImage)
        {
            existing.ImageUrl = null;
        }

        ProductValidator.ValidateForUpdate(existing);

        var updated = await repository.UpdateAsync(existing)
            ?? throw new InvalidOperationException("Falha ao atualizar o produto.");

        return await MapToResponse(updated);
    }

    public async Task<ProductResponse?> DeleteAsync(Guid id)
    {
        var existing = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        var deleted = await repository.DeleteAsync(existing)
            ?? throw new InvalidOperationException("Falha ao deletar o produto.");

        return await MapToResponse(deleted);
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(PaginationRequest paginationRequest)
    {
        var (products, totalCount) = await repository.GetAllAsync(paginationRequest.PageNumber, paginationRequest.PageSize);

        var responseList = new List<ProductResponse>();
        foreach (var product in products)
        {
            responseList.Add(await MapToResponse(product));
        }

        return new PagedResponse<ProductResponse>(responseList, totalCount, paginationRequest.PageNumber, paginationRequest.PageSize);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await repository.GetByIdAsync(id);
        return product is null ? null : await MapToResponse(product);
    }

    public async Task<List<ProductResponse>> GetByFiltersAsync(Guid? categoryId, decimal? minPrice, decimal? maxPrice, bool? isActive)
    {
        var products = await repository.GetByFiltersAsync(categoryId, minPrice, maxPrice, isActive);
        var responseList = new List<ProductResponse>();

        if (products != null)
        {
            foreach (var product in products)
            {
                responseList.Add(await MapToResponse(product));
            }
        }

        return responseList;
    }

    private async Task ValidateCategory(Guid categoryId)
    {
        var exists = await categoryRepository.GetByIdAsync(categoryId);
        if (exists is null)
            throw new ArgumentException("Categoria inválida.");
    }

    private async Task<string?> SaveImageAsync(Stream? imageStream, string? fileName, string? contentType)
    {
        if (imageStream == null || string.IsNullOrWhiteSpace(fileName))
            return null;

        if (imageStream.CanSeek)
            imageStream.Position = 0;

        return await storage.UploadFileAsync(imageStream, fileName, contentType);
    }

    private async Task<ProductResponse> MapToResponse(Product p)
    {
        string? categoryName = null;

        if (p.CategoryId != Guid.Empty)
        {
            var category = await categoryRepository.GetByIdAsync(p.CategoryId);
            categoryName = category?.Name;
        }

        return new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = categoryName ?? string.Empty,
            ImageUrl = p.ImageUrl
        };
    }
}
