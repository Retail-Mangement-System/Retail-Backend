using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Brand;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/brands")]
[AllowAnonymous]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>GET /api/brands — list all brands (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _brandService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<BrandDto>>.SuccessResult(brands));
    }

    /// <summary>GET /api/brands/{id} — single brand (public)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand == null)
            return NotFound(ApiResponse<string>.FailResult("Brand not found"));
        return Ok(ApiResponse<BrandDto>.SuccessResult(brand));
    }

    /// <summary>POST /api/brands — create brand (Admin only)</summary>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var (brand, error) = await _brandService.CreateAsync(dto);
        if (error != null)
            return Conflict(ApiResponse<string>.FailResult(error));

        return CreatedAtAction(nameof(GetById), new { id = brand!.BrandId },
            ApiResponse<BrandDto>.SuccessResult(brand, "Brand created successfully"));
    }

    /// <summary>PUT /api/brands/{id} — update brand (Admin only)</summary>
    [HttpPut("{id:int}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateBrandDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.FailResult("Validation failed"));

        var (brand, error) = await _brandService.UpdateAsync(id, dto);
        if (error != null)
            return error.Contains("not found")
                ? NotFound(ApiResponse<string>.FailResult(error))
                : Conflict(ApiResponse<string>.FailResult(error));

        return Ok(ApiResponse<BrandDto>.SuccessResult(brand!, "Brand updated successfully"));
    }

    /// <summary>DELETE /api/brands/{id} — delete brand (Admin only)</summary>
    [HttpDelete("{id:int}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _brandService.DeleteAsync(id);
        if (!success)
            return error!.Contains("not found")
                ? NotFound(ApiResponse<string>.FailResult(error))
                : BadRequest(ApiResponse<string>.FailResult(error));

        return Ok(ApiResponse<string>.SuccessResult(null, "Brand deleted successfully"));
    }
}