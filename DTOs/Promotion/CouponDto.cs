namespace RetailOrdering.API.DTOs.Promotion
{
    public class CouponDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
