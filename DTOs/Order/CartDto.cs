namespace RetailOrdering.API.DTOs.Order
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = [];
        public decimal Total { get; set; }
    }
}
