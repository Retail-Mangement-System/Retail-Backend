using RetailOrdering.API.DTOs.Inventory;

namespace RetailOrdering.API.Services.Interfaces;

public interface IInventoryService
{
    Task<InventoryDto?> GetByProductIdAsync(int productId);
    Task<InventoryDto?> UpdateAsync(int productId, UpdateInventoryDto dto);
    Task<InventoryDto?> AdjustStockAsync(int productId, AdjustStockDto dto);
    Task<IEnumerable<InventoryDto>> GetLowStockItemsAsync();
}