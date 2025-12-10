using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.Common.Responses;
using CatalogHub.Application.DTOs.Product;
using CatalogHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogHub.Api.Controllers;

[ApiController]
[Route("cataloghub/v1/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest paginationRequest)
    {
        try
        {
            var products = await _productService.GetAllAsync(paginationRequest);
            return Ok(ApiResponse<PagedResponse<ProductResponse>>.Ok(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            return product != null
                ? Ok(ApiResponse<ProductResponse>.Ok(product))
                : NotFound(ApiResponse<string>.Fail("Produto não encontrado"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpGet("filter")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByFilters([FromQuery] Guid? categoryId,
                                                  [FromQuery] decimal? minPrice,
                                                  [FromQuery] decimal? maxPrice,
                                                  [FromQuery] bool? isActive)
    {
        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        {
            return BadRequest(ApiResponse<string>.Fail("O preço mínimo não pode ser maior que o preço máximo."));
        }

        try
        {
            var products = await _productService.GetByFiltersAsync(categoryId, minPrice, maxPrice, isActive);
            return Ok(ApiResponse<List<ProductResponse>>.Ok(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm] CreateProductRequest dto, IFormFile? image)
    {
        try
        {
            if (image != null)
            {
                if (image.Length > 5_000_000)
                    return BadRequest(ApiResponse<string>.Fail("Arquivo muito grande"));

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(image.ContentType))
                    return BadRequest(ApiResponse<string>.Fail("Tipo de arquivo inválido"));
            }

            var created = await _productService.CreateAsync(
                dto,
                image?.OpenReadStream(),
                image?.FileName,
                image?.ContentType
            );

            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, ApiResponse<ProductResponse>.Ok(created));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductRequest dto, IFormFile? image)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<string>.Fail("ID do corpo difere do ID da rota."));

            if (image != null)
            {
                if (image.Length > 5_000_000)
                    return BadRequest(ApiResponse<string>.Fail("Arquivo muito grande"));

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(image.ContentType))
                    return BadRequest(ApiResponse<string>.Fail("Tipo de arquivo inválido"));
            }

            var updated = await _productService.UpdateAsync(
                dto,
                image?.OpenReadStream(),
                image?.FileName,
                image?.ContentType
            );

            return Ok(ApiResponse<ProductResponse>.Ok(updated!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _productService.DeleteAsync(id);
            return Ok(ApiResponse<ProductResponse>.Ok(deleted!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }
}
