// RetailOrdering.API/Services/Interfaces/ICartService.cs
using RetailOrdering.API.DTOs.Order;

namespace RetailOrdering.API.Services.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(int userId);
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}