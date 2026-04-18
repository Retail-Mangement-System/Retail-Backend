using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly IProductService _productService;

    public CategoryController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>GET /api/categories — list all categories (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _productService.GetCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
    }
}