using RetailOrdering.API.DTOs.Product;
using RetailOrdering.API.Models;

namespace RetailOrdering.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(ProductFilterDto filter);
    Task<Product?> GetByIdAsync(int productId);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int productId);
    Task<bool> ExistsAsync(int productId);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
}