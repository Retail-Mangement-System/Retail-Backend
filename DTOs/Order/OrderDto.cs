// RetailOrdering.API/DTOs/Order/OrderDto.cs
namespace RetailOrdering.API.DTOs.Order;

public class OrderDto
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}