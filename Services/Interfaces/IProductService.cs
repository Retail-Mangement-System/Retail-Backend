using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Product;

namespace RetailOrdering.API.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllAsync(ProductFilterDto filter);
    Task<ProductDto?> GetByIdAsync(int productId);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int productId, CreateProductDto dto);
    Task<bool> DeleteAsync(int productId);
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    Task<IEnumerable<BrandDto>> GetBrandsAsync();
}

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class BrandDto
{
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}