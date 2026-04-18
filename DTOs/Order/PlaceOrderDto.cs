namespace RetailOrdering.API.DTOs.Order;

public class PlaceOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
}