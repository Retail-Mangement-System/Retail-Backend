using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.DTOs.Product;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(ProductFilterDto filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Inventory)
            .AsQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Name.Contains(filter.Search) || p.Description.Contains(filter.Search));

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        // Sorting
        query = filter.SortBy.ToLower() switch
        {
            "price" => filter.SortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "createdat" => filter.SortOrder == "desc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => filter.SortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int productId)
        => await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int productId)
        => await _context.Products.AnyAsync(p => p.ProductId == productId);

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        => await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        => await _context.Brands.OrderBy(b => b.BrandName).ToListAsync();
}