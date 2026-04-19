using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
        => await _context.Categories
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Categories.AnyAsync(c => c.CategoryId == id);

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        => await _context.Categories.AnyAsync(c =>
            c.CategoryName.ToLower() == name.ToLower() &&
            (excludeId == null || c.CategoryId != excludeId));

    public async Task<int> GetProductCountAsync(int categoryId)
        => await _context.Products.CountAsync(p => p.CategoryId == categoryId);
}