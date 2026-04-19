namespace RetailOrdering.API.DTOs.Brand;

public class BrandDto
{
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}