using RetailOrdering.API.DTOs.Inventory;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepo;

    public InventoryService(IInventoryRepository inventoryRepo)
    {
        _inventoryRepo = inventoryRepo;
    }

    public async Task<InventoryDto?> GetByProductIdAsync(int productId)
    {
        var inv = await _inventoryRepo.GetByProductIdAsync(productId);
        return inv == null ? null : MapToDto(inv);
    }

    public async Task<InventoryDto?> UpdateAsync(int productId, UpdateInventoryDto dto)
    {
        var inv = await _inventoryRepo.GetByProductIdAsync(productId);
        if (inv == null) return null;

        inv.QuantityInStock = dto.QuantityInStock;
        inv.ReorderLevel = dto.ReorderLevel;
        var updated = await _inventoryRepo.UpdateAsync(inv);
        return MapToDto(updated);
    }

    public async Task<InventoryDto?> AdjustStockAsync(int productId, AdjustStockDto dto)
    {
        var inv = await _inventoryRepo.GetByProductIdAsync(productId);
        if (inv == null) return null;

        inv.QuantityInStock = Math.Max(0, inv.QuantityInStock + dto.Adjustment);
        var updated = await _inventoryRepo.UpdateAsync(inv);
        return MapToDto(updated);
    }

    public async Task<IEnumerable<InventoryDto>> GetLowStockItemsAsync()
    {
        var items = await _inventoryRepo.GetLowStockItemsAsync();
        return items.Select(MapToDto);
    }

    private static InventoryDto MapToDto(Models.Inventory i) => new()
    {
        InventoryId = i.InventoryId,
        ProductId = i.ProductId,
        ProductName = i.Product?.Name ?? string.Empty,
        QuantityInStock = i.QuantityInStock,
        ReorderLevel = i.ReorderLevel,
        LastUpdated = i.LastUpdated
    };
}