using Microsoft.EntityFrameworkCore;
using RetailOrdering.API.Data;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
        => await _context.Brands
            .OrderBy(b => b.BrandName)
            .ToListAsync();

    public async Task<Brand?> GetByIdAsync(int id)
        => await _context.Brands
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.BrandId == id);

    public async Task<Brand> CreateAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand> UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return false;
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Brands.AnyAsync(b => b.BrandId == id);

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        => await _context.Brands.AnyAsync(b =>
            b.BrandName.ToLower() == name.ToLower() &&
            (excludeId == null || b.BrandId != excludeId));

    public async Task<int> GetProductCountAsync(int brandId)
        => await _context.Products.CountAsync(p => p.BrandId == brandId);
}