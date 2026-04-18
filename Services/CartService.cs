using RetailOrdering.API.DTOs.Order;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IInventoryRepository _inventoryRepo;

    public CartService(ICartRepository cartRepo, IInventoryRepository inventoryRepo)
    {
        _cartRepo = cartRepo;
        _inventoryRepo = inventoryRepo;
    }

    // GET CART ─────────────────────────────────────────────────────────────
    public async Task<CartDto?> GetCartAsync(int userId)
    {
        var cart = await _cartRepo.GetCartByUserIdAsync(userId);
        if (cart is null) return null;

        return MapCartToDto(cart);
    }

    // ADD TO CART ──────────────────────────────────────────────────────────
    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        // Check inventory
        var inventory = await _inventoryRepo.GetByProductIdAsync(productId)
            ?? throw new KeyNotFoundException("Product not found in inventory.");

        if (inventory.QuantityInStock < quantity)
            throw new ArgumentException($"Only {inventory.QuantityInStock} units available.");

        var cart = await _cartRepo.GetOrCreateCartAsync(userId);

        // If item already exists, increase quantity
        var existing = await _cartRepo.GetCartItemAsync(cart.CartId, productId);
        if (existing is not null)
        {
            var newQty = existing.Quantity + quantity;
            if (inventory.QuantityInStock < newQty)
                throw new ArgumentException($"Only {inventory.QuantityInStock} units available.");

            existing.Quantity = newQty;
            await _cartRepo.UpdateCartItemAsync(existing);
        }
        else
        {
            var item = new CartItem
            {
                CartId = cart.CartId,
                ProductId = productId,
                Quantity = quantity
            };
            await _cartRepo.AddCartItemAsync(item);
        }
    }

    // REMOVE FROM CART ─────────────────────────────────────────────────────
    public async Task RemoveFromCartAsync(int userId, int cartItemId)
    {
        var item = await _cartRepo.GetCartItemByIdAsync(cartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");

        // Security: verify the item belongs to this user's cart
        var cart = await _cartRepo.GetCartByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("Cart not found.");

        if (item.CartId != cart.CartId)
            throw new UnauthorizedAccessException("Access denied to this cart item.");

        await _cartRepo.RemoveCartItemAsync(item);
    }

    // CLEAR CART ───────────────────────────────────────────────────────────
    public async Task ClearCartAsync(int userId)
    {
        var cart = await _cartRepo.GetCartByUserIdAsync(userId);
        if (cart is null) return;

        await _cartRepo.ClearCartAsync(cart.CartId);
    }

    // HELPER ───────────────────────────────────────────────────────────────
    private static CartDto MapCartToDto(Cart cart)
    {
        var items = cart.CartItems.Select(ci => new CartItemDto
        {
            CartItemId = ci.CartItemId,
            ProductId = ci.ProductId,
            ProductName = ci.Product?.Name ?? string.Empty,
            Price = ci.Product?.Price ?? 0,
            Quantity = ci.Quantity,
            Subtotal = (ci.Product?.Price ?? 0) * ci.Quantity
        }).ToList();

        return new CartDto
        {
            CartId = cart.CartId,
            Items = items,
            Total = items.Sum(i => i.Subtotal)
        };
    }
}