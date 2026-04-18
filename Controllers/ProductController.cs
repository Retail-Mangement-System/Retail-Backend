using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Product;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>GET /api/products — paginated, filterable product list (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
    {
        var result = await _productService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResult(result));
    }

    /// <summary>GET /api/products/{id} — single product (public)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<string>.FailResult("Product not found"));

        return Ok(ApiResponse<ProductDto>.SuccessResult(product));
    }

    /// <summary>POST /api/products — create product (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId },
            ApiResponse<ProductDto>.SuccessResult(product, "Product created successfully"));
    }

    /// <summary>PUT /api/products/{id} — update product (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var product = await _productService.UpdateAsync(id, dto);
        if (product == null)
            return NotFound(ApiResponse<string>.FailResult("Product not found"));

        return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Product updated successfully"));
    }

    /// <summary>DELETE /api/products/{id} — delete product (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<string>.FailResult("Product not found"));

        return Ok(ApiResponse<string>.SuccessResult(null, "Product deleted successfully"));
    }
}