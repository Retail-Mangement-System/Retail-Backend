using RetailOrdering.API.DTOs.Category;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _repo.GetAllAsync();
        var dtos = new List<CategoryDto>();
        foreach (var c in categories)
        {
            dtos.Add(new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ProductCount = await _repo.GetProductCountAsync(c.CategoryId)
            });
        }
        return dtos;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;
        return new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ProductCount = await _repo.GetProductCountAsync(c.CategoryId)
        };
    }

    public async Task<(CategoryDto? dto, string? error)> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repo.NameExistsAsync(dto.CategoryName))
            return (null, $"Category '{dto.CategoryName}' already exists.");

        var category = new Category
        {
            CategoryName = dto.CategoryName.Trim(),
            Description = dto.Description.Trim()
        };

        var created = await _repo.CreateAsync(category);
        return (new CategoryDto
        {
            CategoryId = created.CategoryId,
            CategoryName = created.CategoryName,
            Description = created.Description,
            ProductCount = 0
        }, null);
    }

    public async Task<(CategoryDto? dto, string? error)> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return (null, "Category not found.");

        if (await _repo.NameExistsAsync(dto.CategoryName, excludeId: id))
            return (null, $"Category '{dto.CategoryName}' already exists.");

        category.CategoryName = dto.CategoryName.Trim();
        category.Description = dto.Description.Trim();

        var updated = await _repo.UpdateAsync(category);
        return (new CategoryDto
        {
            CategoryId = updated.CategoryId,
            CategoryName = updated.CategoryName,
            Description = updated.Description,
            ProductCount = await _repo.GetProductCountAsync(updated.CategoryId)
        }, null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var productCount = await _repo.GetProductCountAsync(id);
        if (productCount > 0)
            return (false, $"Cannot delete — {productCount} product(s) are linked to this category.");

        var deleted = await _repo.DeleteAsync(id);
        return deleted ? (true, null) : (false, "Category not found.");
    }
}