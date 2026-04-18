using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db) => _db = db;

    public async Task<Cart?> GetCartByUserIdAsync(int userId)
        => await _db.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await GetCartByUserIdAsync(userId);
        if (cart is not null) return cart;

        cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        => await _db.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        => await _db.CartItems.FindAsync(cartItemId);

    public async Task AddCartItemAsync(CartItem item)
    {
        _db.CartItems.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateCartItemAsync(CartItem item)
    {
        _db.CartItems.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveCartItemAsync(CartItem item)
    {
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int cartId)
    {
        var items = await _db.CartItems
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();
}