using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Category;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/categories")]
[AllowAnonymous]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>GET /api/categories — list all categories (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
    }

    /// <summary>GET /api/categories/{id} — single category (public)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.FailResult("Category not found"));
        return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
    }

    /// <summary>POST /api/categories — create category (Admin only)</summary>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var (category, error) = await _categoryService.CreateAsync(dto);
        if (error != null)
            return Conflict(ApiResponse<string>.FailResult(error));

        return CreatedAtAction(nameof(GetById), new { id = category!.CategoryId },
            ApiResponse<CategoryDto>.SuccessResult(category, "Category created successfully"));
    }

    /// <summary>PUT /api/categories/{id} — update category (Admin only)</summary>
    [HttpPut("{id:int}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var (category, error) = await _categoryService.UpdateAsync(id, dto);
        if (error != null)
            return error.Contains("not found")
                ? NotFound(ApiResponse<string>.FailResult(error))
                : Conflict(ApiResponse<string>.FailResult(error));

        return Ok(ApiResponse<CategoryDto>.SuccessResult(category!, "Category updated successfully"));
    }

    /// <summary>DELETE /api/categories/{id} — delete category (Admin only)</summary>
    [HttpDelete("{id:int}")]
   // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _categoryService.DeleteAsync(id);
        if (!success)
            return error!.Contains("not found")
                ? NotFound(ApiResponse<string>.FailResult(error))
                : BadRequest(ApiResponse<string>.FailResult(error));

        return Ok(ApiResponse<string>.SuccessResult(null, "Category deleted successfully"));
    }
}