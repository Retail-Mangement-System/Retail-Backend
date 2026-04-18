using RetailOrdering.API.Models;

namespace RetailOrdering.API.Repositories.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(int productId);
    Task<Inventory> CreateAsync(Inventory inventory);
    Task<Inventory> UpdateAsync(Inventory inventory);

    // Used by InventoryService — deducts an exact quantity
    Task<bool> DeductStockAsync(int productId, int quantity);

    // Used by OrderService (teammate) — applies a delta (+/-) to current stock
    Task<bool> UpdateStockAsync(int productId, int quantityDelta);

    Task<bool> HasSufficientStockAsync(int productId, int quantity);
    Task<IEnumerable<Inventory>> GetLowStockItemsAsync();
}