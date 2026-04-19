using RetailOrdering.API.DTOs.Brand;

namespace RetailOrdering.API.Services.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync();
    Task<BrandDto?> GetByIdAsync(int id);
    Task<(BrandDto? dto, string? error)> CreateAsync(CreateBrandDto dto);
    Task<(BrandDto? dto, string? error)> UpdateAsync(int id, CreateBrandDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}