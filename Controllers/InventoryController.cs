using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Inventory;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/inventory")]
//[Authorize(Roles = "Admin")]
[AllowAnonymous]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>GET /api/inventory/{productId} — get stock for a product</summary>
    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var inv = await _inventoryService.GetByProductIdAsync(productId);
        if (inv == null)
            return NotFound(ApiResponse<string>.FailResult("Inventory record not found"));

        return Ok(ApiResponse<InventoryDto>.SuccessResult(inv));
    }

    /// <summary>PUT /api/inventory/{productId} — set stock levels</summary>
    [HttpPut("{productId:int}")]
    public async Task<IActionResult> Update(int productId, [FromBody] UpdateInventoryDto dto)
    {
        var inv = await _inventoryService.UpdateAsync(productId, dto);
        if (inv == null)
            return NotFound(ApiResponse<string>.FailResult("Inventory record not found"));

        return Ok(ApiResponse<InventoryDto>.SuccessResult(inv, "Inventory updated"));
    }

    /// <summary>PATCH /api/inventory/{productId}/adjust — adjust stock by delta</summary>
    [HttpPatch("{productId:int}/adjust")]
    public async Task<IActionResult> Adjust(int productId, [FromBody] AdjustStockDto dto)
    {
        var inv = await _inventoryService.AdjustStockAsync(productId, dto);
        if (inv == null)
            return NotFound(ApiResponse<string>.FailResult("Inventory record not found"));

        return Ok(ApiResponse<InventoryDto>.SuccessResult(inv, "Stock adjusted"));
    }

    /// <summary>GET /api/inventory/low-stock — all items below reorder level</summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var items = await _inventoryService.GetLowStockItemsAsync();
        return Ok(ApiResponse<IEnumerable<InventoryDto>>.SuccessResult(items));
    }
}