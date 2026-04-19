using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Product;
using RetailOrdering.API.DTOs.Category;
using RetailOrdering.API.DTOs.Brand;

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