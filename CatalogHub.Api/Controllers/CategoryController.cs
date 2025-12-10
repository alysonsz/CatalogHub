using CatalogHub.Application.Common.Pagination;
using CatalogHub.Application.Common.Responses;
using CatalogHub.Application.DTOs.Category;
using CatalogHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogHub.Api.Controllers;

[ApiController]
[Route("cataloghub/v1/categories")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest paginationRequest)
    {
        try
        {
            var categories = await _categoryService.GetAllAsync(paginationRequest);
            return Ok(ApiResponse<PagedResponse<CategoryResponse>>.Ok(categories));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            return category != null
                ? Ok(ApiResponse<CategoryDetailResponse>.Ok(category))
                : NotFound(ApiResponse<string>.Fail("Categoria não encontrada"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest dto)
    {
        try
        {
            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, ApiResponse<CategoryResponse>.Ok(created));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<string>.Fail("ID do corpo difere do ID da rota."));

            var updated = await _categoryService.UpdateAsync(dto);
            return Ok(ApiResponse<CategoryResponse>.Ok(updated!));
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
            var deleted = await _categoryService.DeleteAsync(id);
            return Ok(ApiResponse<CategoryResponse>.Ok(deleted!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(ex.Message));
        }
    }
}
