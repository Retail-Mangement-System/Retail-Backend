using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByProductIdAsync(int productId)
        => await _context.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);

    public async Task<Inventory> CreateAsync(Inventory inventory)
    {
        inventory.LastUpdated = DateTime.UtcNow;
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    public async Task<Inventory> UpdateAsync(Inventory inventory)
    {
        inventory.LastUpdated = DateTime.UtcNow;
        _context.Inventories.Update(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    /// <summary>
    /// Deducts an exact quantity. Returns false if stock is insufficient.
    /// Used by InventoryService (admin stock correction).
    /// </summary>
    public async Task<bool> DeductStockAsync(int productId, int quantity)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory == null || inventory.QuantityInStock < quantity)
            return false;

        inventory.QuantityInStock -= quantity;
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Applies a +/- delta to current stock. Used by OrderService (teammate).
    /// Pass negative value to reduce stock on order confirm,
    /// positive to restore stock on order cancel.
    /// </summary>
    public async Task<bool> UpdateStockAsync(int productId, int quantityDelta)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory == null) return false;

        inventory.QuantityInStock = Math.Max(0, inventory.QuantityInStock + quantityDelta);
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasSufficientStockAsync(int productId, int quantity)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);
        return inventory != null && inventory.QuantityInStock >= quantity;
    }

    public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
        => await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.QuantityInStock <= i.ReorderLevel)
            .ToListAsync();
}