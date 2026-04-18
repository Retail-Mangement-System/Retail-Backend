// RetailOrdering.API/Services/Interfaces/IOrderService.cs
using RetailOrdering.API.DTOs.Order;

namespace RetailOrdering.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
    Task<List<OrderDto>> GetOrderHistoryAsync(int userId);
    Task<OrderDto?> GetOrderByIdAsync(int orderId);

    // Add to IOrderService.cs
    Task<OrderDto> ReorderAsync(int userId, int orderId, string shippingAddress);
}