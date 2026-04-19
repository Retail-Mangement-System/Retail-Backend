using RetailOrdering.API.DTOs.Brand;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;

    public BrandService(IBrandRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync()
    {
        var brands = await _repo.GetAllAsync();
        var dtos = new List<BrandDto>();
        foreach (var b in brands)
        {
            dtos.Add(new BrandDto
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                Description = b.Description,
                ProductCount = await _repo.GetProductCountAsync(b.BrandId)
            });
        }
        return dtos;
    }

    public async Task<BrandDto?> GetByIdAsync(int id)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return null;
        return new BrandDto
        {
            BrandId = b.BrandId,
            BrandName = b.BrandName,
            Description = b.Description,
            ProductCount = await _repo.GetProductCountAsync(b.BrandId)
        };
    }

    public async Task<(BrandDto? dto, string? error)> CreateAsync(CreateBrandDto dto)
    {
        if (await _repo.NameExistsAsync(dto.BrandName))
            return (null, $"Brand '{dto.BrandName}' already exists.");

        var brand = new Brand
        {
            BrandName = dto.BrandName.Trim(),
            Description = dto.Description.Trim()
        };

        var created = await _repo.CreateAsync(brand);
        return (new BrandDto
        {
            BrandId = created.BrandId,
            BrandName = created.BrandName,
            Description = created.Description,
            ProductCount = 0
        }, null);
    }

    public async Task<(BrandDto? dto, string? error)> UpdateAsync(int id, CreateBrandDto dto)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand == null) return (null, "Brand not found.");

        if (await _repo.NameExistsAsync(dto.BrandName, excludeId: id))
            return (null, $"Brand '{dto.BrandName}' already exists.");

        brand.BrandName = dto.BrandName.Trim();
        brand.Description = dto.Description.Trim();

        var updated = await _repo.UpdateAsync(brand);
        return (new BrandDto
        {
            BrandId = updated.BrandId,
            BrandName = updated.BrandName,
            Description = updated.Description,
            ProductCount = await _repo.GetProductCountAsync(updated.BrandId)
        }, null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var productCount = await _repo.GetProductCountAsync(id);
        if (productCount > 0)
            return (false, $"Cannot delete — {productCount} product(s) are linked to this brand.");

        var deleted = await _repo.DeleteAsync(id);
        return deleted ? (true, null) : (false, "Brand not found.");
    }
}