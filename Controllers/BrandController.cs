using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
{
    private readonly IProductService _productService;

    public BrandController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>GET /api/brands — list all brands (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _productService.GetBrandsAsync();
        return Ok(ApiResponse<IEnumerable<BrandDto>>.SuccessResult(brands));
    }
}