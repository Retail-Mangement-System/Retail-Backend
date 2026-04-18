using RetailOrdering.API.Models;

namespace RetailOrdering.API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task SaveChangesAsync();
}