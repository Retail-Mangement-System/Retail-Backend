using RetailOrdering.API.DTOs.Category;

namespace RetailOrdering.API.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<(CategoryDto? dto, string? error)> CreateAsync(CreateCategoryDto dto);
    Task<(CategoryDto? dto, string? error)> UpdateAsync(int id, CreateCategoryDto dto);
    Task<(bool success, string? error)> DeleteAsync(int id);
}