namespace RetailOrdering.API.DTOs.Product;

public class ProductFilterDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "name";        // name | price | createdAt
    public string SortOrder { get; set; } = "asc";      // asc | desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}