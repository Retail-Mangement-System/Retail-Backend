namespace RetailOrdering.API.DTOs.Promotion;

public class ValidateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderTotal { get; set; }
}

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
}